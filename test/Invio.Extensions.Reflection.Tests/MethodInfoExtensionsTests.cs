using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Peddler;
using Xunit;

using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class MethodInfoExtensionsTests {
        public static TheoryData ClassUnderTestCreateFuncs {
            get {
                var theoryData = new TheoryData<String, Action<MethodInfo>>();
                var actions = MakeCreateFuncActions<ClassUnderTest>();
                foreach (var action in actions) {
                    theoryData.Add(action.Key, action.Value);
                }

                return theoryData;
            }
        }
        public static TheoryData InvalidDeclaringTypeCreateFuncs {
            get {
                var theoryData = new TheoryData<String, Action<MethodInfo>>();
                var actions = MakeCreateFuncActions<MethodInfoExtensionsTests>();
                foreach (var action in actions) {
                    theoryData.Add(action.Key, action.Value);
                }

                return theoryData;
            }
        }

        public static TheoryData ArgumentCountCreateFuncs {
            get {
                var theoryData = new TheoryData<Int32, Int32, String, Action<MethodInfo>>();
                var actions = MakeCreateFuncActions<ClassUnderTest>();
                for (var i = 0; i < actions.Count; i++) {
                    var action = actions.ElementAt(i);
                    theoryData.Add(i, actions.Count, action.Key, action.Value);
                }

                return theoryData;
            }
        }

        private static IDictionary<String, Action<MethodInfo>> MakeCreateFuncActions<T>() where T : class {
            return new Dictionary<String, Action<MethodInfo>> {
                { "Func0", m => { m.CreateFunc0<T>(); } },
                { "Func1", m => { m.CreateFunc1<T>(); } },
                { "Func2", m => { m.CreateFunc2<T>(); } },
                { "Func3", m => { m.CreateFunc3<T>(); } },
                { "Func4", m => { m.CreateFunc4<T>(); } },
                { "Func5", m => { m.CreateFunc5<T>(); } },
                { "Func6", m => { m.CreateFunc6<T>(); } },
                { "Func7", m => { m.CreateFunc7<T>(); } },
                { "Func8", m => { m.CreateFunc8<T>(); } },
                { "Func9", m => { m.CreateFunc9<T>(); } }
            };
        }

        [Fact]
        public void MethodInfoDelegateFunc0_WrongReturnType() {
            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentException>(
                () => func0MethodInfo.CreateFunc0<ClassUnderTest, double>()
            );
        }

        [Theory]
        [MemberData(nameof(ClassUnderTestCreateFuncs))]
         public void CreateMethodInfoDelegate(string functionName, Action<MethodInfo> createFunc) {
             var methodInfo = sutType.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public);
             createFunc(methodInfo);
         }

        [Theory]
        [MemberData(nameof(InvalidDeclaringTypeCreateFuncs))]
        public void MethodInfoDelegate_WrongDeclaringType(String functionName, Action<MethodInfo> createFunc) {
            var methodInfo = sutType.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentException>(
                () => createFunc(methodInfo)
            );
        }

        [Theory]
        [MemberData(nameof(ClassUnderTestCreateFuncs))]
        public void MethodInfoDelegate_Action_NotSupported(String functionName, Action<MethodInfo> createFunc) {
            var methodInfo = sutType.GetMethod("incrementModified", BindingFlags.Instance | BindingFlags.Public);

            Assert.Throws<ArgumentException>(
                () => createFunc(methodInfo)
            );
        }

        [Theory]
        [MemberData(nameof(ArgumentCountCreateFuncs))]
        public void MethodInfoDelegate_ArgumentCount(
            int argumentCount,
            int totalFunctions,
            String functionName,
            Action<MethodInfo> createFunc) {
            var argumentCountGenerator = new Int32Generator(0, totalFunctions);
            var selectedFunctionName =
                ClassUnderTest.FunctionPrefix + argumentCountGenerator.NextDistinct(argumentCount);
            var methodInfo = sutType.GetMethod(selectedFunctionName, BindingFlags.Instance | BindingFlags.Public);

            var ex = Record.Exception(() => createFunc(methodInfo));
            Assert.NotNull(ex);
            Assert.Equal(typeof(ArgumentException), ex.GetType());
            Assert.Contains($"with {argumentCount} parameters", ex.Message);
        }

        public static TheoryData WrongArgumentCountCreateActionData {
            get {
                var incrementMethodName = "incrementModified";
                var addMethodName = "addToModified";

                return new TheoryData<Int32, String, Action<MethodInfo>> {
                    { 0, addMethodName, m => { m.CreateAction0<ClassUnderTest>(); } },
                    { 0, addMethodName, m => { m.CreateAction0(); } },
                    { 1, incrementMethodName, m => { m.CreateAction1<ClassUnderTest, int>(); } },
                    { 1, incrementMethodName, m => { m.CreateAction1<ClassUnderTest>(); } },
                    { 1, incrementMethodName, m => { m.CreateAction1(); } }
                };
            }
        }

        [Theory]
        [MemberData(nameof(WrongArgumentCountCreateActionData))]
        public void MethodInfoDelegateAction0_WrongArgumentCount(
            int expectedArgCount,
            String methodName,
            Action<MethodInfo> createAction) {
            var methodInfo = sutType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            var ex = Record.Exception(() => createAction(methodInfo));
            Assert.NotNull(ex);
            Assert.Equal(typeof(ArgumentException), ex.GetType());
            Assert.Contains($"with {expectedArgCount} parameters", ex.Message);
        }

        [Fact]
        public void MethodInfoDelegateAction1_MismatchParameterType() {
            var methodInfo = sutType.GetMethod("addToModified", BindingFlags.Instance | BindingFlags.Public);

            var ex = Record.Exception(() => methodInfo.CreateAction1<ClassUnderTest, double>());
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Contains($"exactly match the generic type parameter", ex.Message);
        }

        [Fact]
        public void IsImplementationOf_ArgNull_Checks() {
            MethodInfo nullMI = null;
            var methodInfo = isutType.GetMethod("interfaceAction", BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentNullException>(
                () => nullMI.IsImplementationOf(methodInfo)
            );
            Assert.Throws<ArgumentNullException>(
                () => methodInfo.IsImplementationOf(interfaceMethod: null)
            );
        }

        [Fact]
        public void IsImplementationOf_InterfaceSameMethod() {
            var methodInfo = isutType.GetMethod("interfaceAction", BindingFlags.Instance | BindingFlags.Public);
            var interfaceMethodInfo = isutType.GetMethod("interfaceAction");

            Assert.True(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_ClassSameMethod() {
            var methodInfo = sutType.GetMethod("interfaceAction", BindingFlags.Instance | BindingFlags.Public);
            var classMethodInfo = sutType.GetMethod("interfaceAction");

            Assert.True(methodInfo.IsImplementationOf(classMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_Class_DirectImplementation() {
            var methodInfo = sutType.GetMethod("interfaceAction");
            var interfaceMethodInfo = isutType.GetMethod("interfaceAction");

            Assert.True(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_Class_DirectImplementation2() {
            var methodInfo = sutType.GetMethod("baseInterfaceAction");
            var interfaceMethodInfo = isutType.GetMethod("baseInterfaceAction");

            Assert.True(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_Class_BaseImplementation() {
            var methodInfo = sutType.GetMethod("baseInterfaceAction");
            var interfaceMethodInfo = ibaseType.GetMethod("baseInterfaceAction");

            Assert.True(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_MismatchedInterfaces() {
            var methodInfo = iwrongType.GetMethod("interfaceAction");
            var interfaceMethodInfo = isutType.GetMethod("interfaceAction");

            Assert.False(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_Class_WrongInterface() {
            var methodInfo = sutType.GetMethod("interfaceAction");
            var interfaceMethodInfo = iwrongType.GetMethod("interfaceAction");

            Assert.False(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_InterfaceInheritence_NoImplementation() {
            var methodInfo = isutType.GetMethod("baseInterfaceAction");
            var interfaceMethodInfo = ibaseType.GetMethod("baseInterfaceAction");

            Assert.False(methodInfo.IsImplementationOf(interfaceMethodInfo));
        }

        [Fact]
        public void IsImplementationOf_ArgsBackwards() {
            var methodInfo = isutType.GetMethod("interfaceAction");
            var classMethodInfo = sutType.GetMethod("interfaceAction");

            Assert.False(methodInfo.IsImplementationOf(classMethodInfo));
        }

        private static readonly Type isutType = typeof(IClassUnderTest);
        interface IClassUnderTest : IBaseClassUnderTest {
            void interfaceAction();
            new void baseInterfaceAction();
        }

        private static readonly Type ibaseType = typeof(IBaseClassUnderTest);
        interface IBaseClassUnderTest {
            void baseInterfaceAction();
        }

        private static readonly Type iwrongType = typeof(IWrongClassInterface);
        interface IWrongClassInterface {
            void interfaceAction();
        }

        private static readonly Type sutType = typeof(ClassUnderTest);
        class ClassUnderTest : IClassUnderTest {
            public const String FunctionPrefix = "Func";

            private readonly int modifier;
            public int modified;

            public ClassUnderTest(int modifier) {
                this.modifier = modifier;
                this.modified = modifier;
            }

            public void interfaceAction() { }
            public void baseInterfaceAction() { }

            public void incrementModified() {
                modified++;
            }

            public void addToModified(int add) {
                modified += add;
            }

            public int Func0() {
                return modifier;
            }

            public int Func1(int a) {
                return modifier * a;
            }

            public int Func2(int a, int b) {
                return modifier + a + b;
            }

            public int Func3(int a, int b, int c) {
                return modifier + a - b + c;
            }

            public int Func4(int a, int b, int c, int d) {
                return modifier + a * b - c + d;
            }

            public int Func5(int a, int b, int c, int d, int e) {
                return modifier + a + b - c + d / e;
            }

            public int Func6(int a, int b, int c, int d, int e, int f) {
                return modifier + a * b + c / d + e - f;
            }

            public int Func7(int a, int b, int c, int d, int e, int f, int g) {
                return modifier + a + b * c + d + e - f * g;
            }

            public int Func8(int a, int b, int c, int d, int e, int f, int g, int h) {
                return modifier + a * b + c / d - e * f / g - h;
            }

            public int Func9(int a, int b, int c, int d, int e, int f, int g, int h, int i) {
                return modifier * a + b / c * d - e * f - g / h + i;
            }
        }

        public static IEnumerable<object[]> AssignableBaseTypeCases {
            get {
                return new List<object[]> {
                    new object[] {
                        nameof(Fake.Func0),
                        new FuncTestCase<Func<IFake, int>>(
                            method => method.CreateFunc0<IFake, int>(),
                            (fake, func) => func(fake)
                        ),
                        0
                    },
                    new object[] { nameof(Fake.Func0), TypedTestCases.Func0, 0 },
                    new object[] { nameof(Fake.Func0), TestCases.Func0, 0 },
                    new object[] { nameof(Fake.Func1), TypedTestCases.Func1, 1 },
                    new object[] { nameof(Fake.Func1), TestCases.Func1, 1 },
                    new object[] { nameof(Fake.Func2), TypedTestCases.Func2, 2 },
                    new object[] { nameof(Fake.Func2), TestCases.Func2, 2 },
                    new object[] { nameof(Fake.Func3), TypedTestCases.Func3, 3 },
                    new object[] { nameof(Fake.Func3), TestCases.Func3, 3 },
                    new object[] { nameof(Fake.Func4), TypedTestCases.Func4, 4 },
                    new object[] { nameof(Fake.Func4), TestCases.Func4, 4 },
                    new object[] { nameof(Fake.Func5), TypedTestCases.Func5, 5 },
                    new object[] { nameof(Fake.Func5), TestCases.Func5, 5 },
                    new object[] { nameof(Fake.Func6), TypedTestCases.Func6, 6 },
                    new object[] { nameof(Fake.Func6), TestCases.Func6, 6 },
                    new object[] { nameof(Fake.Func7), TypedTestCases.Func7, 7 },
                    new object[] { nameof(Fake.Func7), TestCases.Func7, 7 },
                    new object[] { nameof(Fake.Func8), TypedTestCases.Func8, 8 },
                    new object[] { nameof(Fake.Func8), TestCases.Func8, 8 },
                    new object[] { nameof(Fake.Func9), TypedTestCases.Func9, 9 },
                    new object[] { nameof(Fake.Func9), TestCases.Func9, 9 },
                    new object[] { nameof(Fake.Action0), TypedTestCases.Action0, 0 },
                    new object[] { nameof(Fake.Action0), TestCases.Action0, 0 },
                    new object[] {
                        nameof(Fake.Action1),
                        new ActionTestCase<Action<IFake, int>>(
                            method => method.CreateAction1<IFake, int>(),
                            (fake, action) => action(fake, 1)
                        ),
                        1
                    },
                    new object[] { nameof(Fake.Action1), TypedTestCases.Action1, 1 },
                    new object[] { nameof(Fake.Action1), TestCases.Action1, 1 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AssignableBaseTypeCases))]
        public void CreateDelegate_ExactBaseType<TAction>(
            String methodName,
            ITestCase<TAction> test,
            int expectedValue) {

            // Arrange

            var fake = new Fake();
            var method = typeof(IFake).GetMethod(methodName);

            // Act

            var methodDelegate = test.CreateDelegate(method);
            var actualValue = test.InvokeDelegate(fake, methodDelegate);

            // Assert

            Assert.Equal(expectedValue, actualValue);
        }

        public static IEnumerable<object[]> ArgumentNullCases {
            get {
                return new List<object[]> {
                    new object[] { ToFunc(m => m.CreateFunc0<Fake, int>()) },
                    new object[] { ToFunc(m => m.CreateFunc0<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc1<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc1()) },
                    new object[] { ToFunc(m => m.CreateFunc2<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc2()) },
                    new object[] { ToFunc(m => m.CreateFunc3<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc3()) },
                    new object[] { ToFunc(m => m.CreateFunc4<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc4()) },
                    new object[] { ToFunc(m => m.CreateFunc5<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc5()) },
                    new object[] { ToFunc(m => m.CreateFunc6<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc6()) },
                    new object[] { ToFunc(m => m.CreateFunc7<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc7()) },
                    new object[] { ToFunc(m => m.CreateFunc8<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc8()) },
                    new object[] { ToFunc(m => m.CreateFunc9<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc9()) },
                    new object[] { ToFunc(m => m.CreateAction0()) },
                    new object[] { ToFunc(m => m.CreateAction0<Fake>()) },
                    new object[] { ToFunc(m => m.CreateAction1()) },
                    new object[] { ToFunc(m => m.CreateAction1<Fake>()) },
                    new object[] { ToFunc(m => m.CreateAction1<Fake, int>()) }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ArgumentNullCases))]
        public void CreateDelegate_NullMethodInfo(Func<MethodInfo, object> createDelegate) {

            // Arrange

            MethodInfo method = null;

            // Act

            var exception = Record.Exception(
                () => createDelegate(method)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        public static class TypedTestCases {

            public static ITestCase<Func<IFake, object>> Func0 =
                new FuncTestCase<Func<IFake, object>>(
                    method => method.CreateFunc0<IFake>(),
                    (fake, func) => func(fake)
                );

            public static ITestCase<Func<IFake, object, object>> Func1 =
                new FuncTestCase<Func<IFake, object, object>>(
                    method => method.CreateFunc1<IFake>(),
                    (fake, func) => func(fake, 1)
                );

            public static ITestCase<Func<IFake, object, object, object>> Func2 =
                new FuncTestCase<Func<IFake, object, object, object>>(
                    method => method.CreateFunc2<IFake>(),
                    (fake, func) => func(fake, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object>> Func3 =
                new FuncTestCase<Func<IFake, object, object, object, object>>(
                    method => method.CreateFunc3<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1)
                );

            public static ITestCase<Func<IFake,  object, object, object, object, object>> Func4 =
                new FuncTestCase<Func<IFake, object, object, object, object, object>>(
                    method => method.CreateFunc4<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object, object, object>> Func5 =
                new FuncTestCase<Func<IFake, object, object, object, object, object, object>>(
                    method => method.CreateFunc5<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object, object, object, object>> Func6 =
                new FuncTestCase<Func<IFake, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc6<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object, object, object, object, object>> Func7 =
                new FuncTestCase<Func<IFake, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc7<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object, object, object, object, object, object>> Func8 =
                new FuncTestCase<Func<IFake, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc8<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<IFake, object, object, object, object, object, object, object, object, object, object>> Func9 =
                new FuncTestCase<Func<IFake, object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc9<IFake>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Action<IFake>> Action0 =
                new ActionTestCase<Action<IFake>>(
                    method => method.CreateAction0<IFake>(),
                    (fake, action) => action(fake)
                );

            public static ITestCase<Action<IFake, object>> Action1 =
                new ActionTestCase<Action<IFake, object>>(
                    method => method.CreateAction1<IFake>(),
                    (fake, action) => action(fake, 1)
                );

        }

        public static class TestCases {

            public static ITestCase<Func<object, object>> Func0 =
                new FuncTestCase<Func<object, object>>(
                    method => method.CreateFunc0(),
                    (fake, func) => func(fake)
                );

            public static ITestCase<Func<object, object, object>> Func1 =
                new FuncTestCase<Func<object, object, object>>(
                    method => method.CreateFunc1(),
                    (fake, func) => func(fake, 1)
                );

            public static ITestCase<Func<object, object, object, object>> Func2 =
                new FuncTestCase<Func<object, object, object, object>>(
                    method => method.CreateFunc2(),
                    (fake, func) => func(fake, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object>> Func3 =
                new FuncTestCase<Func<object, object, object, object, object>>(
                    method => method.CreateFunc3(),
                    (fake, func) => func(fake, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object>> Func4 =
                new FuncTestCase<Func<object, object, object, object, object, object>>(
                    method => method.CreateFunc4(),
                    (fake, func) => func(fake, 1, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object, object>> Func5 =
                new FuncTestCase<Func<object, object, object, object, object, object, object>>(
                    method => method.CreateFunc5(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object, object, object>> Func6 =
                new FuncTestCase<Func<object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc6(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object, object, object, object>> Func7 =
                new FuncTestCase<Func<object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc7(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object, object, object, object, object>> Func8 =
                new FuncTestCase<Func<object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc8(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Func<object, object, object, object, object, object, object, object, object, object, object>> Func9 =
                new FuncTestCase<Func<object, object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc9(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<Action<object>> Action0 =
                new ActionTestCase<Action<object>>(
                    method => method.CreateAction0(),
                    (fake, action) => action(fake)
                );

            public static ITestCase<Action<object, object>> Action1 =
                new ActionTestCase<Action<object, object>>(
                    method => method.CreateAction1(),
                    (fake, action) => action(fake, 1)
                );

        }

        public interface ITestCase<TFunc> {

            TFunc CreateDelegate(MethodInfo method);
            int InvokeDelegate(IFake fake, TFunc func);

        }

        private class FuncTestCase<TFunc> : ITestCase<TFunc> {

            private Func<MethodInfo, TFunc> createDelegate { get; }
            private Func<IFake, TFunc, object> invokeDelegate { get; }

            public FuncTestCase(
                Func<MethodInfo, TFunc> createDelegate,
                Func<IFake, TFunc, object> invokeDelegate) {

                this.createDelegate = createDelegate;
                this.invokeDelegate = invokeDelegate;
            }

            public TFunc CreateDelegate(MethodInfo method) {
                return this.createDelegate(method);
            }

            public int InvokeDelegate(IFake fake, TFunc action) {
                return (int)this.invokeDelegate(fake, action);
            }

        }

        private class ActionTestCase<TAction> : ITestCase<TAction> {

            private Func<MethodInfo, TAction> createDelegate { get; }
            private Action<IFake, TAction> invokeDelegate { get; }

            public ActionTestCase(
                Func<MethodInfo, TAction> createDelegate,
                Action<IFake, TAction> invokeDelegate) {

                this.createDelegate = createDelegate;
                this.invokeDelegate = invokeDelegate;
            }

            public TAction CreateDelegate(MethodInfo method) {
                return this.createDelegate(method);
            }

            public int InvokeDelegate(IFake fake, TAction action) {
                this.invokeDelegate(fake, action);

                return fake.Value;
            }

        }

        public interface IFake {

            int Value { get; }
            int Func0();
            int Func1(int a);
            int Func2(int a, int b);
            int Func3(int a, int b, int c);
            int Func4(int a, int b, int c, int d);
            int Func5(int a, int b, int c, int d, int e);
            int Func6(int a, int b, int c, int d, int e, int f);
            int Func7(int a, int b, int c, int d, int e, int f, int g);
            int Func8(int a, int b, int c, int d, int e, int f, int g, int h);
            int Func9(int a, int b, int c, int d, int e, int f, int g, int h, int i);
            void Action0();
            void Action1(int a);

        }

        public class Fake : IFake {

            public int Value { get; private set; }

            public int Func0() {
                return 0;
            }

            public int Func1(int a) {
                return a;
            }

            public int Func2(int a, int b) {
                return a + b;
            }

            public int Func3(int a, int b, int c) {
                return a + b + c;
            }

            public int Func4(int a, int b, int c, int d) {
                return a + b + c + d;
            }

            public int Func5(int a, int b, int c, int d, int e) {
                return a + b + c + d + e;
            }

            public int Func6(int a, int b, int c, int d, int e, int f) {
                return a + b + c + d + e + f;
            }

            public int Func7(int a, int b, int c, int d, int e, int f, int g) {
                return a + b + c + d + e + f + g;
            }

            public int Func8(int a, int b, int c, int d, int e, int f, int g, int h) {
                return a + b + c + d + e + f + g + h;
            }

            public int Func9(
                int a, int b, int c, int d, int e, int f, int g, int h, int i) {

                return a + b + c + d + e + f + g + h + i;
            }

            public void Action0() {
                this.Value = 0;
            }

            public void Action1(int a) {
                this.Value = a;
            }

        }

        private static Func<MethodInfo, object> ToFunc<T>(Func<MethodInfo, T> createDelegate) {
            return new Func<MethodInfo, object>(method => (object)createDelegate(method));
        }

        public static IEnumerable<object[]> StaticMethodCases {
            get {
                return new List<object[]> {
                    new object[] {
                        nameof(StaticFake.Func0),
                        ToFunc(m => m.CreateFunc0<StaticFake, int>())
                    },
                    new object[] {
                        nameof(StaticFake.Func0),
                        ToFunc(m => m.CreateFunc0<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func1),
                        ToFunc(m => m.CreateFunc1<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func1),
                        ToFunc(m => m.CreateFunc1())
                    },
                    new object[] {
                        nameof(StaticFake.Func2),
                        ToFunc(m => m.CreateFunc2<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func2),
                        ToFunc(m => m.CreateFunc2())
                    },
                    new object[] {
                        nameof(StaticFake.Func3),
                        ToFunc(m => m.CreateFunc3<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func3),
                        ToFunc(m => m.CreateFunc3())
                    },
                    new object[] {
                        nameof(StaticFake.Func4),
                        ToFunc(m => m.CreateFunc4<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func4),
                        ToFunc(m => m.CreateFunc4())
                    },
                    new object[] {
                        nameof(StaticFake.Func5),
                        ToFunc(m => m.CreateFunc5<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func5),
                        ToFunc(m => m.CreateFunc5())
                    },
                    new object[] {
                        nameof(StaticFake.Func6),
                        ToFunc(m => m.CreateFunc6<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func6),
                        ToFunc(m => m.CreateFunc6())
                    },
                    new object[] {
                        nameof(StaticFake.Func7),
                        ToFunc(m => m.CreateFunc7<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func7),
                        ToFunc(m => m.CreateFunc7())
                    },
                    new object[] {
                        nameof(StaticFake.Func8),
                        ToFunc(m => m.CreateFunc8<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func8),
                        ToFunc(m => m.CreateFunc8())
                    },
                    new object[] {
                        nameof(StaticFake.Func9),
                        ToFunc(m => m.CreateFunc9<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func9),
                        ToFunc(m => m.CreateFunc9())
                    },
                    new object[] {
                        nameof(StaticFake.Action0),
                        ToFunc(m => m.CreateAction0())
                    },
                    new object[] {
                        nameof(StaticFake.Action0),
                        ToFunc(m => m.CreateAction0<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Action1),
                        ToFunc(m => m.CreateAction1())
                    },
                    new object[] {
                        nameof(StaticFake.Action1),
                        ToFunc(m => m.CreateAction1<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Action1),
                        ToFunc(m => m.CreateAction1<StaticFake, int>())
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(StaticMethodCases))]
        public void CreateDelegate_StaticMethod(
            String methodName,
            Func<MethodInfo, object> createDelegate) {

            // Arrange

            var method = typeof(StaticFake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => createDelegate(method)
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{methodName}' method is static." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public class StaticFake {

            public static int Func0() {
                return 0;
            }

            public static int Func1(int a) {
                return a;
            }

            public static int Func2(int a, int b) {
                return a + b;
            }

            public static int Func3(int a, int b, int c) {
                return a + b + c;
            }

            public static int Func4(int a, int b, int c, int d) {
                return a + b + c + d;
            }

            public static int Func5(int a, int b, int c, int d, int e) {
                return a + b + c + d + e;
            }

            public static int Func6(int a, int b, int c, int d, int e, int f) {
                return a + b + c + d + e + f;
            }

            public static int Func7(int a, int b, int c, int d, int e, int f, int g) {
                return a + b + c + d + e + f + g;
            }

            public static int Func8(int a, int b, int c, int d, int e, int f, int g, int h) {
                return a + b + c + d + e + f + g + h;
            }

            public static int Func9(
                int a, int b, int c, int d, int e, int f, int g, int h, int i) {

                return a + b + c + d + e + f + g + h + i;
            }

            public static void Action0() {}
            public static void Action1(int a) {}

        }

    }

}
