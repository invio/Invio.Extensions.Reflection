using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   Extension methods to add functionality to instances of <see cref="Type" />.
    /// </summary>
    public static class TypeExtensions {

        private static ConcurrentDictionary<Tuple<Type, Type>, bool>
            isDerivativeOfCache { get; }

        static TypeExtensions() {
            isDerivativeOfCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>();
        }

        /// <summary>
        ///   Makes a <see cref="String" /> that contains the type name with its
        ///   closing generic type arguments.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type" /> instance the caller wants to create the named
        ///   string for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="type" /> is null.
        /// </exception>
        /// <returns>
        ///   A <see cref="String" /> that is unique to the type specified for the
        ///   fully qualified name with its closed generic type arguments.
        /// </returns>
        public static String GetNameWithGenericParameters(this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            var genericTypes = String.Join(
                ", ",
                type.GenericTypeArguments.Select(genericType => genericType.FullName)
            );

            var name = type.Name;

            if (!String.IsNullOrWhiteSpace(genericTypes)) {
                name += $"<{genericTypes}>";
            }

            return name;
        }

        /// <summary>
        ///   Determine whether or not the <see cref="Type" /> of interest,
        ///   <paramref name="type" />, has the <paramref name="parentType" />
        ///   somewhere in its hierarchy.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type" /> instance whose hierarchy will be checked to see
        ///   if it is the same type as <paramref name="parentType" />, or if it
        ///   a derivative of <paramref name="parentType" />.
        /// </param>
        /// <param name="parentType">
        ///   The <see cref="Type" /> instance that may or may not be somewhere
        ///   in the hierarchy of <paramref name="type" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="type" /> or <paramref name="parentType" /> is null.
        /// </exception>
        /// <returns>
        ///   This method returns <c>true</c> if one of the follow situations is met:
        ///   <list type="bullet">
        ///     <item>
        ///       <description>
        ///         The <paremref name="type" /> and <paramref name="parentType" /> are
        ///         the same <see cref="Type" />.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <description>
        ///         The <paramref name="parentType" /> is either not a generic
        ///         <see cref="Type" />, or a closed generic <see cref="Type" />
        ///         and <paramref name="type" /> is assignable to that
        ///         <see cref="Type" />.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <description>
        ///         The <paramref name="parentType" /> is an open generic
        ///         <see cref="Type" />, and <paramref name="type" /> either
        ///         implements that <see cref="Type" />, or inherits from a
        ///         <see cref="Type" /> that implements that <see cref="Type" />.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <description>
        ///         The <paremref name="type" /> and <paramref name="parentType" /> are
        ///         the same <see cref="Type" />.
        ///       </description>
        ///     </item>
        ///   </list>
        ///   In all other situations, this method returns <c>false</c>.
        /// </returns>
        public static bool IsDerivativeOf(this Type type, Type parentType) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            } else if (parentType == null) {
                throw new ArgumentNullException(nameof(parentType));
            }

            return isDerivativeOfCache.GetOrAdd(
                Tuple.Create(type, parentType),
                tuple => IsDerivativeOfImpl(tuple.Item1, tuple.Item2)
            );
        }

        private static bool IsDerivativeOfImpl(Type type, Type parentType) {
            if (type == parentType || parentType == typeof(object)) {
                return true;
            }

            if (parentType.IsConstructedGenericType) {
                return parentType.IsAssignableFrom(type);
            }

            var parentTypeInfo = parentType.GetTypeInfo();

            if (!parentTypeInfo.IsGenericType) {
                return parentType.IsAssignableFrom(type);
            }

            IEnumerable<Type> potentialMatches;

            if (parentTypeInfo.IsInterface) {
                potentialMatches = type.GetInterfaces();
            } else {
                potentialMatches = GetBaseTypes(type);
            }

            return
                potentialMatches
                    .Select(potentialMatch => potentialMatch.GetTypeInfo())
                    .Where(potentialMatch => potentialMatch.IsGenericType)
                    .Select(potentialMatch => potentialMatch.GetGenericTypeDefinition())
                    .Any(potentialMatch => potentialMatch == parentType);
        }

        private static IEnumerable<Type> GetBaseTypes(Type type) {
            while (type != null) {
                yield return type;

                type = type.GetTypeInfo().BaseType;
            }
        }

    }

}
