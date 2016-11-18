using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {

    /// <summary>
    ///   A collections of extension methods on <see cref="MethodInfo" />
    ///   that allows the caller to cache the reflection-based objects into
    ///   a delegate. This speeds up the invocation of methods normally
    ///   performed via the extended <see cref="MethodInfo" />.
    /// </summary>
    public static class MethodInfoExtensions {

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 0-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="TBase">
        ///   A <see cref="Type" /> that is assignable to one that contains the
        ///   <see cref="MethodInfo" /> passed in via <paramref name="method" />.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///   A <see cref="Type" /> that is assignable to the one that is returned from the
        ///   <see cref="MethodInfo" /> passed is via <paramref name="method" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="method" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="method" /> is not parameterless, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified
        ///   on the <see cref="MemberInfo.DeclaringType" /> property on
        ///   <paramref name="method" />, or the type specified for
        ///   <typeparamref name="TResult" /> is not assignable to the type specified
        ///   on the <see cref="MethodInfo.ReturnType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, TResult> CreateFunc0<TBase, TResult>(this MethodInfo method)
            where TBase : class {

            CheckMethod<TBase>(method);
            CheckParameters(method.GetParameters(), 0);

            if (!method.ReturnType.IsAssignableFrom(typeof(TResult))) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TResult)}' was '{typeof(TResult).Name}', " +
                    $"which is not assignable to the method's return type of " +
                    $"{method.ReturnType.Name}.",
                    nameof(method)
                );
            }

            return CreateFunc<TBase, TResult, Func<TBase, TResult>>(method);
        }

        private static TFunc CreateFunc<TBase, TResult, TFunc>(MethodInfo method) {
            var instance = Expression.Parameter(typeof(TBase), "instance");
            var parameters = method.GetParameters();

            Func<ParameterInfo, int, ParameterExpression> toArgument =
                (info, index) => Expression.Parameter(typeof(object), $"argument{index}");

            var arguments = parameters.Select(toArgument).ToArray();

            Func<ParameterInfo, ParameterExpression, Expression> convert =
                (info, expression) => Expression.Convert(expression, info.ParameterType);

            var body = Expression.Convert(
                Expression.Call(
                    Expression.Convert(instance, method.DeclaringType),
                    method,
                    parameters
                        .Zip(arguments, convert)
                        .ToArray()
                ),
                typeof(TResult)
            );

            var lambda = Expression.Lambda<TFunc>(
                body,
                new ParameterExpression[] { instance }
                    .Concat(arguments)
                    .ToArray()
            );

            return lambda.Compile();
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 0-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be recalled efficiently.
        /// </param>
        /// <typeparam name="TBase">
        ///   A <see cref="Type" /> that is assignable to one that contains the
        ///   <see cref="MethodInfo" /> passed in via <paramref name="method" />.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="method" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="method" /> isn't parameterless.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke methods normally
        ///   performed by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object> CreateFunc0<TBase>(this MethodInfo methodInfo)
            where TBase : class {

            CheckMethod<TBase>(methodInfo);
            CheckParameters(methodInfo.GetParameters(), 0);

            var delegateBuilder = semiGenericFunc0Builder.MakeGenericMethod(
                typeof(TBase),
                methodInfo.ReturnType
            );

            return (Func<TBase, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 1-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object> CreateFunc1<T>(this MethodInfo methodInfo)
            where T : class {

            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 1);
            var delegateBuilder = semiGenericFunc1Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 2-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object> CreateFunc2<T>(this MethodInfo methodInfo)
            where T : class {

            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 2);
            var delegateBuilder = semiGenericFunc2Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object>)delegateBuilder.Invoke(
                null,
                new object[] { methodInfo }
            );
        }

        /// <summary>
        /// Return an efficient functor for the specified 3-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object> CreateFunc3<T>(
            this MethodInfo methodInfo) where T : class {

            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 3);
            var delegateBuilder = semiGenericFunc3Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 4-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object> CreateFunc4<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 4);
            var delegateBuilder = semiGenericFunc4Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 5-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object, object> CreateFunc5<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 5);
            var delegateBuilder = semiGenericFunc5Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 6-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object, object, object> CreateFunc6<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 6);
            var delegateBuilder = semiGenericFunc6Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 7-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object, object, object, object> CreateFunc7<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 7);
            var delegateBuilder = semiGenericFunc7Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 8-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object, object, object, object, object> CreateFunc8<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 8);
            var delegateBuilder = semiGenericFunc8Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                parameters[7].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 9-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Func<T, object, object, object, object, object, object, object, object, object, object> CreateFunc9<T>(this MethodInfo methodInfo) where T : class {
            CheckMethod<T>(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 9);
            var delegateBuilder = semiGenericFunc9Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                parameters[7].ParameterType,
                parameters[8].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<T, object, object, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        #region Semi Generic Func Builders & MethodInfos

        public static T CreateCompatibleDelegate<T>(Type instanceType, MethodInfo methodInfo) {
            if (instanceType == null) {
                throw new ArgumentNullException("instanceType");
            }
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }

            var methodTypes = methodInfo.GetParameters().Select(m => m.ParameterType);

            var arguments = methodTypes.Select(m => Expression.Parameter(m)).ToList();

            var instanceParameter = Expression.Parameter(instanceType);

            // Create method call.;
            var methodCall = Expression.Call(
                instanceParameter,
                methodInfo,
                arguments
                );

            arguments.Insert(0, instanceParameter);
            return Expression.Lambda<T>(
                methodCall,
                arguments
                ).Compile();
        }


        private static readonly MethodInfo semiGenericFunc0Builder =
            new Func<MethodInfo, Func<object, object>>(CreateSemiGenericFunc0Impl<object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc1Builder =
            new Func<MethodInfo, Func<object, object, object>>(
                CreateSemiGenericFunc1Impl<object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc2Builder =
            new Func<MethodInfo, Func<object, object, object, object>>(
                CreateSemiGenericFunc2Impl<object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc3Builder =
            new Func<MethodInfo, Func<object, object, object, object, object>>(
                CreateSemiGenericFunc3Impl<object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc4Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object>>(
                CreateSemiGenericFunc4Impl<object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc5Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object>>(
                CreateSemiGenericFunc5Impl<object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc6Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object>>(
                CreateSemiGenericFunc6Impl<object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc7Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object>>(
                CreateSemiGenericFunc7Impl<object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc8Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object, object>>(
                CreateSemiGenericFunc8Impl<object, object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericFunc9Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object, object, object>>(
                CreateSemiGenericFunc9Impl<object, object, object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static Func<S, object> CreateSemiGenericFunc0Impl<S, T>(MethodInfo methodInfo) where S : class {
            Func<S, T> func = CreateCompatibleDelegate<Func<S, T>>(typeof(S), methodInfo);
            Func<S, object> ret = (S target) => func(target);
            return ret;
        }

        private static Func<S, object, object> CreateSemiGenericFunc1Impl<S, T, U>(MethodInfo methodInfo) where S : class {
            Func<S, T, U> func = CreateCompatibleDelegate<Func<S, T, U>>(typeof(S), methodInfo);
            Func<S, object, object> ret = (S target, object p0) => func(target, (T)p0);
            return ret;
        }

        private static Func<S, object, object, object> CreateSemiGenericFunc2Impl<S, T, U, V>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V> func = CreateCompatibleDelegate<Func<S, T, U, V>>(typeof(S), methodInfo);
            Func<S, object, object, object> ret = (S target, object p0, object p1) => func(target, (T)p0, (U)p1);
            return ret;
        }

        private static Func<S, object, object, object, object> CreateSemiGenericFunc3Impl<S, T, U, V, W>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W> func = CreateCompatibleDelegate<Func<S, T, U, V, W>>(typeof(S), methodInfo);
            Func<S, object, object, object, object> ret = (S target, object p0, object p1, object p2) => func(target, (T)p0, (U)p1, (V)p2);
            return ret;
        }

        private static Func<S, object, object, object, object, object> CreateSemiGenericFunc4Impl<S, T, U, V, W, X>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3) => func(target, (T)p0, (U)p1, (V)p2, (W)p3);
            return ret;
        }

        private static Func<S, object, object, object, object, object, object> CreateSemiGenericFunc5Impl<S, T, U, V, W, X, Y>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X, Y>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3, object p4) => func(target, (T)p0, (U)p1, (V)p2, (W)p3, (X)p4);
            return ret;
        }

        private static Func<S, object, object, object, object, object, object, object> CreateSemiGenericFunc6Impl<S, T, U, V, W, X, Y, Z>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X, Y, Z>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3, object p4, object p5) => func(target, (T)p0, (U)p1, (V)p2, (W)p3, (X)p4, (Y)p5);
            return ret;
        }

        private static Func<S, object, object, object, object, object, object, object, object> CreateSemiGenericFunc7Impl<S, T, U, V, W, X, Y, Z, A>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X, Y, Z, A>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3, object p4, object p5, object p6) => func(target, (T)p0, (U)p1, (V)p2, (W)p3, (X)p4, (Y)p5, (Z)p6);
            return ret;
        }

        private static Func<S, object, object, object, object, object, object, object, object, object> CreateSemiGenericFunc8Impl<S, T, U, V, W, X, Y, Z, A, B>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A, B> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X, Y, Z, A, B>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3, object p4, object p5, object p6, object p7) => func(target, (T)p0, (U)p1, (V)p2, (W)p3, (X)p4, (Y)p5, (Z)p6, (A)p7);
            return ret;
        }

        private static Func<S, object, object, object, object, object, object, object, object, object, object> CreateSemiGenericFunc9Impl<S, T, U, V, W, X, Y, Z, A, B, C>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A, B, C> func = CreateCompatibleDelegate<Func<S, T, U, V, W, X, Y, Z, A, B, C>>(typeof(S), methodInfo);
            Func<S, object, object, object, object, object, object, object, object, object, object> ret = (S target, object p0, object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8) => func(target, (T)p0, (U)p1, (V)p2, (W)p3, (X)p4, (Y)p5, (Z)p6, (A)p7, (B)p8);
            return ret;
        }

        #endregion

        /// <summary>
        /// Return an efficient functor for the specified 0-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object> CreateFunc0(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            CheckParameters(methodInfo.GetParameters(), 0);
            var delegateBuilder = objectFunc0Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                methodInfo.ReturnType
            );

            return (Func<object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 1-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object> CreateFunc1(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 1);
            var delegateBuilder = objectFunc1Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 2-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object> CreateFunc2(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 2);
            var delegateBuilder = objectFunc2Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 3-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object> CreateFunc3(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 3);
            var delegateBuilder = objectFunc3Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 4-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object> CreateFunc4(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 4);
            var delegateBuilder = objectFunc4Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 5-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object> CreateFunc5(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 5);
            var delegateBuilder = objectFunc5Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 6-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object> CreateFunc6(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 6);
            var delegateBuilder = objectFunc6Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 7-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object> CreateFunc7(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 7);
            var delegateBuilder = objectFunc7Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 8-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object, object> CreateFunc8(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 8);
            var delegateBuilder = objectFunc8Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                parameters[7].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient functor for the specified 9-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object, object, object> CreateFunc9(this MethodInfo methodInfo) {
            CheckMethod(methodInfo);
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 9);
            var delegateBuilder = objectFunc9Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType,
                parameters[1].ParameterType,
                parameters[2].ParameterType,
                parameters[3].ParameterType,
                parameters[4].ParameterType,
                parameters[5].ParameterType,
                parameters[6].ParameterType,
                parameters[7].ParameterType,
                parameters[8].ParameterType,
                methodInfo.ReturnType
            );

            return (Func<object, object, object, object, object, object, object, object, object, object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        #region Non-Generic Func Builders & MethodInfos

        private static readonly MethodInfo objectFunc0Builder =
            new Func<MethodInfo, Func<object, object>>(CreateObjectFunc0Impl<object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc1Builder =
            new Func<MethodInfo, Func<object, object, object>>(
                CreateObjectFunc1Impl<object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc2Builder =
            new Func<MethodInfo, Func<object, object, object, object>>(
                CreateObjectFunc2Impl<object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc3Builder =
            new Func<MethodInfo, Func<object, object, object, object, object>>(
                CreateObjectFunc3Impl<object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc4Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object>>(
                CreateObjectFunc4Impl<object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc5Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object>>(
                CreateObjectFunc5Impl<object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc6Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object>>(
                CreateObjectFunc6Impl<object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc7Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object>>(
                CreateObjectFunc7Impl<object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc8Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object, object>>(
                CreateObjectFunc8Impl<object, object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static readonly MethodInfo objectFunc9Builder =
            new Func<MethodInfo, Func<object, object, object, object, object, object, object, object, object, object, object>>(
                CreateObjectFunc9Impl<object, object, object, object, object, object, object, object, object, object, object>
            )
            .GetMethodInfo()
            .GetGenericMethodDefinition();

        private static Func<object, object> CreateObjectFunc0Impl<S, T>(MethodInfo methodInfo) where S : class {
            Func<S, T> func = (Func<S, T>)methodInfo.CreateDelegate(
                typeof(Func<S, T>)
            );
            Func<object, object> ret = (object target) => func((S)target);
            return ret;
        }

        private static Func<object, object, object> CreateObjectFunc1Impl<S, T, U>(MethodInfo methodInfo) where S : class {
            Func<S, T, U> func = (Func<S, T, U>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U>)
            );
            Func<object, object, object> ret = (object target, object param0) => func((S)target, (T)param0);
            return ret;
        }

        private static Func<object, object, object, object> CreateObjectFunc2Impl<S, T, U, V>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V> func = (Func<S, T, U, V>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V>)
            );
            Func<object, object, object, object> ret = (object target, object param0, object param1) => func((S)target, (T)param0, (U)param1);
            return ret;
        }

        private static Func<object, object, object, object, object> CreateObjectFunc3Impl<S, T, U, V, W>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W> func = (Func<S, T, U, V, W>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W>)
            );
            Func<object, object, object, object, object> ret = (object target, object param0, object param1, object param2) => func((S)target, (T)param0, (U)param1, (V)param2);
            return ret;
        }

        private static Func<object, object, object, object, object, object> CreateObjectFunc4Impl<S, T, U, V, W, X>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X> func = (Func<S, T, U, V, W, X>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X>)
            );
            Func<object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3);
            return ret;
        }

        private static Func<object, object, object, object, object, object, object> CreateObjectFunc5Impl<S, T, U, V, W, X, Y>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y> func = (Func<S, T, U, V, W, X, Y>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X, Y>)
            );
            Func<object, object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3, object param4) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3, (X)param4);
            return ret;
        }

        private static Func<object, object, object, object, object, object, object, object> CreateObjectFunc6Impl<S, T, U, V, W, X, Y, Z>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z> func = (Func<S, T, U, V, W, X, Y, Z>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X, Y, Z>)
            );
            Func<object, object, object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3, object param4, object param5) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3, (X)param4, (Y)param5);
            return ret;
        }

        private static Func<object, object, object, object, object, object, object, object, object> CreateObjectFunc7Impl<S, T, U, V, W, X, Y, Z, A>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A> func = (Func<S, T, U, V, W, X, Y, Z, A>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X, Y, Z, A>)
            );
            Func<object, object, object, object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3, object param4, object param5, object param6) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3, (X)param4, (Y)param5, (Z)param6);
            return ret;
        }

        private static Func<object, object, object, object, object, object, object, object, object, object> CreateObjectFunc8Impl<S, T, U, V, W, X, Y, Z, A, B>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A, B> func = (Func<S, T, U, V, W, X, Y, Z, A, B>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X, Y, Z, A, B>)
            );
            Func<object, object, object, object, object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3, object param4, object param5, object param6, object param7) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3, (X)param4, (Y)param5, (Z)param6, (A)param7);
            return ret;
        }

        private static Func<object, object, object, object, object, object, object, object, object, object, object> CreateObjectFunc9Impl<S, T, U, V, W, X, Y, Z, A, B, C>(MethodInfo methodInfo) where S : class {
            Func<S, T, U, V, W, X, Y, Z, A, B, C> func = (Func<S, T, U, V, W, X, Y, Z, A, B, C>)methodInfo.CreateDelegate(
                typeof(Func<S, T, U, V, W, X, Y, Z, A, B, C>)
            );
            Func<object, object, object, object, object, object, object, object, object, object, object> ret = (object target, object param0, object param1, object param2, object param3, object param4, object param5, object param6, object param7, object param8) => func((S)target, (T)param0, (U)param1, (V)param2, (W)param3, (X)param4, (Y)param5, (Z)param6, (A)param7, (B)param8);
            return ret;
        }

        #endregion

        private static void CheckMethod<T>(MethodInfo methodInfo) where T : class {
            CheckMethod(methodInfo);
            if (!methodInfo.DeclaringType.IsAssignableFrom(typeof(T))) {
                throw new ArgumentException(
                    "The declaring type of the method did not match the generic " +
                    "parameter T.",
                    "methodInfo"
                );
            }
        }

        private static void CheckMethod(MethodInfo methodInfo) {
            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.ReturnType == typeof(void)) {
                throw new ArgumentException(
                    "You cannot create a Func delegate for a method with a " +
                    "void return type.",
                    nameof(methodInfo)
                );
            }
        }

        #region Actions

        /// <summary>
        /// Return an efficient action for the specified 0-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Action<object> CreateAction0(this MethodInfo methodInfo) {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            CheckParameters(methodInfo.GetParameters(), 0);
            var delegateBuilder = objectAction0Builder.MakeGenericMethod(
                methodInfo.DeclaringType
            );

            return (Action<object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient action for the specified 0-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<T> CreateAction0<T>(this MethodInfo methodInfo) where T : class {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            CheckParameters(methodInfo.GetParameters(), 0);
            var delegateBuilder = genericAction0Builder.MakeGenericMethod(
                typeof(T)
            );

            return (Action<T>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<object, object> CreateAction1(this MethodInfo methodInfo) {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 1);
            var delegateBuilder = objectAction1Builder.MakeGenericMethod(
                methodInfo.DeclaringType,
                parameters[0].ParameterType
            );

            return (Action<object, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<T, object> CreateAction1<T>(this MethodInfo methodInfo) where T : class {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 1);
            var delegateBuilder = semiGenericAction1Builder.MakeGenericMethod(
                typeof(T),
                parameters[0].ParameterType
            );

            return (Action<T, object>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// Both the target type and the parameter type are strongly typed at
        /// compile time.
        /// </summary>
        public static Action<T1, T2> CreateAction1<T1, T2>(this MethodInfo methodInfo) where T1 : class {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            var parameters = methodInfo.GetParameters();
            CheckParameters(parameters, 1);
            if (parameters[0].ParameterType != typeof(T2)) {
                throw new ArgumentException(
                    "The method's argument type must exactly match the generic type parameter T2.",
                    "methodInfo"
                );
            }
            var delegateBuilder = genericAction1Builder.MakeGenericMethod(
                typeof(T1),
                typeof(T2)
            );

            return (Action<T1, T2>)delegateBuilder.Invoke(null, new object[] { methodInfo });
        }

        #region Action Builders & MethodInfos

        private static readonly MethodInfo genericAction0Builder =
            new Func<MethodInfo, Action<object>>(CreateAction0Impl<object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo genericAction1Builder =
            new Func<MethodInfo, Action<object, object>>(CreateAction1Impl<object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo semiGenericAction1Builder =
            new Func<MethodInfo, Action<object, object>>(CreateSemiGenericAction1Impl<object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo objectAction0Builder =
            new Func<MethodInfo, Action<object>>(CreateObjectAction0Impl<object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static readonly MethodInfo objectAction1Builder =
            new Func<MethodInfo, Action<object, object>>(CreateObjectAction1Impl<object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

        private static Action<object> CreateObjectAction0Impl<S>(MethodInfo methodInfo) {
            Action<S> action = (Action<S>)methodInfo.CreateDelegate(
                typeof(Action<S>)
            );
            return (object target) => { action((S)target); };
        }

        private static Action<S> CreateAction0Impl<S>(MethodInfo methodInfo) where S : class {
            Action<S> action = (Action<S>)methodInfo.CreateDelegate(
                typeof(Action<S>)
            );
            return action;
        }

        private static Action<S, T> CreateAction1Impl<S, T>(MethodInfo methodInfo) where S : class {
            return (Action<S, T>)methodInfo.CreateDelegate(
                typeof(Action<S, T>)
            );
        }

        private static Action<S, object> CreateSemiGenericAction1Impl<S, T>(MethodInfo methodInfo) where S : class {
            var action = CreateAction1Impl<S, T>(methodInfo);
            return (S target, object param) => { action(target, (T)param); };
        }

        private static Action<object, object> CreateObjectAction1Impl<S, T>(MethodInfo methodInfo) where S : class {
            Action<S, T> action = (Action<S, T>)methodInfo.CreateDelegate(
                typeof(Action<S, T>)
            );
            return (object target, object param) => { action((S)target, (T)param); };
        }

        #endregion

        #endregion

        private static void CheckParameters(ParameterInfo[] parameters, int parameterCount) {
            if (parameters.Length != parameterCount) {
                throw new ArgumentException(
                    $"The method must take exactly {parameterCount} argument(s).",
                    "methodInfo"
                );
            }
        }

        public static bool IsImplementationOf(this MethodInfo methodInfo, MethodInfo interfaceMethod) {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            } else if (interfaceMethod == null) {
                throw new ArgumentNullException("interfaceMethod");
            }
            bool result;
            if (methodInfo == interfaceMethod) {
                result = true;
            } else if (!methodInfo.DeclaringType.GetTypeInfo().IsInterface) {
                var interfaceType = interfaceMethod.DeclaringType;
                if (interfaceType.IsAssignableFrom(methodInfo.DeclaringType)) {
                    var map = methodInfo.DeclaringType.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
                    result = Array.IndexOf(map.InterfaceMethods, interfaceMethod) ==
                            Array.IndexOf(map.TargetMethods, methodInfo);
                } else {
                    result = false;
                }
            } else {
                result = false;
            }
            return result;
        }

    }
}
