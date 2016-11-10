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
        ///   on which the property is defined as well as the field's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="field" />.
        /// </typeparam>
        /// <typeparam name="TField">
        ///   An assignable <see cref="Type" /> for values stored in
        ///   <paramref name="field" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="field" />, or the type specified for
        ///   <typeparamref name="TField" /> is not assignable to the type specified
        ///   on the <see cref="FieldInfo.FieldType" /> property on <paramref name="field" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, TField> CreateGetter<TBase, TField>(this FieldInfo field)
            where TBase : class {

            return CreateGetterFuncImpl<TBase, TField, Func<TBase, TField>>(field);
        }

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
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="field" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="field" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, object> CreateGetter<TBase>(this FieldInfo field)
            where TBase : class {

            return CreateGetterFuncImpl<TBase, object, Func<TBase, object>>(field);
        }

        /// <summary>
        ///   Return an efficient setter delegate for the specified field.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Func<object, object> CreateGetter(this FieldInfo field) {
            return CreateGetterFuncImpl<object, object, Func<object, object>>(field);
        }

        /// <summary>
        ///   Return an efficient setter delegate for the specified field.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the property is defined as well as the field's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="field" />.
        /// </typeparam>
        /// <typeparam name="TField">
        ///   An assignable <see cref="Type" /> for values stored in
        ///   <paramref name="field" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="field" />, or the type specified for
        ///   <typeparamref name="TField" /> is not assignable to the type specified
        ///   on the <see cref="FieldInfo.FieldType" /> property on <paramref name="field" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set field values on
        ///   the parent by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, TField> CreateSetter<TBase, TField>(this FieldInfo field)
            where TBase : class {

            return CreateSetterActionImpl<TBase, TField, Action<TBase, TField>>(field);
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
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="FieldInfo" />
        ///   passed in via <paramref name="field" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="field" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set field values on
        ///   the parent by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, object> CreateSetter<TBase>(this FieldInfo field)
            where TBase : class {

            return CreateSetterActionImpl<TBase, object, Action<TBase, object>>(field);
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified field.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="field">
        ///   The <see cref="FieldInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="field" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="field" /> represents a static field.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set field values on
        ///   the parent by utilizing <see cref="FieldInfo" /> via reflection.
        /// </returns>
        public static Action<object, object> CreateSetter(this FieldInfo field) {
            return CreateSetterActionImpl<object, object, Action<object, object>>(field);
        }

        private static TFunc CreateGetterFuncImpl<TBase, TField, TFunc>(
            FieldInfo field) where TBase : class {

            CheckArguments<TBase, TField>(field);

            var instance = Expression.Parameter(typeof(TBase), "instance");

            var body = Expression.Field(
                Expression.Convert(instance, field.DeclaringType),
                field
            );

            return Expression.Lambda<TFunc>(body, instance).Compile();
        }

        private static TAction CreateSetterActionImpl<TBase, TField, TAction>(
            FieldInfo field) where TBase : class {

            CheckArguments<TBase, TField>(field);

            var instance = Expression.Parameter(typeof(TBase), "instance");
            var fieldValue = Expression.Parameter(typeof(TField), "fieldValue");

            var body = Expression.Assign(
                 Expression.Field(
                     Expression.Convert(instance, field.DeclaringType),
                     field
                 ),
                 Expression.Convert(fieldValue, field.FieldType)
            );

            return Expression.Lambda<TAction>(body, instance, fieldValue).Compile();
        }

        private static void CheckArguments<TBase, TField>(FieldInfo field)
            where TBase : class {

            if (field == null) {
                throw new ArgumentNullException(nameof(field));
            }

            if (!typeof(TBase).IsAssignableFrom(field.DeclaringType)) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TBase)}' was '{typeof(TBase).Name}', " +
                    $"which is not assignable to the the field's declaring type of " +
                    $"'{field.DeclaringType.Name}'.",
                    nameof(field)
                );
            }

            if (!typeof(TField).IsAssignableFrom(field.FieldType)) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TField)}' was '{typeof(TField).Name}', " +
                    $"which is not assignable to the the field's value type of " +
                    $"'{field.FieldType.Name}'.",
                    nameof(field)
                );
            }

            if (field.IsStatic) {
                throw new ArgumentException(
                    $"The '{field.Name}' field is static.",
                    nameof(field)
                );
            }
        }

    }

}
