using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   A collections of extension methods on <see cref="ConstructorInfo" />
    ///   that allows the caller to cache the reflection-based objects into
    ///   a delegate. This speeds up the instantiation of objects normally
    ///   created from the extended <see cref="ConstructorInfo" />.
    /// </summary>
    public static class ConstructorInfoExtensions {

        private static ConcurrentDictionary<ConstructorInfo, Func<object[], object>> untypedArrays { get; }
        private static ConcurrentDictionary<ConstructorInfo, object> typedArrays { get; }
        private static ConcurrentDictionary<ConstructorInfo, object> untypedFuncs { get; }
        private static ConcurrentDictionary<ConstructorInfo, object> typedFuncs { get; }

        static ConstructorInfoExtensions() {
            untypedArrays = new ConcurrentDictionary<ConstructorInfo, Func<object[], object>>();
            typedArrays = new ConcurrentDictionary<ConstructorInfo, object>();
            untypedFuncs = new ConcurrentDictionary<ConstructorInfo, object>();
            typedFuncs = new ConcurrentDictionary<ConstructorInfo, object>();
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object[], object> CreateArrayFunc(this ConstructorInfo constructor) {
            if (constructor == null) {
                throw new ArgumentNullException(nameof(constructor));
            }

            return untypedArrays.GetOrAdd(constructor, CreateArrayFuncImpl<object>);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object[], T> CreateArrayFunc<T>(this ConstructorInfo constructor) {
            CheckType<T>(constructor);

            return (Func<object[], T>)typedArrays.GetOrAdd(constructor, CreateArrayFuncImpl<T>);
        }

        private static Func<object[], T> CreateArrayFuncImpl<T>(ConstructorInfo constructor) {
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
                    typeof(T)
                )
            );

            return Expression.Lambda<Func<object[], T>>(body, array).Compile();
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> isn't parameterless.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object>
            CreateFunc0(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 0);

            return CreateUntypedFunc<Func<object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> isn't parameterless.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<T>
            CreateFunc0<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 0);

            return CreateTypedFunc<T, Func<T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 1 parameter.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object>
            CreateFunc1(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 1);

            return CreateUntypedFunc<Func<object, object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 1 parameter.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, T>
            CreateFunc1<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 1);

            return CreateTypedFunc<T, Func<object, T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 2 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object>
            CreateFunc2(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 2);

            return CreateUntypedFunc<Func<object, object, object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 2 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, T>
            CreateFunc2<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 2);

            return CreateTypedFunc<T, Func<object, object, T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 3 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object>
            CreateFunc3(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 3);

            return CreateUntypedFunc<Func<object, object, object, object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 3 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, T>
            CreateFunc3<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 3);

            return CreateTypedFunc<T, Func<object, object, object, T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 4 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object>
            CreateFunc4(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 4);

            return CreateUntypedFunc<Func<object, object, object, object, object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 4 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, T>
            CreateFunc4<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 4);

            return CreateTypedFunc<T, Func<object, object, object, object, T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 5 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object>
            CreateFunc5(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 5);

            return CreateUntypedFunc<Func<object, object, object, object, object, object>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 5 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, T>
            CreateFunc5<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 5);

            return CreateTypedFunc<T, Func<object, object, object, object, object, T>>(constructor);
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 6 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object>
            CreateFunc6(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 6);

            return CreateUntypedFunc<Func<object, object, object, object, object, object, object>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 6 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, T>
            CreateFunc6<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 6);

            return CreateTypedFunc<T, Func<object, object, object, object, object, object, T>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 7 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, object>
            CreateFunc7(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 7);

            return CreateUntypedFunc<Func<object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 7 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, T>
            CreateFunc7<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 7);

            return CreateTypedFunc<T, Func<object, object, object, object, object, object, object, T>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 8 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, object, object>
            CreateFunc8(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 8);

            return CreateUntypedFunc<Func<object, object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 8 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, object, T>
            CreateFunc8<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 8);

            return CreateTypedFunc<T, Func<object, object, object, object, object, object, object, object, T>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <see cref="ConstructorInfo" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 9 parameters.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, object, object, object>
            CreateFunc9(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 9);

            return CreateUntypedFunc<Func<object, object, object, object, object, object, object, object, object, object>>(
                constructor
            );
        }

        /// <summary>
        ///   Return an efficient functor for the specified constructor which, when called,
        ///   creates a new instance of the class that the <paramref name="constructor" />
        ///   references via type parameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="ConstructorInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="T">
        ///   The type of object that will be provided when the delegate that is returned
        ///   from this method is invoked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="constructor" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> does not have 9 parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="constructor" /> references a different type
        ///   in its <see cref="MemberInfo.DeclaringType" /> property than what
        ///   was provided via the type parameter <typeparamref name="T" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently create instances normally created
        ///   by invoking the <paramref name="constructor" /> directly.
        /// </returns>
        public static Func<object, object, object, object, object, object, object, object, object, T>
            CreateFunc9<T>(this ConstructorInfo constructor) {

            CheckParameters(constructor, expected: 9);

            return CreateTypedFunc<T, Func<object, object, object, object, object, object, object, object, object, T>>(
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

        private static void CheckType<T>(ConstructorInfo constructor) {
            if (constructor == null) {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (constructor.DeclaringType != typeof(T)) {
                throw new ArgumentException(
                    $"Type parameter '{typeof(T).Name}' does not match the " +
                    $"'{nameof(MemberInfo.DeclaringType)}' of the provided " +
                    $"'{typeof(ConstructorInfo).Name}' object.",
                    nameof(constructor)
                );
            }
        }

        private static TFunc CreateTypedFunc<T, TFunc>(ConstructorInfo constructor) {
            CheckType<T>(constructor);

            return (TFunc)typedFuncs.GetOrAdd(constructor, CreateFuncImpl<T, TFunc>);
        }


        private static TFunc CreateUntypedFunc<TFunc>(ConstructorInfo constructor) {
            return (TFunc)untypedFuncs.GetOrAdd(constructor, CreateFuncImpl<object, TFunc>);
        }

        private static object CreateFuncImpl<T, TFunc>(ConstructorInfo constructor) {
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
                typeof(T)
            );

            return Expression.Lambda<TFunc>(body, arguments).Compile();
        }

    }

}
