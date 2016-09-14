using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    public static class ConstructorInfoExtensions {

        private static ConcurrentDictionary<ConstructorInfo, Func<object[], object>> cache;

        static ConstructorInfoExtensions() {
            cache = new ConcurrentDictionary<ConstructorInfo, Func<object[], object>>();
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <returns>
        ///   A <see cref="Func<object[], object>" /> delegate that can create efficiently
        ///   create instances normally created by <paramref name="constructor" />.
        /// </returns>
        public static Func<object[], object> CreateArrayFunc(this ConstructorInfo constructor) {
            if (constructor == null) {
                throw new ArgumentNullException(nameof(constructor));
            }

            return cache.GetOrAdd(
                constructor,
                _ => CreateArrayFuncImpl(constructor)
            );
        }

        private static Func<object[], object> CreateArrayFuncImpl(ConstructorInfo constructor) {
            var array = Expression.Parameter(typeof(object[]), "array");

            Func<ParameterInfo, int, Expression> toExpression =
                (ParameterInfo parameter, int index) => {
                    return Expression.Convert(
                        Expression.ArrayAccess(array, Expression.Constant(index)),
                        parameter.ParameterType
                    );
                };

            var parameters =
                constructor
                    .GetParameters()
                    .Select(toExpression)
                    .ToArray();

            var body = Expression.Block(
                Expression.IfThen(
                    Expression.Equal(array, Expression.Constant(null, array.Type)),
                    Expression.Throw(
                         Expression.Constant(
                             new ArgumentNullException(array.Name)
                         )
                    )
                ),
                Expression.IfThen(
                    Expression.NotEqual(
                        Expression.Property(array, typeof(object[]).GetProperty("Length")),
                        Expression.Constant(parameters.Length)
                    ),
                    Expression.Throw(
                        Expression.Constant(
                            new ArgumentException(
                                $"The '{array.Name}' argument must " +
                                $"have {parameters.Length:N0} objects.",
                                array.Name
                            )
                        )
                    )
                ),
                Expression.Convert(
                    Expression.New(constructor, parameters),
                    typeof(object)
                )
            );

            return Expression.Lambda<Func<object[], object>>(body, array).Compile();
        }

    }

}
