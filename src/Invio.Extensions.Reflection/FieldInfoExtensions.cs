using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   A collections of extension methods on <see cref="FieldInfo" />
    ///   that allows the caller to cache access to the values stored
    ///   for that <see cref="FieldInfo" /> as a delegate.
    ///   This speeds up the getting and setting of values stored
    ///   in these fields from the extended <see cref="FieldInfo" />.
    /// </summary>
    public static class FieldInfoExtensions {

        /// <summary>
        ///   Return an efficient getter delegate for the specified field.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the field is defined, but not for the field's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="fieldInfo">
        ///   The <see cref="FieldInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="fieldInfo" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="fieldInfo" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="fieldInfo" /> represents a static field.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, object> CreateGetter<TBase>(this FieldInfo fieldInfo)
            where TBase : class {

            if (fieldInfo == null) {
                throw new ArgumentNullException("fieldInfo");
            }

            if (fieldInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static fields."
                );
            }

            var delegateBuilder = genericGetBuilder.MakeGenericMethod(
                typeof(TBase),
                fieldInfo.FieldType
            );

            return (Func<TBase, object>)delegateBuilder.Invoke(null, new object[] { fieldInfo });
        }

        /// <summary>
        ///   Return an efficient setter delegate for the specified field.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the field is defined, but not for the field's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="fieldInfo">
        ///   The <see cref="FieldInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="fieldInfo" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="fieldInfo" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="fieldInfo" /> represents a static field.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set field values on
        ///   the parent by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, object> CreateSetter<TBase>(this FieldInfo fieldInfo)
            where TBase : class {

            if (fieldInfo == null) {
                throw new ArgumentNullException("fieldInfo");
            }

            if (fieldInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static fields."
                );
            }

            var delegateBuilder = genericSetBuilder.MakeGenericMethod(
                typeof(TBase),
                fieldInfo.FieldType
            );

            return (Action<TBase, object>)delegateBuilder.Invoke(null, new object[] { fieldInfo });
        }

        private static readonly MethodInfo genericGetBuilder =
            typeof(FieldInfoExtensions).GetMethod(
                "CreateGetDelegateImpl",
                BindingFlags.Static | BindingFlags.NonPublic
            );

        private static readonly MethodInfo genericSetBuilder =
            typeof(FieldInfoExtensions).GetMethod(
                "CreateSetDelegateImpl",
                BindingFlags.Static | BindingFlags.NonPublic
            );

        private static Func<S, object> CreateGetDelegateImpl<S, T>(FieldInfo fieldInfo)
            where S : class {

            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Field(instExp, fieldInfo);
            var getter = Expression.Lambda<Func<S, T>>(fieldExp, instExp).Compile();
            return (S s) => (object)getter(s);
        }

        private static Action<S, object> CreateSetDelegateImpl<S, T>(FieldInfo fieldInfo)
            where S : class {

            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Field(instExp, fieldInfo);
            var valueExp = Expression.Parameter(typeof(T));
            var setter = Expression.Lambda<Action<S, T>>(
                Expression.Assign(fieldExp, valueExp), instExp, valueExp
            ).Compile();

            return (S s, object t) => { setter(s, (T)t); };
        }
    }
}
