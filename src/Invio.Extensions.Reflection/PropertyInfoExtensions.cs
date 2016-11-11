using System;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   A collections of extension methods on <see cref="PropertyInfo" />
    ///   that allows the caller to cache the reflection-based objects into
    ///   a delegate. This speeds up the getting and setting of values stored
    ///   in these properties from the extended <see cref="PropertyInfo" />.
    /// </summary>
    public static class PropertyInfoExtensions {

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the property is defined as well as the property's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="property" />.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///   The <see cref="PropertyInfo.PropertyType" /> on the provided
        ///   <paramref name="property" /> instance.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a getter, or
        ///   when <paramref name="property" /> references a static property, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="property" />, or the type specified for
        ///   <typeparamref name="TProperty" /> is not assignable to the type specified
        ///   on the <see cref="PropertyInfo.PropertyType" /> property on
        ///   <paramref name="property" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by calling <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, TProperty> CreateGetter<TBase, TProperty>(
            this PropertyInfo property) where TBase : class {

            var method = GetGetMethodInfo<TBase, TProperty>(property);

            return method.CreateFunc0<TBase, TProperty>();
        }

        /// <summary>
        ///   Return an efficient setter delegate for the specified property.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the property is defined as well as the property's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="property" />.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///   The <see cref="PropertyInfo.PropertyType" /> on the provided
        ///   <paramref name="property" /> instance.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a setter, or
        ///   when <paramref name="property" /> references a static property, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="property" />, or the type specified for
        ///   <typeparamref name="TProperty" /> is not assignable to the type specified
        ///   on the <see cref="PropertyInfo.PropertyType" /> property on
        ///   <paramref name="property" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set property values on
        ///   the parent by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, TProperty> CreateSetter<TBase, TProperty>(
            this PropertyInfo property) where TBase : class {

            var method = GetSetMethodInfo<TBase, TProperty>(property);

            return method.CreateAction1<TBase, TProperty>();
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the property is defined, but not for the property's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="property" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a getter, or
        ///   when <paramref name="property" /> references a static property, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="property" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, object> CreateGetter<TBase>(this PropertyInfo property)
            where TBase : class {

            return GetGetMethodInfo<TBase, object>(property).CreateFunc0<TBase>();
        }

        /// <summary>
        ///   Return an efficient setter delegate for the specified property.
        ///   The delegate is strongly compile-time typed for the base type
        ///   on which the property is defined, but not for the property's type.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="property" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a setter, or
        ///   when <paramref name="property" /> references a static property, or
        ///   the type specified for <typeparamref name="TBase" /> is not assignable
        ///   to the type specified on the <see cref="MemberInfo.DeclaringType" />
        ///   property on <paramref name="property" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set property values on
        ///   the parent by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, object> CreateSetter<TBase>(this PropertyInfo property)
            where TBase : class {

            return GetSetMethodInfo<TBase, object>(property).CreateAction1<TBase>();
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a getter, or
        ///   when <paramref name="property" /> references a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<object, object> CreateGetter(this PropertyInfo property) {
            return GetGetMethodInfo<object, object>(property).CreateFunc0();
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="property">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="property" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="property" /> does not have a setter, or
        ///   when <paramref name="property" /> references a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<object, object> CreateSetter(this PropertyInfo property) {
            return GetSetMethodInfo<object, object>(property).CreateAction1();
        }

        private static MethodInfo GetGetMethodInfo<TBase, TProperty>(PropertyInfo property)
            where TBase : class {

            CheckArguments<TBase, TProperty>(property);

            var getMethodInfo = property.GetGetMethod(nonPublic: true);

            if (getMethodInfo == null) {
                throw new ArgumentException(
                    $"The '{property.Name}' property on '{property.DeclaringType.Name}' " +
                    $"does not have a get accessor.",
                    nameof(property)
                );
            }

            if (getMethodInfo.IsStatic) {
                throw new ArgumentException(
                    $"The '{property.Name}' property is static.",
                    nameof(property)
                );
            }

            return getMethodInfo;
        }

        private static MethodInfo GetSetMethodInfo<TBase, TProperty>(PropertyInfo property)
            where TBase : class {

            CheckArguments<TBase, TProperty>(property);

            var setMethodInfo = property.GetSetMethod(nonPublic: true);

            if (setMethodInfo == null) {
                throw new ArgumentException(
                    $"The '{property.Name}' property on '{property.DeclaringType.Name}' " +
                    $"does not have a set accessor.",
                    nameof(property)
                );
            }

            if (setMethodInfo.IsStatic) {
                throw new ArgumentException(
                    $"The '{property.Name}' property is static.",
                    nameof(property)
                );
            }

            return setMethodInfo;
        }

        private static void CheckArguments<TBase, TProperty>(PropertyInfo property)
            where TBase : class {

            if (property == null) {
                throw new ArgumentNullException(nameof(property));
            }

            if (!typeof(TBase).IsAssignableFrom(property.DeclaringType)) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TBase)}' was '{typeof(TBase).Name}', " +
                    $"which is not assignable to the property's declaring type of " +
                    $"'{property.DeclaringType.Name}'.",
                    nameof(property)
                );
            }

            if (!typeof(TProperty).IsAssignableFrom(property.PropertyType)) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TProperty)}' was '{typeof(TProperty).Name}', " +
                    $"which is not assignable to the property's value type of " +
                    $"'{property.PropertyType.Name}'.",
                    nameof(property)
                );
            }
        }

    }

}
