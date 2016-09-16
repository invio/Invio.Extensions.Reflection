using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {
    // Credit to:
    // http://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo
    public static class FieldInfoExtensions {
        /// <summary>
        /// Return an efficient getter delegate for the specified field.
        /// </summary>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of it's construction.
        /// </remarks>
        public static Func<T, object> CreateGetter<T>(this FieldInfo fieldInfo) where T : class {
            if (fieldInfo == null) {
                throw new ArgumentNullException("fieldInfo");
            }
            if (fieldInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static fields. Please consider implementing " +
                    "CreateStaticGetter if you truly require this functionality."
                );
            }

            var delegateBuilder = genericGetBuilder.MakeGenericMethod(
                typeof(T),
                fieldInfo.FieldType
            );
            return (Func<T, object>)delegateBuilder.Invoke(null, new object[] { fieldInfo });
        }

        /// <summary>
        /// Return an efficient setter delegate for the specified field.
        /// </summary>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of it's construction.
        /// </remarks>
        public static Action<T, object> CreateSetter<T>(this FieldInfo fieldInfo) where T : class {
            if (fieldInfo == null) {
                throw new ArgumentNullException("fieldInfo");
            }
            if (fieldInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static fields. Please consider implementing " +
                    "CreateStaticSetter if you truly require this functionality."
                );
            }

            var delegateBuilder = genericSetBuilder.MakeGenericMethod(
                typeof(T),
                fieldInfo.FieldType
            );
            return (Action<T, object>)delegateBuilder.Invoke(null, new object[] { fieldInfo });
        }

        private static readonly MethodInfo genericGetBuilder = typeof(FieldInfoExtensions).GetMethod(
            "CreateGetDelegateImpl",
            BindingFlags.Static | BindingFlags.NonPublic
        );

        private static readonly MethodInfo genericSetBuilder = typeof(FieldInfoExtensions).GetMethod(
            "CreateSetDelegateImpl",
            BindingFlags.Static | BindingFlags.NonPublic
        );

        private static Func<S, object> CreateGetDelegateImpl<S, T>(FieldInfo fieldInfo) where S : class {
            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Field(instExp, fieldInfo);
            var getter = Expression.Lambda<Func<S, T>>(fieldExp, instExp).Compile();
            return (S s) => (object)getter(s);
        }

        private static Action<S, object> CreateSetDelegateImpl<S, T>(FieldInfo fieldInfo) where S : class {
            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Field(instExp, fieldInfo);
            var valueExp = Expression.Parameter(typeof(T));
            var setter = Expression.Lambda<Action<S, T>>(Expression.Assign(fieldExp, valueExp), instExp, valueExp).Compile();
            return (S s, object t) => { setter(s, (T)t); };
        }
    }
}
