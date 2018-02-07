using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   A collections of extension methods on <see cref="PropertyInfo" />
    ///   that allows the caller to cache the reflection-based objects into
    ///   a delegate. This speeds up the getting and setting of values stored
    ///   in these properties from the extended <see cref="PropertyInfo" />.
    /// </summary>
    public static class CachedDelegatesPropertyInfoExtensions {

        private static ConcurrentDictionary<Tuple<Type, Type, object>, object> setters { get; }
        private static ConcurrentDictionary<Tuple<Type, Type, object>, object> getters { get; }

        static CachedDelegatesPropertyInfoExtensions() {
            setters = new ConcurrentDictionary<Tuple<Type, Type, object>, object>();
            getters = new ConcurrentDictionary<Tuple<Type, Type, object>, object>();
        }

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

            return GetOrCreateGetter<TBase, TProperty>(property);
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

            return GetOrCreateSetter<TBase, TProperty>(property);
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

            return GetOrCreateGetter<TBase, object>(property, skipPropertyTypeCheck: true);
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

            return GetOrCreateSetter<TBase, object>(property, skipPropertyTypeCheck: true);
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
            return GetOrCreateGetter<object, object>(
                property,
                skipDeclaringTypeCheck: true,
                skipPropertyTypeCheck: true
            );
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
            return GetOrCreateSetter<object, object>(
                property,
                skipDeclaringTypeCheck: true,
                skipPropertyTypeCheck: true
            );
        }

        private static Func<TBase, TProperty> GetOrCreateGetter<TBase, TProperty>(
            PropertyInfo property,
            bool skipDeclaringTypeCheck = false,
            bool skipPropertyTypeCheck = false) where TBase : class {

            return (Func<TBase, TProperty>)getters.GetOrAdd(
                new Tuple<Type, Type, object>(typeof(TBase), typeof(TProperty), property),
                _ => CreateGetterImpl<TBase, TProperty>(
                    property,
                    skipDeclaringTypeCheck,
                    skipPropertyTypeCheck
                )
            );
        }

        private static Func<TBase, TProperty> CreateGetterImpl<TBase, TProperty>(
            PropertyInfo property,
            bool skipDeclaringTypeCheck,
            bool skipPropertyTypeCheck) where TBase : class {

            CheckArguments<TBase, TProperty>(
                property,
                skipDeclaringTypeCheck,
                skipPropertyTypeCheck
            );

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

            var instance = Expression.Parameter(typeof(TBase), "instance");

            var body = Expression.Convert(
                Expression.Call(
                    Expression.Convert(instance, property.DeclaringType),
                    getMethodInfo
                ),
                typeof(TProperty)
            );

            return Expression.Lambda<Func<TBase, TProperty>>(body, instance).Compile();
        }

        private static Action<TBase, TProperty> GetOrCreateSetter<TBase, TProperty>(
            PropertyInfo property,
            bool skipDeclaringTypeCheck = false,
            bool skipPropertyTypeCheck = false) where TBase : class {

            return (Action<TBase, TProperty>)setters.GetOrAdd(
                new Tuple<Type, Type, object>(typeof(TBase), typeof(TProperty), property),
                _ => CreateSetterImpl<TBase, TProperty>(
                    property,
                    skipDeclaringTypeCheck,
                    skipPropertyTypeCheck
                )
            );
        }

        private static Action<TBase, TProperty> CreateSetterImpl<TBase, TProperty>(
            PropertyInfo property,
            bool skipDeclaringTypeCheck,
            bool skipPropertyTypeCheck) where TBase : class {

            CheckArguments<TBase, TProperty>(
                property,
                skipDeclaringTypeCheck,
                skipPropertyTypeCheck
            );

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

            var instance = Expression.Parameter(typeof(TBase), "instance");
            var value = Expression.Parameter(typeof(TProperty), "value");

            var body = Expression.Assign(
                Expression.Property(
                    Expression.Convert(instance, property.DeclaringType),
                    property
                ),
                Expression.Convert(value, property.PropertyType)
            );

            return Expression.Lambda<Action<TBase, TProperty>>(body, instance, value).Compile();
        }

        private static void CheckArguments<TBase, TProperty>(
            PropertyInfo property,
            bool skipDeclaringTypeCheck,
            bool skipPropertyTypeCheck) where TBase : class {

            if (property == null) {
                throw new ArgumentNullException(nameof(property));
            }

            if (!skipDeclaringTypeCheck &&
                !property.DeclaringType.IsAssignableFrom(typeof(TBase))) {

                throw new ArgumentException(
                    $"Type parameter '{nameof(TBase)}' was '{typeof(TBase).Name}', " +
                    $"which is not assignable to the property's declaring type of " +
                    $"'{property.DeclaringType.Name}'.",
                    nameof(property)
                );
            }

            if (!skipPropertyTypeCheck &&
                !property.PropertyType.IsAssignableFrom(typeof(TProperty))) {

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
