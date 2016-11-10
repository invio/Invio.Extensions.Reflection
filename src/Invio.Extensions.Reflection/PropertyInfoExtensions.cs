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
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public get accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="propertyInfo" />.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///   The <see cref="PropertyInfo.PropertyType" /> on the provided
        ///   <paramref name="propertyInfo" /> instance.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a getter,
        ///   or <paramref name="nonPublic" /> is 'false' and the getter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by calling <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, TProperty> CreateGetter<TBase, TProperty>(
            this PropertyInfo propertyInfo, bool nonPublic = true) where TBase : class {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);
            return getMethodInfo.CreateFunc0<TBase, TProperty>();
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
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public set accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="propertyInfo" />.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///   The <see cref="PropertyInfo.PropertyType" /> on the provided
        ///   <paramref name="propertyInfo" /> instance.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a setter,
        ///   or <paramref name="nonPublic" /> is 'false' and the setter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set property values on
        ///   the parent by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, TProperty> CreateSetter<TBase, TProperty>(
            this PropertyInfo propertyInfo, bool nonPublic = true) where TBase : class {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);
            return setMethodInfo.CreateAction1<TBase, TProperty>();
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
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public get accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="propertyInfo" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a getter,
        ///   or <paramref name="nonPublic" /> is 'false' and the getter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<TBase, object> CreateGetter<TBase>(
            this PropertyInfo propertyInfo, bool nonPublic = true) where TBase : class {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);
            return getMethodInfo.CreateFunc0<TBase>();
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
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its set accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public set accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <typeparam name="TBase">
        ///   The <see cref="Type" /> that contains the <see cref="PropertyInfo" />
        ///   passed in via <paramref name="propertyInfo" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a setter,
        ///   or <paramref name="nonPublic" /> is 'false' and the setter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently set property values on
        ///   the parent by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<TBase, object> CreateSetter<TBase>(
            this PropertyInfo propertyInfo, bool nonPublic = true) where TBase : class {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);
            return setMethodInfo.CreateAction1<TBase>();
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public get accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a getter,
        ///   or <paramref name="nonPublic" /> is 'false' and the getter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Func<object, object> CreateGetter(
            this PropertyInfo propertyInfo, bool nonPublic = true) {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);
            return getMethodInfo.CreateFunc0();
        }

        /// <summary>
        ///   Return an efficient getter delegate for the specified property.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo" /> that will have its get accessor
        ///   cached into an efficient delegate for reuse.
        /// </param>
        /// <param name="nonPublic">
        ///   Indicates whether a non-public get accessor can be utilized.
        ///   'true' if a non-public accessor can be returned; otherwise, 'false'.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="propertyInfo" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="propertyInfo" /> does not have a getter,
        ///   or <paramref name="nonPublic" /> is 'false' and the getter is not public.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   Thrown when <paramref name="propertyInfo" /> represents a static property.
        /// </exception>
        /// <returns>
        ///   A delegate that can be called to efficiently fetch values normally
        ///   retreived by utilizing <see cref="PropertyInfo" /> via reflection.
        /// </returns>
        public static Action<object, object> CreateSetter(
            this PropertyInfo propertyInfo, bool nonPublic = true) {

            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }

            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);
            return setMethodInfo.CreateAction1();
        }

        private static MethodInfo GetGetMethodInfo(PropertyInfo propertyInfo, bool nonPublic) {
            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic);

            if (getMethodInfo == null) {
                var declaringTypeName =
                    propertyInfo
                        .DeclaringType
                        .GetNameWithGenericParameters();

                throw new ArgumentException(
                    $"The property '{propertyInfo.Name}' on type '{declaringTypeName}' " +
                    $"must have a getter in order to create a delegate",
                    nameof(propertyInfo)
                );
            }

            if (getMethodInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static properties."
                );
            }

            return getMethodInfo;
        }

        private static MethodInfo GetSetMethodInfo(PropertyInfo propertyInfo, bool nonPublic) {
            var setMethodInfo = propertyInfo.GetSetMethod(nonPublic);

            if (setMethodInfo == null) {
                var declaringTypeName =
                    propertyInfo
                        .DeclaringType
                        .GetNameWithGenericParameters();

                throw new ArgumentException(
                    $"The property '{propertyInfo.Name}' on type '{declaringTypeName}' " +
                    $"must have a setter in order to create a delegate",
                    nameof(propertyInfo)
                );
            }

            if (setMethodInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static properties."
                );
            }

            return setMethodInfo;
        }
    }
}
