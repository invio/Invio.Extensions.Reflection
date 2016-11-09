using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   Extension methods to add functionality to instances of <see cref="Type" />.
    /// </summary>
    public static class TypeExtensions {

        /// <summary>
        ///   Makes a <see cref="String" /> that contains the type name with its
        ///   closing generic type arguments.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type" /> instance the caller wants to create the named
        ///   string for.
        /// </param>
        /// <returns>
        ///   A <see cref="String" /> that is unique to the type specified for the
        ///   fully qualified name with its closed generic type arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="type" /> is null.
        /// </exception>
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
    }

}
