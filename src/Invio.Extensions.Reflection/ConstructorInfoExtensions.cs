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

        public static Func<object> CreateFunc0(this ConstructorInfo constructor) {
            CheckParameters(constructor, expected: 0);

            var body = Expression.Convert(
                Expression.New(constructor),
                typeof(object)
            );

            return Expression.Lambda<Func<object>>(body).Compile();
        }

        public static Func<object, object>
            CreateFunc1(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 1);

            return CreateFunc<Func<object, object>>(constructor);
        }

        public static Func<object, object, object>
            CreateFunc2(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 2);

            return CreateFunc<Func<object, object, object>>(constructor);
        }

        public static Func<object, object, object, object>
            CreateFunc3(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 3);

            return CreateFunc<Func<object, object, object, object>>(constructor);
        }

        public static Func<object, object, object, object, object>
            CreateFunc4(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 4);

            return CreateFunc<Func<object, object, object, object, object>>(constructor);
        }

        public static Func<object, object, object, object, object, object>
            CreateFunc5(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 5);

            return CreateFunc<Func<object, object, object, object, object, object>>(constructor);
        }

        public static Func<object, object, object, object, object, object, object>
            CreateFunc6(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 6);

            return CreateFunc<Func<object, object, object, object, object, object, object>>(
                constructor
            );
        }

        public static Func<object, object, object, object, object, object, object, object>
            CreateFunc7(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 7);

            return CreateFunc<Func<object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        public static Func<object, object, object, object, object, object, object, object, object>
            CreateFunc8(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 8);

            return CreateFunc<Func<object, object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        public static Func<object, object, object, object, object, object, object, object, object, object>
            CreateFunc9(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 9);

            return CreateFunc<Func<object, object, object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        private static void CheckParameters(ConstructorInfo constructor, int expected) {
            if (constructor == null) {
                throw new ArgumentNullException(nameof(constructor));
            }

            var parameters = constructor.GetParameters();

            if (parameters.Length != expected) {
                throw new ArgumentException(
                    $"The '{nameof(constructor)}' argument must reference a " +
                    $"{typeof(ConstructorInfo).Name} with {expected:N0} parameters.",
                    nameof(constructor)
                );
            }
        }

        private static TFunc CreateFunc<TFunc>(ConstructorInfo constructor) {
            var parameters = constructor.GetParameters();

            Func<ParameterInfo, int, ParameterExpression> toArgument =
                (info, index) => Expression.Parameter(typeof(object), $"argument{index}");

            var arguments = parameters.Select(toArgument).ToArray();

            Func<ParameterInfo, ParameterExpression, Expression> convert =
                (info, expression) => Expression.Convert(expression, info.ParameterType);

            var body = Expression.Convert(
                Expression.New(
                    constructor,
                    parameters
                        .Zip(arguments, convert)
                        .ToArray()
                ),
                typeof(object)
            );

            return Expression.Lambda<TFunc>(body, arguments).Compile();
        }

    }

}
