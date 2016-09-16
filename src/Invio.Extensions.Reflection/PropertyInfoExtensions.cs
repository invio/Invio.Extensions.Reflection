using System;
using System.Reflection;

namespace Invio.Extensions.Reflection {
    public static class PropertyInfoExtensions {

        /// <summary>
        /// Return an efficient getter delegate for the specified property.
        /// The delegate is strongly compile-time typed for the base entity
        /// (holding the property), and the property type.
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Func<T1, T2> CreateGetter<T1, T2>(this PropertyInfo propertyInfo,
                                                        bool nonPublic = true) where T1 : class {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);
            return getMethodInfo.CreateFunc0<T1, T2>();
        }

        /// <summary>
        /// Return an efficient setter delegate for the specified property.
        /// The delegate is strongly compile-time typed for the base entity
        /// (holding the property), and the property type.
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Action<T1, T2> CreateSetter<T1, T2>(this PropertyInfo propertyInfo,
                                                          bool nonPublic = true) where T1 : class {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);

            return setMethodInfo.CreateAction1<T1, T2>();
        }

        /// <summary>
        /// Return an efficient getter delegate for the specified property.
        /// The delegate is strongly compile-time typed for the base entity
        /// (holding the property).
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Func<T, object> CreateGetter<T>(this PropertyInfo propertyInfo,
                                                      bool nonPublic = true) where T : class {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);

            return getMethodInfo.CreateFunc0<T>();
        }

        /// <summary>
        /// Return an efficient setter delegate for the specified property.
        /// The delegate is strongly compile-time typed for the base entity
        /// (holding the property).
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Action<T, object> CreateSetter<T>(this PropertyInfo propertyInfo,
                                                        bool nonPublic = true) where T : class {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);

            return setMethodInfo.CreateAction1<T>();
        }

        /// <summary>
        /// Return an efficient getter delegate for the specified property.
        /// The delegate is strongly run-time typed for the base entity
        /// (holding the property).
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Func<object, object> CreateGetter(this PropertyInfo propertyInfo,
                                                        bool nonPublic = true) {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var getMethodInfo = GetGetMethodInfo(propertyInfo, nonPublic);

            return getMethodInfo.CreateFunc0();
        }

        /// <summary>
        /// Return an efficient setter delegate for the specified property.
        /// The delegate is strongly run-time typed for the base entity
        /// (holding the property).
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned.
        /// true if a non-public accessor is to be returned; otherwise, false.
        /// </param>
        /// <remarks>
        /// Note: while use of the returned delegate is efficient, construction
        /// is _expensive_ - you should be getting significant re-use out of
        /// the delegate to justify the expense of its construction.
        /// </remarks>
        public static Action<object, object> CreateSetter(this PropertyInfo propertyInfo,
                                                          bool nonPublic = true) {
            if (propertyInfo == null) {
                throw new ArgumentNullException("propertyInfo");
            }
            var setMethodInfo = GetSetMethodInfo(propertyInfo, nonPublic);

            return setMethodInfo.CreateAction1();
        }

        private static MethodInfo GetGetMethodInfo(PropertyInfo propertyInfo, bool nonPublic) {
            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic);
            if (getMethodInfo == null) {
                var declaringTypeName = propertyInfo.DeclaringType.GetNameWithGenericParameters();
                throw new ArgumentException(
                    $"The property '{propertyInfo.Name}' on type '{declaringTypeName}' must have a getter in order " +
                    "to create a delegate",
                    "propertyInfo"
                );
            }
            if (getMethodInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static properties. Please consider implementing " +
                    "CreateStaticGetter if you truly require this functionality."
                );
            }
            return getMethodInfo;
        }

        private static MethodInfo GetSetMethodInfo(PropertyInfo propertyInfo, bool nonPublic) {
            var setMethodInfo = propertyInfo.GetSetMethod(nonPublic);
            if (setMethodInfo == null) {
                var declaringTypeName = propertyInfo.DeclaringType.GetNameWithGenericParameters();
                throw new ArgumentException(
                    "The property '{propertyInfo.Name}' on type '{declaringTypeName}' must have a setter in order " +
                    "to create a delegate",
                    "propertyInfo"
                );
            }
            if (setMethodInfo.IsStatic) {
                throw new NotSupportedException(
                    "This method does not support static properties. Please consider implementing " +
                    "CreateStaticSetter if you truly require this functionality."
                );
            }
            return setMethodInfo;
        }
    }
}
