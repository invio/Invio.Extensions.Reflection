using System;
using System.Linq.Expressions;
using System.Reflection;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class ReflectionHelperTests {
        private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

        // == Error Handling ==

        #region ArgumentNullException tests

        [Fact]
        public void GetMethodInfo_ActionOf1Expression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetMethodFromExpression<Object>(null)
            );
            Assert.Equal("invokeMethod", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetMethodFromExpression(null)
            );
            Assert.Equal("invokeMethod", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_Action_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionOf1_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action<Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionOf2_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action<Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionOf3_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action<Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionOf4_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action<Object, Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_ActionOf5_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetActionMethod((Action<Object, Object, Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_Func_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_FuncOf1_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_FuncOf2_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_FuncOf3_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object, Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_FuncOf4_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object, Object, Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMethodInfo_FuncOf5_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetFuncMethod((Func<Object, Object, Object, Object, Object, Object>)null)
            );
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void GetMemberInfo_InstanceExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetMember((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GetPropertyInfo_InstanceExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetProperty((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GetFieldInfo_InstanceExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetField((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getField", ex.ParamName);
        }

        [Fact]
        public void GetMemberInfo_StaticExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetMember((Expression<Func<Object>>)null)
            );
            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GetPropertyInfo_StaticExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetProperty((Expression<Func<Object>>)null)
            );
            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GetFieldInfo_StaticExpression_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper.GetField((Expression<Func<Object>>)null)
            );
            Assert.Equal("getField", ex.ParamName);
        }


        [Fact]
        public void GenericReflectionHelper_GetMemberInfo_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper<Object>.GetMember((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GenericReflectionHelper_GetPropertyInfo_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper<Object>.GetProperty((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GenericReflectionHelper_GetFieldInfo_Null() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => ReflectionHelper<Object>.GetField((Expression<Func<Object, Object>>)null)
            );
            Assert.Equal("getField", ex.ParamName);
        }

        #endregion

        // Non-method Expressions

        [Fact]
        public void GetInstanceMethod_NonMethodExpression_ArgumentException() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetMethodFromExpression<Object>(
                    _ => new Object()
                )
            );

            Assert.Equal("invokeMethod", ex.ParamName);
        }

        [Fact]
        public void GetStaticMethod_NonMethodExpression_ArgumentException() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetMethodFromExpression(
                    () => new Object()
                )
            );

            Assert.Equal("invokeMethod", ex.ParamName);
        }

        // Non-member Expressions

        [Fact]
        public void GetStaticMember_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetMember(() => 4 * 5)
            );

            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GetStaticProperty_NonMember() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetProperty(() => 4 * 5)
            );

            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GetStaticField_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetField(() => 4 * 5)
            );

            Assert.Equal("getField", ex.ParamName);
        }

        [Fact]
        public void GetMember_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetMember<Object, Int32>(_ => 4 * 5)
            );

            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GetProperty_NonMember() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetProperty<Object, Int32>(_ => 4 * 5)
            );

            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GetField_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetField<Object, Int32>(_ => 4 * 5)
            );

            Assert.Equal("getField", ex.ParamName);
        }

        [Fact]
        public void GetMember_Constant_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetMember(() => TestPrivateConst)
            );

            Assert.Equal("getMember", ex.ParamName);
        }

        [Fact]
        public void GetField_Constant_NonMemberExpression() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetField(() => TestPrivateConst)
            );

            Assert.Equal("getField", ex.ParamName);
        }

        // == Incorrect member type ==

        [Fact]
        public void GetField_ReferencingProperty() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper<TestClass>.GetField(tc => tc.TestInstanceProperty)
            );

            Assert.Equal("getField", ex.ParamName);
        }

        [Fact]
        public void GetStaticField_ReferencingProperty() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetField(() => TestPrivateStaticProperty)
            );

            Assert.Equal("getField", ex.ParamName);
        }

        [Fact]
        public void GetProperty_ReferencingField() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper<TestClass>.GetProperty(tc => tc.TestInstanceField)
            );

            Assert.Equal("getProperty", ex.ParamName);
        }

        [Fact]
        public void GetStaticProperty_ReferencingField() {
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionHelper.GetProperty(() => TestPrivateStaticField)
            );

            Assert.Equal("getProperty", ex.ParamName);
        }

        // == Normal Operation ==

        [Fact]
        public void GetPrivateStaticField() {
            Assert.Equal(
                testPrivateStaticFieldInfo,
                ReflectionHelper.GetField(() => TestPrivateStaticField)
            );
        }

        [Fact]
        public void GetPrivateStaticReadonlyField() {
            Assert.Equal(
                testPrivateStaticReadonlyFieldInfo,
                ReflectionHelper.GetField(() => TestPrivateStaticReadonlyField)
            );
        }

        [Fact]
        public void GetPrivateStaticReadonlyProperty() {
            Assert.Equal(
                testPrivateStaticReadonlyPropertyInfo,
                ReflectionHelper.GetProperty(() => TestPrivateStaticReadonlyProperty)
            );
        }

        [Fact]
        public void GetPrivateStaticProperty() {
            Assert.Equal(
                testPrivateStaticPropertyInfo,
                ReflectionHelper.GetProperty(() => TestPrivateStaticProperty)
            );
        }

        [Fact]
        public void GetPrivateField() {
            Assert.Equal(
                testPrivateInstanceFieldInfo,
                ReflectionHelper.GetField(() => TestPrivateInstanceField)
            );
        }

        [Fact]
        public void GetPrivateReadonlyField() {
            Assert.Equal(
                testPrivateReadonlyInstanceFieldInfo,
                ReflectionHelper.GetField(() => TestPrivateReadonlyInstanceField)
            );
        }

        [Fact]
        public void GetPrivateReadonlyProperty() {
            Assert.Equal(
                testPrivateReadonlyInstancePropertyInfo,
                ReflectionHelper.GetProperty(() => TestPrivateReadonlyInstanceProperty)
            );
        }

        [Fact]
        public void GetPrivateProperty() {
            Assert.Equal(
                testPrivateInstancePropertyInfo,
                ReflectionHelper.GetProperty(() => TestPrivateInstanceProperty)
            );
        }

        [Fact]
        public void GetPrivateActionFromExpression() {
            Assert.Equal(
                testPrivateActionMethod,
                ReflectionHelper.GetMethodFromExpression<ReflectionHelperTests>(
                    inst => inst.TestPrivateAction()
                )
            );
        }

        [Fact]
        public void GetPrivateActionFromDelegate() {
            Assert.Equal(
                testPrivateActionMethod,
                ReflectionHelper.GetActionMethod(TestPrivateAction)
            );
        }

        [Fact]
        public void GetPrivateFuncFromExpression() {
            Assert.Equal(
                testPrivateFuncMethod,
                ReflectionHelper.GetMethodFromExpression<ReflectionHelperTests>(
                    inst => inst.TestPrivateFunc()
                )
            );
        }

        [Fact]
        public void GetPrivateFuncFromDelegate() {
            Assert.Equal(
                testPrivateFuncMethod,
                ReflectionHelper.GetFuncMethod(TestPrivateFunc)
            );
        }

        [Fact]
        public void GetPrivateStaticActionFromExpression() {
            Assert.Equal(
                testPrivateStaticActionMethod,
                ReflectionHelper.GetMethodFromExpression(
                    () => TestPrivateStaticAction()
                )
            );
        }

        [Fact]
        public void GetPrivateStaticActionFromDelegate() {
            Assert.Equal(
                testPrivateStaticActionMethod,
                ReflectionHelper.GetActionMethod(TestPrivateStaticAction)
            );
        }

        [Fact]
        public void GetPrivateStaticFuncFromExpression() {
            Assert.Equal(
                testPrivateStaticFuncMethod,
                ReflectionHelper.GetMethodFromExpression(
                    () => TestPrivateStaticFunc()
                )
            );
        }

        [Fact]
        public void GetPrivateStaticFuncFromDelegate() {
            Assert.Equal(
                testPrivateStaticFuncMethod,
                ReflectionHelper.GetFuncMethod(TestPrivateStaticFunc)
            );
        }

        [Fact]
        public void GetPrivateStaticActionOf1() {
            Assert.Equal(
                testPrivateStaticActionOf1Method,
                ReflectionHelper.GetActionMethod<String>(TestPrivateStaticActionOf1)
            );
        }

        [Fact]
        public void GetPrivateStaticFuncOf1() {
            Assert.Equal(
                testPrivateStaticFuncOf1Method,
                ReflectionHelper.GetFuncMethod<String, String>(TestPrivateStaticFuncOf1)
            );
        }

        [Fact]
        public void GetPrivateActionOf1() {
            Assert.Equal(
                testPrivateActionOf1Method,
                ReflectionHelper.GetActionMethod<String>(TestPrivateActionOf1)
            );
        }

        [Fact]
        public void GetPrivateFuncOf1() {
            Assert.Equal(
                testPrivateFuncOf1Method,
                ReflectionHelper.GetFuncMethod<String, String>(TestPrivateFuncOf1)
            );
        }

        [Fact]
        public void GetStaticField() {
            Assert.Equal(
                testStaticFieldInfo,
                ReflectionHelper.GetField(() => TestClass.TestStaticField)
            );
        }

        [Fact]
        public void GetStaticReadonlyField() {
            Assert.Equal(
                testStaticReadonlyFieldInfo,
                ReflectionHelper.GetField(() => TestClass.TestStaticReadonlyField)
            );
        }

        [Fact]
        public void GetStaticReadonlyProperty() {
            Assert.Equal(
                testStaticReadonlyPropertyInfo,
                ReflectionHelper.GetProperty(() => TestClass.TestStaticReadonlyProperty)
            );
        }

        [Fact]
        public void GetStaticProperty() {
            Assert.Equal(
                testStaticPropertyInfo,
                ReflectionHelper.GetProperty(() => TestClass.TestStaticProperty)
            );
        }

        [Fact]
        public void GetField() {
            Assert.Equal(
                testInstanceFieldInfo,
                ReflectionHelper<TestClass>.GetField(tc => tc.TestInstanceField)
            );
        }

        [Fact]
        public void GetReadonlyField() {
            Assert.Equal(
                testReadonlyInstanceFieldInfo,
                ReflectionHelper<TestClass>.GetField(tc => tc.TestReadonlyInstanceField)
            );
        }

        [Fact]
        public void GetReadonlyProperty() {
            Assert.Equal(
                testReadonlyInstancePropertyInfo,
                ReflectionHelper<TestClass>.GetProperty(tc => tc.TestReadonlyInstanceProperty)
            );
        }

        [Fact]
        public void GetProperty() {
            Assert.Equal(
                testInstancePropertyInfo,
                ReflectionHelper<TestClass>.GetProperty(tc => tc.TestInstanceProperty)
            );
        }

        [Fact]
        public void GetActionFromExpression() {
            Assert.Equal(
                testActionMethod,
                ReflectionHelper.GetMethodFromExpression<TestClass>(
                    inst => inst.TestAction()
                )
            );
        }

        [Fact]
        public void GetActionFromDelegate() {
            Assert.Equal(
                testActionMethod,
                ReflectionHelper.GetActionMethod(new TestClass().TestAction)
            );
        }

        [Fact]
        public void GetActionOf1() {
            Assert.Equal(
                testActionOf1Method,
                ReflectionHelper.GetActionMethod<String>(new TestClass().TestActionOf1)
            );
        }

        [Fact]
        public void GetStaticActionFromExpression() {
            Assert.Equal(
                testStaticActionMethod,
                ReflectionHelper.GetMethodFromExpression(
                    () => TestClass.TestStaticAction()
                )
            );
        }

        [Fact]
        public void GetStaticActionFromDelegate() {
            Assert.Equal(
                testStaticActionMethod,
                ReflectionHelper.GetActionMethod(TestClass.TestStaticAction)
            );
        }

        [Fact]
        public void GetStaticActionOf1() {
            Assert.Equal(
                testStaticActionOf1Method,
                ReflectionHelper.GetActionMethod<String>(TestClass.TestStaticActionOf1)
            );
        }

        [Fact]
        public void GetStaticActionOf2() {
            Assert.Equal(
                testStaticActionOf2Method,
                ReflectionHelper.GetActionMethod<String, Int32>(TestClass.TestStaticActionOf2)
            );
        }

        [Fact]
        public void GetStaticActionOf3() {
            Assert.Equal(
                testStaticActionOf3Method,
                ReflectionHelper.GetActionMethod<String, Int32, Double>(TestClass.TestStaticActionOf3)
            );
        }

        [Fact]
        public void GetStaticActionOf4() {
            Assert.Equal(
                testStaticActionOf4Method,
                ReflectionHelper.GetActionMethod<String, Int32, Double, DateTime>(
                    TestClass.TestStaticActionOf4
                )
            );
        }

        [Fact]
        public void GetStaticActionOf5() {
            Assert.Equal(
                testStaticActionOf5Method,
                ReflectionHelper.GetActionMethod<String, Int32, Double, DateTime, Uri>(
                    TestClass.TestStaticActionOf5
                )
            );
        }

        [Fact]
        public void GetFuncFromExpression() {
            Assert.Equal(
                testFuncMethod,
                ReflectionHelper.GetMethodFromExpression<TestClass>(
                    inst => inst.TestFunc()
                )
            );
        }

        [Fact]
        public void GetFuncFromDelegate() {
            Assert.Equal(
                testFuncMethod,
                ReflectionHelper.GetFuncMethod(new TestClass().TestFunc)
            );
        }

        [Fact]
        public void GetFuncOf1() {
            Assert.Equal(
                testFuncOf1Method,
                ReflectionHelper.GetFuncMethod<String, String>(new TestClass().TestFuncOf1)
            );
        }

        [Fact]
        public void GetStaticFuncFromExpression() {
            Assert.Equal(
                testStaticFuncMethod,
                ReflectionHelper.GetMethodFromExpression(
                    () => TestClass.TestStaticFunc()
                )
            );
        }

        [Fact]
        public void GetStaticFuncFromDelegate() {
            Assert.Equal(
                testStaticFuncMethod,
                ReflectionHelper.GetFuncMethod(TestClass.TestStaticFunc)
            );
        }

        [Fact]
        public void GetStaticFuncOf1() {
            Assert.Equal(
                testStaticFuncOf1Method,
                ReflectionHelper.GetFuncMethod<String, String>(TestClass.TestStaticFuncOf1)
            );
        }

        [Fact]
        public void GetStaticFuncOf2() {
            Assert.Equal(
                testStaticFuncOf2Method,
                ReflectionHelper.GetFuncMethod<String, Int32, String>(TestClass.TestStaticFuncOf2)
            );
        }

        [Fact]
        public void GetStaticFuncOf3() {
            Assert.Equal(
                testStaticFuncOf3Method,
                ReflectionHelper.GetFuncMethod<String, Int32, Double, String>(
                    TestClass.TestStaticFuncOf3
                )
            );
        }

        [Fact]
        public void GetStaticFuncOf4() {
            Assert.Equal(
                testStaticFuncOf4Method,
                ReflectionHelper.GetFuncMethod<String, Int32, Double, DateTime, String>(
                    TestClass.TestStaticFuncOf4
                )
            );
        }

        [Fact]
        public void GetStaticFuncOf5() {
            Assert.Equal(
                testStaticFuncOf5Method,
                ReflectionHelper.GetFuncMethod<String, Int32, Double, DateTime, Uri, String>(
                    TestClass.TestStaticFuncOf5
                )
            );
        }

        [Fact]
        public void GetAmbgious_Action() {
            Assert.Equal(
                testOverloadedMethod_Action,
                ReflectionHelper.GetActionMethod(TestClass.Overloaded)
            );
        }

        [Fact]
        public void GetAmbgious_FuncOfString() {
            Assert.Equal(
                testOverloadedMethod_FuncOfString,
                ReflectionHelper.GetFuncMethod<String, String>(TestClass.Overloaded)
            );
        }

        [Fact]
        public void GetAmbgious_ActionOfStringString() {
            Assert.Equal(
                testOverloadedMethod_ActionOfStringString,
                ReflectionHelper.GetActionMethod<String, String>(TestClass.Overloaded)
            );
        }

        [Fact]
        public void GetAmbgious_ActionOfStringInt() {
            Assert.Equal(
                testOverloadedMethod_ActionOfStringInt32,
                ReflectionHelper.GetActionMethod<String, Int32>(TestClass.Overloaded)
            );
        }

        // Private Fields and Properties

        private static readonly FieldInfo testPrivateStaticFieldInfo =
            typeof(ReflectionHelperTests).GetField(nameof(TestPrivateStaticField), PrivateStatic);
        private static readonly FieldInfo testPrivateStaticReadonlyFieldInfo =
            typeof(ReflectionHelperTests).GetField(nameof(TestPrivateStaticReadonlyField), PrivateStatic);

        private static readonly PropertyInfo testPrivateStaticReadonlyPropertyInfo =
            typeof(ReflectionHelperTests).GetProperty(nameof(TestPrivateStaticReadonlyProperty), PrivateStatic);
        private static readonly PropertyInfo testPrivateStaticPropertyInfo =
            typeof(ReflectionHelperTests).GetProperty(nameof(TestPrivateStaticProperty), PrivateStatic);

        private static readonly FieldInfo testPrivateInstanceFieldInfo =
            typeof(ReflectionHelperTests).GetField(nameof(TestPrivateInstanceField), Private);
        private static readonly FieldInfo testPrivateReadonlyInstanceFieldInfo =
            typeof(ReflectionHelperTests).GetField(nameof(TestPrivateReadonlyInstanceField), Private);

        private static readonly PropertyInfo testPrivateReadonlyInstancePropertyInfo =
            typeof(ReflectionHelperTests).GetProperty(nameof(TestPrivateReadonlyInstanceProperty), Private);
        private static readonly PropertyInfo testPrivateInstancePropertyInfo =
            typeof(ReflectionHelperTests).GetProperty(nameof(TestPrivateInstanceProperty), Private);

        // Private Functions

        private static readonly MethodInfo testPrivateActionMethod =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateAction), Private);
        private static readonly MethodInfo testPrivateActionOf1Method =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateActionOf1), Private);

        private static readonly MethodInfo testPrivateStaticActionMethod =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateStaticAction), PrivateStatic);
        private static readonly MethodInfo testPrivateStaticActionOf1Method =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateStaticActionOf1), PrivateStatic);

        private static readonly MethodInfo testPrivateFuncMethod =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateFunc), Private);
        private static readonly MethodInfo testPrivateFuncOf1Method =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateFuncOf1), Private);

        private static readonly MethodInfo testPrivateStaticFuncMethod =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateStaticFunc), PrivateStatic);
        private static readonly MethodInfo testPrivateStaticFuncOf1Method =
            typeof(ReflectionHelperTests).GetMethod(nameof(TestPrivateStaticFuncOf1), PrivateStatic);

        // Public Fields and Properties

        private static readonly FieldInfo testStaticFieldInfo =
            typeof(TestClass).GetField(nameof(TestClass.TestStaticField), PublicStatic);
        private static readonly FieldInfo testStaticReadonlyFieldInfo =
            typeof(TestClass).GetField(nameof(TestClass.TestStaticReadonlyField), PublicStatic);
        private static readonly PropertyInfo testStaticReadonlyPropertyInfo =
            typeof(TestClass).GetProperty(nameof(TestClass.TestStaticReadonlyProperty), PublicStatic);
        private static readonly PropertyInfo testStaticPropertyInfo =
            typeof(TestClass).GetProperty(nameof(TestClass.TestStaticProperty), PublicStatic);

        private static readonly FieldInfo testInstanceFieldInfo =
            typeof(TestClass).GetField(nameof(TestClass.TestInstanceField), Public);
        private static readonly FieldInfo testReadonlyInstanceFieldInfo =
            typeof(TestClass).GetField(nameof(TestClass.TestReadonlyInstanceField), Public);
        private static readonly PropertyInfo testReadonlyInstancePropertyInfo =
            typeof(TestClass).GetProperty(nameof(TestClass.TestReadonlyInstanceProperty), Public);
        private static readonly PropertyInfo testInstancePropertyInfo =
            typeof(TestClass).GetProperty(nameof(TestClass.TestInstanceProperty), Public);

        // Public Methods

        private static readonly MethodInfo testActionMethod =
            typeof(TestClass).GetMethod(nameof(TestClass.TestAction), Public);
        private static readonly MethodInfo testActionOf1Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestActionOf1), Public);

        private static readonly MethodInfo testStaticActionMethod =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticAction), PublicStatic);
        private static readonly MethodInfo testStaticActionOf1Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticActionOf1), PublicStatic);
        private static readonly MethodInfo testStaticActionOf2Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticActionOf2), PublicStatic);
        private static readonly MethodInfo testStaticActionOf3Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticActionOf3), PublicStatic);
        private static readonly MethodInfo testStaticActionOf4Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticActionOf4), PublicStatic);
        private static readonly MethodInfo testStaticActionOf5Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticActionOf5), PublicStatic);

        private static readonly MethodInfo testFuncMethod =
            typeof(TestClass).GetMethod(nameof(TestClass.TestFunc), Public);
        private static readonly MethodInfo testFuncOf1Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestFuncOf1), Public);

        private static readonly MethodInfo testStaticFuncMethod =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFunc), PublicStatic);
        private static readonly MethodInfo testStaticFuncOf1Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFuncOf1), PublicStatic);
        private static readonly MethodInfo testStaticFuncOf2Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFuncOf2), PublicStatic);
        private static readonly MethodInfo testStaticFuncOf3Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFuncOf3), PublicStatic);
        private static readonly MethodInfo testStaticFuncOf4Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFuncOf4), PublicStatic);
        private static readonly MethodInfo testStaticFuncOf5Method =
            typeof(TestClass).GetMethod(nameof(TestClass.TestStaticFuncOf5), PublicStatic);

        // Overloaded method overloads

        private static readonly MethodInfo testOverloadedMethod_Action =
            typeof(TestClass).GetMethod(nameof(TestClass.Overloaded), new Type[0]);
        private static readonly MethodInfo testOverloadedMethod_FuncOfString =
            typeof(TestClass).GetMethod(nameof(TestClass.Overloaded), new[] { typeof(String) });
        private static readonly MethodInfo testOverloadedMethod_ActionOfStringString =
            typeof(TestClass).GetMethod(
                nameof(TestClass.Overloaded),
                new[] { typeof(String), typeof(String) }
            );
        private static readonly MethodInfo testOverloadedMethod_ActionOfStringInt32 =
            typeof(TestClass).GetMethod(
                nameof(TestClass.Overloaded),
                new[] { typeof(String), typeof(Int32) }
            );

        private const String TestPrivateConst = "TestPrivateConst";
        private static String TestPrivateStaticField = "TestPrivateReadonlyField";
        private static readonly String TestPrivateStaticReadonlyField = "TestPrivateReadonlyField";
        private static String TestPrivateStaticReadonlyProperty { get; } = "TestPrivateGetOnlyField";
        private static String TestPrivateStaticProperty { get; set; } = "TestPrivateGetSetProperty";

        private String TestPrivateInstanceField = "TestPrivateInstanceField";
        private readonly String TestPrivateReadonlyInstanceField = "TestPrivateReadonlyInstanceField";
        private String TestPrivateReadonlyInstanceProperty { get; } = "TestPrivateReadonlyInstanceProperty";
        private String TestPrivateInstanceProperty { get; set; } = "TestPrivateInstanceProperty";

        private void TestPrivateAction() {}
        private void TestPrivateActionOf1(String t1) {}

        private static void TestPrivateStaticAction() {}
        private static void TestPrivateStaticActionOf1(String t1) {}

        private String TestPrivateFunc() => "TestPrivate";
        private String TestPrivateFuncOf1(String t1) => "TestPrivate";

        private static String TestPrivateStaticFunc() => "TestPrivate";
        private static String TestPrivateStaticFuncOf1(String t1) => "TestPrivate";

        private class TestClass {
            public static String TestStaticField = "TestReadonlyField";
            public static readonly String TestStaticReadonlyField = "TestReadonlyField";
            public static String TestStaticReadonlyProperty { get; } = "TestGetOnlyField";
            public static String TestStaticProperty { get; set; } = "TestGetSetProperty";

            public String TestInstanceField = "TestInstanceField";
            public readonly String TestReadonlyInstanceField = "TestReadonlyInstanceField";
            public String TestReadonlyInstanceProperty { get; } = "TestReadonlyInstanceProperty";
            public String TestInstanceProperty { get; set; } = "TestInstanceProperty";

            public void TestAction() {}
            public void TestActionOf1(String t1) {}

            public static void TestStaticAction() {}
            public static void TestStaticActionOf1(String t1) {}
            public static void TestStaticActionOf2(String t1, Int32 t2) {}
            public static void TestStaticActionOf3(String t1, Int32 t2, Double t3) {}
            public static void TestStaticActionOf4(String t1, Int32 t2, Double t3, DateTime t4) {}
            public static void TestStaticActionOf5(String t1, Int32 t2, Double t3, DateTime t4, Uri t5) {}

            public String TestFunc() => "Test";
            public String TestFuncOf1(String t1) => "Test";

            public static String TestStaticFunc() => "Test";
            public static String TestStaticFuncOf1(String t1) => "Test";
            public static String TestStaticFuncOf2(String t1, Int32 t2) => "Test";
            public static String TestStaticFuncOf3(String t1, Int32 t2, Double t3) => "Test";
            public static String TestStaticFuncOf4(String t1, Int32 t2, Double t3, DateTime t4) => "Test";
            public static String TestStaticFuncOf5(String t1, Int32 t2, Double t3, DateTime t4, Uri t5) => "Test";

            public static void Overloaded() {}
            public static String Overloaded(String t1) => "Test";
            public static void Overloaded(String t1, String t2) {}
            public static void Overloaded(String t1, Int32 t2) {}
        }
    }
}
