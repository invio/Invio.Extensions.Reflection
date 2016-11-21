using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invio.Xunit;
using Xunit;

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

        public static IEnumerable<object[]> ActionWithReturnTypeCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(Fake.Func0), TypedTestCases<Fake>.Action0 },
                    new object[] { nameof(Fake.Func0), TestCases<Fake>.Action0 },
                    new object[] { nameof(Fake.Func1), TypedTestCases<Fake>.Action1 },
                    new object[] { nameof(Fake.Func1), TestCases<Fake>.Action1 },
                    new object[] { nameof(Fake.Func2), TypedTestCases<Fake>.Action2 },
                    new object[] { nameof(Fake.Func2), TestCases<Fake>.Action2 },
                    new object[] { nameof(Fake.Func3), TypedTestCases<Fake>.Action3 },
                    new object[] { nameof(Fake.Func3), TestCases<Fake>.Action3 },
                    new object[] { nameof(Fake.Func4), TypedTestCases<Fake>.Action4 },
                    new object[] { nameof(Fake.Func4), TestCases<Fake>.Action4 },
                    new object[] { nameof(Fake.Func5), TypedTestCases<Fake>.Action5 },
                    new object[] { nameof(Fake.Func5), TestCases<Fake>.Action5 },
                    new object[] { nameof(Fake.Func6), TypedTestCases<Fake>.Action6 },
                    new object[] { nameof(Fake.Func6), TestCases<Fake>.Action6 },
                    new object[] { nameof(Fake.Func7), TypedTestCases<Fake>.Action7 },
                    new object[] { nameof(Fake.Func7), TestCases<Fake>.Action7 },
                    new object[] { nameof(Fake.Func8), TypedTestCases<Fake>.Action8 },
                    new object[] { nameof(Fake.Func8), TestCases<Fake>.Action8 },
                    new object[] { nameof(Fake.Func9), TypedTestCases<Fake>.Action9 },
                    new object[] { nameof(Fake.Func9), TestCases<Fake>.Action9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ActionWithReturnTypeCases))]
        public void CreateAction_OnMethodInfoWithReturnValue<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> testCase) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => testCase.CreateDelegate(method)
            );

            // Assert

            Assert.Equal(
                "You cannot create an Action delegate for a method with a " +
                "non-void return type." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public static IEnumerable<object[]> FuncWithoutReturnTypeCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(Fake.Action0), TypedTestCases<Fake>.Func0 },
                    new object[] { nameof(Fake.Action0), TestCases<Fake>.Func0 },
                    new object[] { nameof(Fake.Action1), TypedTestCases<Fake>.Func1 },
                    new object[] { nameof(Fake.Action1), TestCases<Fake>.Func1 },
                    new object[] { nameof(Fake.Action2), TypedTestCases<Fake>.Func2 },
                    new object[] { nameof(Fake.Action2), TestCases<Fake>.Func2 },
                    new object[] { nameof(Fake.Action3), TypedTestCases<Fake>.Func3 },
                    new object[] { nameof(Fake.Action3), TestCases<Fake>.Func3 },
                    new object[] { nameof(Fake.Action4), TypedTestCases<Fake>.Func4 },
                    new object[] { nameof(Fake.Action4), TestCases<Fake>.Func4 },
                    new object[] { nameof(Fake.Action5), TypedTestCases<Fake>.Func5 },
                    new object[] { nameof(Fake.Action5), TestCases<Fake>.Func5 },
                    new object[] { nameof(Fake.Action6), TypedTestCases<Fake>.Func6 },
                    new object[] { nameof(Fake.Action6), TestCases<Fake>.Func6 },
                    new object[] { nameof(Fake.Action7), TypedTestCases<Fake>.Func7 },
                    new object[] { nameof(Fake.Action7), TestCases<Fake>.Func7 },
                    new object[] { nameof(Fake.Action8), TypedTestCases<Fake>.Func8 },
                    new object[] { nameof(Fake.Action8), TestCases<Fake>.Func8 },
                    new object[] { nameof(Fake.Action9), TypedTestCases<Fake>.Func9 },
                    new object[] { nameof(Fake.Action9), TestCases<Fake>.Func9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(FuncWithoutReturnTypeCases))]
        public void CreateFunc_OnVoidMethodInfo<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> testCase) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => testCase.CreateDelegate(method)
            );

            // Assert

            Assert.Equal(
                "You cannot create a Func delegate for a method with a " +
                "void return type." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public static IEnumerable<object[]> TooFewArgumentsCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(Fake.Func0), TypedTestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func0), TestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func1), TypedTestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func1), TestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func2), TypedTestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func2), TestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func3), TypedTestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func3), TestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func4), TypedTestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func4), TestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func5), TypedTestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func5), TestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func6), TypedTestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func6), TestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func7), TypedTestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func7), TestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func8), TypedTestCases<Fake>.Func9, 9 },
                    new object[] { nameof(Fake.Func8), TestCases<Fake>.Func9, 9},
                    new object[] { nameof(Fake.Action0), TypedTestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action0), TestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action1), TypedTestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action1), TestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action2), TypedTestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action2), TestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action3), TypedTestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action3), TestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action4), TypedTestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action4), TestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action5), TypedTestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action5), TestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action6), TypedTestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action6), TestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action7), TypedTestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action7), TestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action8), TypedTestCases<Fake>.Action9, 9 },
                    new object[] { nameof(Fake.Action8), TestCases<Fake>.Action9, 9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TooFewArgumentsCases))]
        public void CreateDelegate_TooFewArguments<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> test,
            int expectedNumberOfParameters) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => test.CreateDelegate(method)
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);

            Assert.Equal(
                "The 'method' argument must reference a " +
                "MethodInfo with " + expectedNumberOfParameters + " parameters." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public static IEnumerable<object[]> TooManyArgumentsCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(Fake.Func1), TypedTestCases<Fake>.Func0, 0 },
                    new object[] { nameof(Fake.Func1), TestCases<Fake>.Func0, 0 },
                    new object[] { nameof(Fake.Func2), TypedTestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func2), TestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func3), TypedTestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func3), TestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func4), TypedTestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func4), TestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func5), TypedTestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func5), TestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func6), TypedTestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func6), TestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func7), TypedTestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func7), TestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func8), TypedTestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func8), TestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func9), TypedTestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func9), TestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func10), TypedTestCases<Fake>.Func9, 9 },
                    new object[] { nameof(Fake.Func10), TestCases<Fake>.Func9, 9 },
                    new object[] { nameof(Fake.Action1), TypedTestCases<Fake>.Action0, 0 },
                    new object[] { nameof(Fake.Action1), TestCases<Fake>.Action0, 0 },
                    new object[] { nameof(Fake.Action2), TypedTestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action2), TestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action3), TypedTestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action3), TestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action4), TypedTestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action4), TestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action5), TypedTestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action5), TestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action6), TypedTestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action6), TestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action7), TypedTestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action7), TestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action8), TypedTestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action8), TestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action9), TypedTestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action9), TestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action10), TypedTestCases<Fake>.Action9, 9 },
                    new object[] { nameof(Fake.Action10), TestCases<Fake>.Action9, 9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TooManyArgumentsCases))]
        public void CreateDelegate_TooManyArguments<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> test,
            int expectedNumberOfParameters) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => test.CreateDelegate(method)
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);

            Assert.Equal(
                "The 'method' argument must reference a " +
                "MethodInfo with " + expectedNumberOfParameters + " parameters." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public static IEnumerable<object[]> InvalidBaseTypeCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(IFake.Func0), TypedTestCases<IFake>.Func0 },
                    new object[] { nameof(IFake.Func1), TypedTestCases<IFake>.Func1 },
                    new object[] { nameof(IFake.Func2), TypedTestCases<IFake>.Func2 },
                    new object[] { nameof(IFake.Func3), TypedTestCases<IFake>.Func3 },
                    new object[] { nameof(IFake.Func4), TypedTestCases<IFake>.Func4 },
                    new object[] { nameof(IFake.Func5), TypedTestCases<IFake>.Func5 },
                    new object[] { nameof(IFake.Func6), TypedTestCases<IFake>.Func6 },
                    new object[] { nameof(IFake.Func7), TypedTestCases<IFake>.Func7 },
                    new object[] { nameof(IFake.Func8), TypedTestCases<IFake>.Func8 },
                    new object[] { nameof(IFake.Func9), TypedTestCases<IFake>.Func9 },
                    new object[] { nameof(IFake.Action0), TypedTestCases<IFake>.Action0 },
                    new object[] { nameof(IFake.Action1), TypedTestCases<IFake>.Action1 },
                    new object[] { nameof(IFake.Action2), TypedTestCases<IFake>.Action2 },
                    new object[] { nameof(IFake.Action3), TypedTestCases<IFake>.Action3 },
                    new object[] { nameof(IFake.Action4), TypedTestCases<IFake>.Action4 },
                    new object[] { nameof(IFake.Action5), TypedTestCases<IFake>.Action5 },
                    new object[] { nameof(IFake.Action6), TypedTestCases<IFake>.Action6 },
                    new object[] { nameof(IFake.Action7), TypedTestCases<IFake>.Action7 },
                    new object[] { nameof(IFake.Action8), TypedTestCases<IFake>.Action8 },
                    new object[] { nameof(IFake.Action9), TypedTestCases<IFake>.Action9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidBaseTypeCases))]
        public void CreateDelegate_InvalidBaseType<TFunc>(
            String methodName,
            ITestCase<IFake, TFunc> test) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var exception = Record.Exception(
                () => test.CreateDelegate(method)
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);

            Assert.Equal(
                "Type parameter 'TBase' was 'IFake', which is not " +
                "assignable to the method's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: method",
                exception.Message
            );
        }

        public static IEnumerable<object[]> ValidBaseTypeCases {
            get {
                return new List<object[]> {
                    new object[] { nameof(Fake.Func0), TypedTestCases<Fake>.Func0, 0 },
                    new object[] { nameof(Fake.Func0), TestCases<Fake>.Func0, 0 },
                    new object[] { nameof(Fake.Func1), TypedTestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func1), TestCases<Fake>.Func1, 1 },
                    new object[] { nameof(Fake.Func2), TypedTestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func2), TestCases<Fake>.Func2, 2 },
                    new object[] { nameof(Fake.Func3), TypedTestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func3), TestCases<Fake>.Func3, 3 },
                    new object[] { nameof(Fake.Func4), TypedTestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func4), TestCases<Fake>.Func4, 4 },
                    new object[] { nameof(Fake.Func5), TypedTestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func5), TestCases<Fake>.Func5, 5 },
                    new object[] { nameof(Fake.Func6), TypedTestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func6), TestCases<Fake>.Func6, 6 },
                    new object[] { nameof(Fake.Func7), TypedTestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func7), TestCases<Fake>.Func7, 7 },
                    new object[] { nameof(Fake.Func8), TypedTestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func8), TestCases<Fake>.Func8, 8 },
                    new object[] { nameof(Fake.Func9), TypedTestCases<Fake>.Func9, 9 },
                    new object[] { nameof(Fake.Func9), TestCases<Fake>.Func9, 9 },
                    new object[] { nameof(Fake.Action0), TypedTestCases<Fake>.Action0, 0 },
                    new object[] { nameof(Fake.Action0), TestCases<Fake>.Action0, 0 },
                    new object[] { nameof(Fake.Action1), TypedTestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action1), TestCases<Fake>.Action1, 1 },
                    new object[] { nameof(Fake.Action2), TypedTestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action2), TestCases<Fake>.Action2, 2 },
                    new object[] { nameof(Fake.Action3), TypedTestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action3), TestCases<Fake>.Action3, 3 },
                    new object[] { nameof(Fake.Action4), TypedTestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action4), TestCases<Fake>.Action4, 4 },
                    new object[] { nameof(Fake.Action5), TypedTestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action5), TestCases<Fake>.Action5, 5 },
                    new object[] { nameof(Fake.Action6), TypedTestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action6), TestCases<Fake>.Action6, 6 },
                    new object[] { nameof(Fake.Action7), TypedTestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action7), TestCases<Fake>.Action7, 7 },
                    new object[] { nameof(Fake.Action8), TypedTestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action8), TestCases<Fake>.Action8, 8 },
                    new object[] { nameof(Fake.Action9), TypedTestCases<Fake>.Action9, 9 },
                    new object[] { nameof(Fake.Action9), TestCases<Fake>.Action9, 9 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidBaseTypeCases))]
        public void CreateDelegate_ExactBaseType<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> test,
            int expectedValue) {

            // Arrange

            var fake = new Fake();
            var method = typeof(Fake).GetMethod(methodName);

            // Act

            var methodDelegate = test.CreateDelegate(method);
            var actualValue = test.InvokeDelegate(fake, methodDelegate);

            // Assert

            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [MemberData(nameof(ValidBaseTypeCases))]
        public void CreateDelegate_AssignableBaseType<TFunc>(
            String methodName,
            ITestCase<Fake, TFunc> test,
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
                    new object[] { ToFunc(m => m.CreateFunc0<Fake>()) },
                    new object[] { ToFunc(m => m.CreateFunc0()) },
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
                    new object[] { ToFunc(m => m.CreateAction1<Fake>()) }
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

        public static class TypedTestCases<TBase> where TBase : class, IFake {

            public static ITestCase<TBase, Func<TBase, object>> Func0 =
                new FuncTestCase<TBase, Func<TBase, object>>(
                    method => method.CreateFunc0<TBase>(),
                    (fake, func) => func(fake)
                );

            public static ITestCase<TBase, Func<TBase, object, object>> Func1 =
                new FuncTestCase<TBase, Func<TBase, object, object>>(
                    method => method.CreateFunc1<TBase>(),
                    (fake, func) => func(fake, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object>> Func2 =
                new FuncTestCase<TBase, Func<TBase, object, object, object>>(
                    method => method.CreateFunc2<TBase>(),
                    (fake, func) => func(fake, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object>> Func3 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object>>(
                    method => method.CreateFunc3<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase,  object, object, object, object, object>> Func4 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object>>(
                    method => method.CreateFunc4<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object, object, object>> Func5 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object, object>>(
                    method => method.CreateFunc5<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object, object, object, object>> Func6 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc6<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object>> Func7 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc7<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object, object>> Func8 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc8<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object, object, object>> Func9 =
                new FuncTestCase<TBase, Func<TBase, object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc9<TBase>(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase>> Action0 =
                new ActionTestCase<TBase, Action<TBase>>(
                    method => method.CreateAction0<TBase>(),
                    (fake, action) => action(fake)
                );

            public static ITestCase<TBase, Action<TBase, object>> Action1 =
                new ActionTestCase<TBase, Action<TBase, object>>(
                    method => method.CreateAction1<TBase>(),
                    (fake, action) => action(fake, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object>> Action2 =
                new ActionTestCase<TBase, Action<TBase, object, object>>(
                    method => method.CreateAction2<TBase>(),
                    (fake, action) => action(fake, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object>> Action3 =
                new ActionTestCase<TBase, Action<TBase, object, object, object>>(
                    method => method.CreateAction3<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object>> Action4 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object>>(
                    method => method.CreateAction4<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object, object>> Action5 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object, object>>(
                    method => method.CreateAction5<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object, object, object>> Action6 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object, object, object>>(
                    method => method.CreateAction6<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object, object, object, object>> Action7 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object, object, object, object>>(
                    method => method.CreateAction7<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object, object, object, object, object>> Action8 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object, object, object, object, object>>(
                    method => method.CreateAction8<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<TBase, object, object, object, object, object, object, object, object, object>> Action9 =
                new ActionTestCase<TBase, Action<TBase, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateAction9<TBase>(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );
        }

        public static class TestCases<TBase> where TBase : class, IFake {

            public static ITestCase<TBase, Func<object, object>> Func0 =
                new FuncTestCase<TBase, Func<object, object>>(
                    method => method.CreateFunc0(),
                    (fake, func) => func(fake)
                );

            public static ITestCase<TBase, Func<object, object, object>> Func1 =
                new FuncTestCase<TBase, Func<object, object, object>>(
                    method => method.CreateFunc1(),
                    (fake, func) => func(fake, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object>> Func2 =
                new FuncTestCase<TBase, Func<object, object, object, object>>(
                    method => method.CreateFunc2(),
                    (fake, func) => func(fake, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object>> Func3 =
                new FuncTestCase<TBase, Func<object, object, object, object, object>>(
                    method => method.CreateFunc3(),
                    (fake, func) => func(fake, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object>> Func4 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object>>(
                    method => method.CreateFunc4(),
                    (fake, func) => func(fake, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object, object>> Func5 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object, object>>(
                    method => method.CreateFunc5(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object, object, object>> Func6 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc6(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object, object, object, object>> Func7 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc7(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object, object, object, object, object>> Func8 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc8(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Func<object, object, object, object, object, object, object, object, object, object, object>> Func9 =
                new FuncTestCase<TBase, Func<object, object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateFunc9(),
                    (fake, func) => func(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object>> Action0 =
                new ActionTestCase<TBase, Action<object>>(
                    method => method.CreateAction0(),
                    (fake, action) => action(fake)
                );

            public static ITestCase<TBase, Action<object, object>> Action1 =
                new ActionTestCase<TBase, Action<object, object>>(
                    method => method.CreateAction1(),
                    (fake, action) => action(fake, 1)
                );

            public static ITestCase<TBase, Action<object, object, object>> Action2 =
                new ActionTestCase<TBase, Action<object, object, object>>(
                    method => method.CreateAction2(),
                    (fake, action) => action(fake, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object>> Action3 =
                new ActionTestCase<TBase, Action<object, object, object, object>>(
                    method => method.CreateAction3(),
                    (fake, action) => action(fake, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object>> Action4 =
                new ActionTestCase<TBase, Action<object, object, object, object, object>>(
                    method => method.CreateAction4(),
                    (fake, action) => action(fake, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object, object>> Action5 =
                new ActionTestCase<TBase, Action<object, object, object, object, object, object>>(
                    method => method.CreateAction5(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object, object, object>> Action6 =
                new ActionTestCase<TBase, Action<object, object, object, object, object, object, object>>(
                    method => method.CreateAction6(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object, object, object, object>> Action7 =
                new ActionTestCase<TBase, Action<object, object, object, object, object, object, object, object>>(
                    method => method.CreateAction7(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object, object, object, object, object>> Action8 =
                new ActionTestCase<TBase, Action<object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateAction8(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1, 1)
                );

            public static ITestCase<TBase, Action<object, object, object, object, object, object, object, object, object, object>> Action9 =
                new ActionTestCase<TBase, Action<object, object, object, object, object, object, object, object, object, object>>(
                    method => method.CreateAction9(),
                    (fake, action) => action(fake, 1, 1, 1, 1, 1, 1, 1, 1, 1)
                );

        }

        public interface ITestCase<TBase, TFunc> {

            TFunc CreateDelegate(MethodInfo method);
            int InvokeDelegate(TBase fake, TFunc func);

        }

        private class FuncTestCase<TBase, TFunc> : ITestCase<TBase, TFunc>
            where TBase : class, IFake {

            private Func<MethodInfo, TFunc> createDelegate { get; }
            private Func<TBase, TFunc, object> invokeDelegate { get; }

            public FuncTestCase(
                Func<MethodInfo, TFunc> createDelegate,
                Func<TBase, TFunc, object> invokeDelegate) {

                this.createDelegate = createDelegate;
                this.invokeDelegate = invokeDelegate;
            }

            public TFunc CreateDelegate(MethodInfo method) {
                return this.createDelegate(method);
            }

            public int InvokeDelegate(TBase fake, TFunc action) {
                return (int)this.invokeDelegate(fake, action);
            }

        }

        private class ActionTestCase<TBase, TAction> : ITestCase<TBase, TAction>
            where TBase : class, IFake {

            private Func<MethodInfo, TAction> createDelegate { get; }
            private Action<TBase, TAction> invokeDelegate { get; }

            public ActionTestCase(
                Func<MethodInfo, TAction> createDelegate,
                Action<TBase, TAction> invokeDelegate) {

                this.createDelegate = createDelegate;
                this.invokeDelegate = invokeDelegate;
            }

            public TAction CreateDelegate(MethodInfo method) {
                return this.createDelegate(method);
            }

            public int InvokeDelegate(TBase fake, TAction action) {
                this.invokeDelegate(fake, action);

                return fake.Value;
            }

        }

        public interface IFake {

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

            int Value { get; }
            void Action0();
            void Action1(int a);
            void Action2(int a, int b);
            void Action3(int a, int b, int c);
            void Action4(int a, int b, int c, int d);
            void Action5(int a, int b, int c, int d, int e);
            void Action6(int a, int b, int c, int d, int e, int f);
            void Action7(int a, int b, int c, int d, int e, int f, int g);
            void Action8(int a, int b, int c, int d, int e, int f, int g, int h);
            void Action9(int a, int b, int c, int d, int e, int f, int g, int h, int i);

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

            public int Func9(int a, int b, int c, int d, int e, int f, int g, int h, int i) {
                return a + b + c + d + e + f + g + h + i;
            }

            public int Func10(
                int a, int b, int c, int d, int e, int f, int g, int h, int i, int j) {

                return a + b + c + d + e + f + g + h + i + j;
            }

            public void Action0() {
                this.Value = 0;
            }

            public void Action1(int a) {
                this.Value = a;
            }

            public void Action2(int a, int b) {
                this.Value = a + b;
            }

            public void Action3(int a, int b, int c) {
                this.Value = a + b + c;
            }

            public void Action4(int a, int b, int c, int d) {
                this.Value = a + b + c + d;
            }

            public void Action5(int a, int b, int c, int d, int e) {
                this.Value = a + b + c + d + e;
            }

            public void Action6(int a, int b, int c, int d, int e, int f) {
                this.Value = a + b + c + d + e + f;
            }

            public void Action7(int a, int b, int c, int d, int e, int f, int g) {
                this.Value = a + b + c + d + e + f + g;
            }

            public void Action8(int a, int b, int c, int d, int e, int f, int g, int h) {
                this.Value = a + b + c + d + e + f + g + h;
            }

            public void Action9(int a, int b, int c, int d, int e, int f, int g, int h, int i) {
                this.Value = a + b + c + d + e + f + g + h + i;
            }

            public void Action10(
                int a, int b, int c, int d, int e, int f, int g, int h, int i, int j) {

                this.Value = a + b + c + d + e + f + g + h + i + j;
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
                        ToFunc(m => m.CreateFunc0<StaticFake>())
                    },
                    new object[] {
                        nameof(StaticFake.Func0),
                        ToFunc(m => m.CreateFunc0())
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
