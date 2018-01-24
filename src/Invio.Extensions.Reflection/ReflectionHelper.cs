using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Invio.Extensions.Reflection {
    /// <summary>
    ///   A collection of helper functions for retrieving <see cref="MemberInfo"/> instances using
    ///   expressions and method groups.
    /// </summary>
    public static class ReflectionHelper {

        /// <summary>
        ///   Retrieves the <see cref="MethodInfo" /> for an instance method from an expression that
        ///   invokes the method.
        /// </summary>
        /// <remarks>
        ///   The overloads of this method that take <see cref="Delegate" /> types may be more
        ///   performant than this method, however, this method is convenient for instance methods
        ///   when an instance of the type is not available. It also allows for more type inference
        ///   than the <see cref="Delegate" /> based alternatives.
        /// </remarks>
        /// <param name="invokeMethod">
        ///   An expression that invokes an instance method on
        ///   <typeparamref name="TDefiningType" />.
        /// </param>
        /// <typeparam name="TDefiningType">The type that defines the method.</typeparam>
        /// <returns>The <see cref="MethodInfo" /> for the invoked method.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="invokeMethod" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="invokeMethod" /> is not a
        ///   <see cref="MethodCallExpression" />.
        /// </exception>
        public static MethodInfo GetMethodFromExpression<TDefiningType>(
            Expression<Action<TDefiningType>> invokeMethod) {

            if (invokeMethod == null) {
                throw new ArgumentNullException(nameof(invokeMethod));
            }
            if (!(invokeMethod.Body is MethodCallExpression methodCall)) {
                throw new ArgumentException(
                    $"The body of the expression must be a {nameof(MethodCallExpression)}.",
                    nameof(invokeMethod)
                );
            }

            return methodCall.Method;
        }

        /// <summary>
        ///   Retrieves the <see cref="MethodInfo" /> for an static method from an expression that
        ///   invokes the method.
        /// </summary>
        /// <remarks>
        ///   The overloads of this method that take <see cref="Delegate" /> types may be more
        ///   performant than this method, however, this method allows for more type inference
        ///   than the <see cref="Delegate" /> based alternatives.
        /// </remarks>
        /// <param name="invokeMethod">
        ///   An expression that invokes an static method.
        /// </param>
        /// <returns>The <see cref="MethodInfo" /> for the invoked method.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="invokeMethod" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="invokeMethod" /> is not a
        ///   <see cref="MethodCallExpression" />.
        /// </exception>
        public static MethodInfo GetMethodFromExpression(Expression<Action> invokeMethod) {
            if (invokeMethod == null) {
                throw new ArgumentNullException(nameof(invokeMethod));
            }
            if (!(invokeMethod.Body is MethodCallExpression methodCall)) {
                throw new ArgumentException(
                    $"The body of the expression must be a {nameof(MethodCallExpression)}.",
                    nameof(invokeMethod)
                );
            }

            return methodCall.Method;
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action" /> delegate.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method can only be used with delegates created with method group syntax as
        ///     opposed to anonymous functions. For example
        ///     <code>ReflectionHelper.GetMethodInfo(Console.Beep)</code> will work, but
        ///     <code>ReflectionHelper.GetMethodInfo(() => Console.Beep())</code> will not.
        ///   </para>
        ///   <para>
        ///     When there are multiple overloads of a method it is important to use the matching
        ///     generic methods and specify type parameters in order to reference the correct
        ///     method.
        ///   </para>
        /// </remarks>
        /// <param name="method">
        ///   An <see cref="Action" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod(Action method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action{T1}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions. For example
        ///   <code>ReflectionHelper.GetMethodInfo&lt;String>(Console.WriteLine)</code> will work,
        ///   but
        ///   <code>ReflectionHelper.GetMethodInfo&lt;String>(() => Console.WriteLine(null))</code>
        ///   will not.
        /// </remarks>
        /// <typeparam name="T1">
        ///   The type of the first argument to the delegate.
        /// </typeparam>
        /// <param name="method">
        ///   An <see cref="Action{T1}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod<T1>(Action<T1> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action{T1, T2}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetActionMethod"/>
        /// <typeparam name="T1">
        ///   The type of the first argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T2">
        ///   The type of the second argument to the delegate.
        /// </typeparam>
        /// <param name="method">
        ///   An <see cref="Action{T1, T2}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod<T1, T2>(Action<T1, T2> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action{T1, T2, T3}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetActionMethod"/>
        /// <typeparam name="T1">
        ///   The type of the first argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T2">
        ///   The type of the second argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T3">
        ///   The type of the third argument to the delegate.
        /// </typeparam>
        /// <param name="method">
        ///   An <see cref="Action{T1, T2, T3}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod<T1, T2, T3>(Action<T1, T2, T3> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action{T1, T2, T3, T4}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetActionMethod"/>
        /// <typeparam name="T1">
        ///   The type of the first argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T2">
        ///   The type of the second argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T3">
        ///   The type of the third argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T4">
        ///   The type of the fourth argument to the delegate.
        /// </typeparam>
        /// <param name="method">
        ///   An <see cref="Action{T1, T2, T3, T4}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Action{T1, T2, T3, T4, T5}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetActionMethod"/>
        /// <typeparam name="T1">
        ///   The type of the first argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T2">
        ///   The type of the second argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T3">
        ///   The type of the third argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T4">
        ///   The type of the fourth argument to the delegate.
        /// </typeparam>
        /// <typeparam name="T5">
        ///   The type of the fifth argument to the delegate.
        /// </typeparam>
        /// <param name="method">
        ///   An <see cref="Action{T1, T2, T3, T4, T5}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetActionMethod<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> method) {

            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{TResult}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method can only be used with delegates created with method group syntax as
        ///     opposed to anonymous functions. For example
        ///     <code>ReflectionHelper.GetMethodInfo(Console.ReadKey)</code> will work, but
        ///     <code>ReflectionHelper.GetMethodInfo(() => Console.ReadKey())</code> will not.
        ///   </para>
        ///   <para>
        ///     When there are multiple overloads of a method it is important to use the matching
        ///     generic methods and specify type parameters for all arguments in order to reference
        ///     the correct method.
        ///   </para>
        /// </remarks>
        /// <param name="method">
        ///   A <see cref="Func{TResult}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<TResult>(Func<TResult> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{T1, TResult}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions. For example
        ///   <code>ReflectionHelper.GetMethodInfo&lt;Boolean, Int32>(Console.ReadKey)</code>
        ///   will work, but
        ///   <code>ReflectionHelper.GetMethodInfo&lt;Boolean, Int32>(() => Console.ReadKey(false))
        ///   </code> will not.
        /// </remarks>
        /// <param name="method">
        ///   A <see cref="Func{T1, TResult}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<T1, TResult>(Func<T1, TResult> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{T1, T2, TResult}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetFuncMethod{T1,TResult}"/>
        /// <param name="method">
        ///   A <see cref="Func{T1, T2, TResult}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<T1, T2, TResult>(Func<T1, T2, TResult> method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{T1, T2, T3, TResult}" /> delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetFuncMethod{T1,TResult}"/>
        /// <param name="method">
        ///   A <see cref="Func{T1, T2, T3, TResult}" /> delegate for a static or instance method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> method) {

            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{T1, T2, T3, T4, TResult}" />
        ///   delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetFuncMethod{T1,TResult}"/>
        /// <param name="method">
        ///   A <see cref="Func{T1, T2, T3, T4, TResult}" /> delegate for a static or instance
        ///   method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> method) {

            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the method info for an <see cref="Func{T1, T2, T3, T4, T5, TResult}" />
        ///   delegate.
        /// </summary>
        /// <remarks>
        ///   This method can only be used with delegates created with method group syntax as
        ///   opposed to anonymous functions.
        /// </remarks>
        /// <seealso cref="GetFuncMethod{T1,TResult}"/>
        /// <param name="method">
        ///   A <see cref="Func{T1, T2, T3, T4, T5, TResult}" /> delegate for a static or instance
        ///   method.
        /// </param>
        /// <returns>
        ///   The <see cref="MethodInfo" /> for the method referenced by the specified delegate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="method" /> argument is null.
        /// </exception>
        public static MethodInfo GetFuncMethod<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> method) {

            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            return method.GetMethodInfo();
        }

        /// <summary>
        ///   Retrieves the <see cref="MemberInfo" /> for an instance property or field based on an
        ///   expression that accesses it.
        /// </summary>
        /// <param name="getMember">
        ///   An expression that retrieves the value of a field or property.
        /// </param>
        /// <typeparam name="TDefiningType">The type that defines the field or property.</typeparam>
        /// <typeparam name="TResult">The type of the field or property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field or property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getMember" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getMember" /> is not a <see cref="MemberExpression" />
        /// </exception>
        public static MemberInfo GetMember<TDefiningType, TResult>(
            Expression<Func<TDefiningType, TResult>> getMember) {

            if (getMember == null) {
                throw new ArgumentNullException(nameof(getMember));
            }

            if (!(getMember.Body is MemberExpression memberExpression)) {
                throw new ArgumentException(
                    $"The body of the expression must be a {nameof(MethodCallExpression)}.",
                    nameof(getMember)
                );
            }

            return memberExpression.Member;
        }

        /// <summary>
        ///   Retrieves the <see cref="PropertyInfo"/> for an instance property based on an
        ///   expression that accesses it.
        /// </summary>
        /// <param name="getProperty">
        ///   An expression that retrieves the value of a property.
        /// </param>
        /// <typeparam name="TDefiningType">The type that defines the property.</typeparam>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getProperty" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getProperty" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getProperty" /> is not a property.
        /// </exception>
        public static PropertyInfo GetProperty<TDefiningType, TResult>(
            Expression<Func<TDefiningType, TResult>> getProperty) {

            if (getProperty == null) {
                throw new ArgumentNullException(nameof(getProperty));
            }

            MemberInfo memberInfo;

            try {
                memberInfo = GetMember(getProperty);
            } catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message, nameof(getProperty));
            }

            if (!(memberInfo is PropertyInfo propertyInfo)) {
                throw new ArgumentException(
                    $"The specified expression references a member that is not a property: " +
                    $"{memberInfo.Name} {memberInfo.GetType().Name}.",
                    nameof(getProperty)
                );
            }

            return propertyInfo;
        }

        /// <summary>
        ///   Retrieves the <see cref="FieldInfo"/> for an instance field based on an expression
        ///   that accesses it.
        /// </summary>
        /// <param name="getField">
        ///   An expression that retrieves the value of a field.
        /// </param>
        /// <typeparam name="TDefiningType">The type that defines the field.</typeparam>
        /// <typeparam name="TResult">The type of the field.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getField" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getField" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getField" /> is not a field.
        /// </exception>
        public static FieldInfo GetField<TDefiningType, TResult>(
            Expression<Func<TDefiningType, TResult>> getField) {

            if (getField == null) {
                throw new ArgumentNullException(nameof(getField));
            }

            MemberInfo memberInfo;
            try {
                memberInfo = GetMember(getField);
            } catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message, nameof(getField));
            }

            if (!(memberInfo is FieldInfo fieldInfo)) {
                throw new ArgumentException(
                    $"The specified expression references a member that is not a field: " +
                    $"{memberInfo.Name} {memberInfo.GetType().Name}.",
                    nameof(getField)
                );
            }

            return fieldInfo;
        }

        /// <summary>
        ///   Retrieves the <see cref="MemberInfo" /> for a static property or field based on an
        ///   expression that accesses it.
        /// </summary>
        /// <param name="getMember">
        ///   An expression that retrieves the value of a field or property.
        /// </param>
        /// <typeparam name="TResult">The type of the field or property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field or property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getMember" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getMember" /> is not a <see cref="MemberExpression" />
        /// </exception>
        public static MemberInfo GetMember<TResult>(
            Expression<Func<TResult>> getMember) {

            if (getMember == null) {
                throw new ArgumentNullException(nameof(getMember));
            }

            if (!(getMember.Body is MemberExpression memberExpression)) {
                throw new ArgumentException(
                    $"The body of the expression must be a {nameof(MemberExpression)}.",
                    nameof(getMember)
                );
            }

            return memberExpression.Member;
        }

        /// <summary>
        ///   Retrieves the <see cref="PropertyInfo"/> for a static property based on an expression
        ///   that accesses it.
        /// </summary>
        /// <param name="getProperty">
        ///   An expression that retrieves the value of a property.
        /// </param>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getProperty" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getProperty" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getProperty" /> is not a property.
        /// </exception>
        public static PropertyInfo GetProperty<TResult>(
            Expression<Func<TResult>> getProperty) {

            if (getProperty == null) {
                throw new ArgumentNullException(nameof(getProperty));
            }

            MemberInfo memberInfo;

            try {
                memberInfo = GetMember(getProperty);
            } catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message, nameof(getProperty));
            }

            if (!(memberInfo is PropertyInfo propertyInfo)) {
                throw new ArgumentException(
                    $"The specified expression references a member that is not a property: " +
                    $"{memberInfo.Name} {memberInfo.GetType().Name}.",
                    nameof(getProperty)
                );
            }

            return propertyInfo;
        }

        /// <summary>
        ///   Retrieves the <see cref="FieldInfo"/> for a static field based on an expression that
        ///   accesses it.
        /// </summary>
        /// <param name="getField">
        ///   An expression that retrieves the value of a field.
        /// </param>
        /// <typeparam name="TResult">The type of the field.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getField" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getField" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getField" /> is not a field.
        /// </exception>
        public static FieldInfo GetField<TResult>(
            Expression<Func<TResult>> getField) {

            if (getField == null) {
                throw new ArgumentNullException(nameof(getField));
            }

            MemberInfo memberInfo;
            try {
                memberInfo = GetMember(getField);
            } catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message, nameof(getField));
            }

            if (!(memberInfo is FieldInfo fieldInfo)) {
                throw new ArgumentException(
                    $"The specified expression references a member that is not a field: " +
                    $"{memberInfo.Name} {memberInfo.GetType().Name}.",
                    nameof(getField)
                );
            }

            return fieldInfo;
        }
    }

    /// <summary>
    ///   A generic class for retrieving the <see cref="MemberInfo" /> instances of instance members
    ///   using expressions.
    /// </summary>
    /// <remarks>
    ///   In some cases these methods are more convenient than the static methods on
    ///   <see cref="ReflectionHelper" /> becuse type inference can be used for member types, but
    ///   not for <typeparamref name="TDefiningType" />.
    /// </remarks>
    /// <typeparam name="TDefiningType">
    ///   The type that defines the member being retrieved.
    /// </typeparam>
    public static class ReflectionHelper<TDefiningType> {
        /// <summary>
        ///   Retrieves the <see cref="MemberInfo" /> for an instance property or field based on an
        ///   expression that accesses it.
        /// </summary>
        /// <seealso cref="ReflectionHelper.GetMember{TDefiningType,TResult}" />
        /// <param name="getMember">
        ///   An expression that retrieves the value of a field or property.
        /// </param>
        /// <typeparam name="TResult">The type of the field or property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field or property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getMember" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getMember" /> is not a <see cref="MemberExpression" />
        /// </exception>
        public static MemberInfo GetMember<TResult>(
            Expression<Func<TDefiningType, TResult>> getMember) {

            return ReflectionHelper.GetMember(getMember);
        }

        /// <summary>
        ///   Retrieves the <see cref="PropertyInfo"/> for an instance property based on an
        ///   expression that accesses it.
        /// </summary>
        /// <seealso cref="ReflectionHelper.GetProperty{TDefiningType,TResult}" />
        /// <param name="getProperty">
        ///   An expression that retrieves the value of a property.
        /// </param>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the property referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getProperty" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getProperty" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getProperty" /> is not a property.
        /// </exception>
        public static PropertyInfo GetProperty<TResult>(
            Expression<Func<TDefiningType, TResult>> getProperty) {

            return ReflectionHelper.GetProperty(getProperty);
        }

        /// <summary>
        ///   Retrieves the <see cref="FieldInfo"/> for an instance field based on an expression
        ///   that accesses it.
        /// </summary>
        /// <seealso cref="ReflectionHelper.GetField{TDefiningType,TResult}" />
        /// <param name="getField">
        ///   An expression that retrieves the value of a field.
        /// </param>
        /// <typeparam name="TResult">The type of the field.</typeparam>
        /// <returns>
        ///   The <see cref="MemberInfo" /> of the field referenced by the expression.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="getField" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The body of <paramref name="getField" /> is not a <see cref="MemberExpression" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The member referenced by <paramref name="getField" /> is not a field.
        /// </exception>
        public static FieldInfo GetField<TResult>(
            Expression<Func<TDefiningType, TResult>> getField) {

            return ReflectionHelper.GetField(getField);
        }
    }
}
