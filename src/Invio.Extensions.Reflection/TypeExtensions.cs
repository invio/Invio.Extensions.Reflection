using System;
using System.Linq;
using System.Reflection;

namespace Invio.Extensions.Reflection {
    public static class TypeExtensions {

        /// <summary>
        /// Makes a string that contains the type name with it's generic parameters.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type" /> instance the caller wants to create the named
        ///   string for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="type" /> is null.
        /// </exception>
        /// <returns>
        ///   A string that is unique to the type specified for the fullyqualified name
        ///   and generic types.
        /// </returns>
        public static String GetNameWithGenericParameters(this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            var genericTypes = String.Join(", ", type.GenericTypeArguments.Select(gt => gt.FullName));
            var name = $"{type.Name}";

            if (!String.IsNullOrWhiteSpace(genericTypes)) {
                name += $"<{genericTypes}>";
            }

            return name;
        }
    }
}
