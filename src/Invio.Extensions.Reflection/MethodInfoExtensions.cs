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
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, TResult> CreateFunc0<TBase, TResult>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 0);
            CheckFunc<TBase, TResult>(method);

            return CreateFunc<TBase, TResult, Func<TBase, TResult>>(method);
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
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object> CreateFunc0<TBase>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 0);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 1-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object> CreateFunc1<TBase>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 1);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 2-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object> CreateFunc2<TBase>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 2);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 3-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object>
            CreateFunc3<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 3);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 4-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object>
            CreateFunc4<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 4);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 5-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object, object>
            CreateFunc5<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 5);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 6-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object, object, object>
            CreateFunc6<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 6);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 7-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object, object, object, object>
            CreateFunc7<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 7);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 8-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object, object, object, object, object>
            CreateFunc8<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 8);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        ///   Return an efficient delegate for the specified method which, when called,
        ///   invokes a 9-parameter method on the provided instance of the class.
        /// </summary>
        /// <remarks>
        ///   While use of the returned delegate is efficient, construction
        ///   is expensive. You should be getting significant re-use out of
        ///   the delegate to justify the expense of its construction.
        /// </remarks>
        /// <param name="method">
        ///   The <see cref="MethodInfo" /> instance the caller wants to turn into
        ///   a compiled delegate that can be called (and recalled) efficiently.
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
        ///   <paramref name="method" /> has a return type of void, or when
        ///   <paramref name="method" /> references a static property, or the type
        ///   <typeparamref name="TBase" /> is not assignable to the type specified on the
        ///   <see cref="MemberInfo.DeclaringType" /> property on <paramref name="method" />.
        /// </exception>
        /// <returns>
        ///   A delegate that can called to efficiently invoke the method normally
        ///   done by interacting with the <paramref name="method" /> directly.
        /// </returns>
        public static Func<TBase, object, object, object, object, object, object, object, object, object, object>
            CreateFunc9<TBase>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 9);
            CheckFunc<TBase>(method);

            return CreateFunc<TBase, object, Func<TBase, object, object, object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 0-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object> CreateFunc0(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 0);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 1-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object> CreateFunc1(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 1);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 2-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object> CreateFunc2(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 2);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 3-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object> CreateFunc3(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 3);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 4-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object> CreateFunc4(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 4);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 5-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object> CreateFunc5(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 5);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 6-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object> CreateFunc6(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 6);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 7-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object> CreateFunc7(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 7);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 8-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object, object> CreateFunc8(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 8);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object, object, object, object, object>>(method);
        }

        /// <summary>
        /// Return an efficient functor for the specified 9-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Func<object, object, object, object, object, object, object, object, object, object, object> CreateFunc9(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 9);
            CheckFunc(method);

            return CreateFunc<object, object, Func<object, object, object, object, object, object, object, object, object, object, object>>(method);
        }

        private static TFunc CreateFunc<TBase, TResult, TFunc>(MethodInfo method) {
            var instance = Expression.Parameter(typeof(TBase), "instance");
            var parameters = method.GetParameters();

            Func<ParameterInfo, int, ParameterExpression> toArgument =
                (info, index) => Expression.Parameter(typeof(object), $"argument{index}");

            var arguments = parameters.Select(toArgument).ToArray();

            Func<ParameterInfo, ParameterExpression, Expression> convert =
                (info, expression) => Expression.Convert(expression, info.ParameterType);

            var lambda = Expression.Lambda<TFunc>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(instance, method.DeclaringType),
                        method,
                        parameters
                            .Zip(arguments, convert)
                            .ToArray()
                    ),
                    typeof(TResult)
                ),
                new ParameterExpression[] { instance }
                    .Concat(arguments)
                    .ToArray()
            );

            return lambda.Compile();
        }

        private static void CheckFunc<TBase, TResult>(MethodInfo method) {
            CheckFunc<TBase>(method);

            if (!method.ReturnType.IsAssignableFrom(typeof(TResult))) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TResult)}' was '{typeof(TResult).Name}', " +
                    $"which is not assignable to the method's return type of " +
                    $"{method.ReturnType.Name}.",
                    nameof(method)
                );
            }
        }

        private static void CheckFunc<TBase>(MethodInfo method) {
            CheckFunc(method);

            if (!method.DeclaringType.IsAssignableFrom(typeof(TBase))) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TBase)}' was '{typeof(TBase).Name}', " +
                    $"which is not assignable to the method's declaring type of " +
                    $"'{method.DeclaringType.Name}'.",
                    nameof(method)
                );
            }
        }

        private static void CheckFunc(MethodInfo method) {
            if (method.ReturnType == typeof(void)) {
                throw new ArgumentException(
                    "You cannot create a Func delegate for a method with a " +
                    "void return type.",
                    nameof(method)
                );
            }
        }

        private static void CheckAction<TBase>(MethodInfo method) {
            CheckAction(method);

            if (!method.DeclaringType.IsAssignableFrom(typeof(TBase))) {
                throw new ArgumentException(
                    $"Type parameter '{nameof(TBase)}' was '{typeof(TBase).Name}', " +
                    $"which is not assignable to the method's declaring type of " +
                    $"'{method.DeclaringType.Name}'.",
                    nameof(method)
                );
            }
        }

        private static void CheckAction(MethodInfo method) {
            if (method.ReturnType != typeof(void)) {
                throw new ArgumentException(
                    "You cannot create an Action delegate for a method with a " +
                    "non-void return type.",
                    nameof(method)
                );
            }
        }

        private static void CheckMethod(MethodInfo method, int expectedParameters) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            if (method.IsStatic) {
                throw new ArgumentException(
                    $"The '{method.Name}' method is static.",
                    nameof(method)
                );
            }

            var parameters = method.GetParameters();

            if (parameters.Length != expectedParameters) {
                throw new ArgumentException(
                    $"The '{nameof(method)}' argument must reference a " +
                    $"{typeof(MethodInfo)} with {expectedParameters:N0} parameters.",
                    nameof(method)
                );
            }
        }

        /// <summary>
        /// Return an efficient action for the specified 0-parameter method.
        /// The delegate returned is strongly run-time typed.
        /// </summary>
        public static Action<object> CreateAction0(this MethodInfo method) {
            CheckMethod(method, expectedParameters: 0);
            CheckAction(method);

            return CreateAction<Action<object>>(
                typeof(object),
                new Type[0],
                method
            );
        }

        /// <summary>
        /// Return an efficient action for the specified 0-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<TBase> CreateAction0<TBase>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 0);
            CheckAction<TBase>(method);

            return CreateAction<Action<TBase>>(
                typeof(TBase),
                new Type[0],
                method
            );
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<object, object> CreateAction1(this MethodInfo method) {

            CheckMethod(method, expectedParameters: 1);
            CheckAction(method);

            return CreateAction<Action<object, object>>(
                typeof(object),
                new [] { typeof(object) },
                method
            );
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// The base entity for the delegate is strongly compile-time typed, the
        /// parameters are strongly run-time typed.
        /// </summary>
        public static Action<TBase, object> CreateAction1<TBase>(this MethodInfo method)
            where TBase : class {

            CheckMethod(method, expectedParameters: 1);
            CheckAction<TBase>(method);

            return CreateAction<Action<TBase, object>>(
                typeof(TBase),
                new [] { typeof(object) },
                method
            );
        }

        /// <summary>
        /// Return an efficient action for the specified 1-parameter method.
        /// Both the target type and the parameter type are strongly typed at
        /// compile time.
        /// </summary>
        public static Action<TBase, TParameter>
            CreateAction1<TBase, TParameter>(this MethodInfo method) where TBase : class {

            CheckMethod(method, expectedParameters: 1);
            CheckAction<TBase>(method);

            var parameters = method.GetParameters();

            if (parameters[0].ParameterType != typeof(TParameter)) {
                throw new ArgumentException(
                    "The method's argument type must exactly match the generic type parameter TParameter.",
                    "method"
                );
            }

            return CreateAction<Action<TBase, TParameter>>(
                typeof(TBase),
                new [] { typeof(TParameter) },
                method
            );
        }

        private static TAction CreateAction<TAction>(
            Type baseType,
            Type[] parameterTypes,
            MethodInfo method) {

            var instance = Expression.Parameter(baseType, "instance");
            var parameters = method.GetParameters();

            Func<ParameterInfo, int, ParameterExpression> toArgument =
                (info, index) =>
                    Expression.Parameter(parameterTypes[index], $"argument{index}");

            var arguments = parameters.Select(toArgument).ToArray();

            Func<ParameterInfo, ParameterExpression, Expression> convert =
                (info, expression) => Expression.Convert(expression, info.ParameterType);

            var lambda = Expression.Lambda<TAction>(
                Expression.Call(
                    Expression.Convert(instance, method.DeclaringType),
                    method,
                    parameters
                        .Zip(arguments, convert)
                        .ToArray()
                ),
                new ParameterExpression[] { instance }
                    .Concat(arguments)
                    .ToArray()
            );

            return lambda.Compile();
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
