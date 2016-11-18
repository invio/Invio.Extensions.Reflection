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

        public static TheoryData IteratorCount {
            get {
                return new TheoryData<Int32> { MethodInfoExtensionsTests.intGenerator.Next() };
            }
        }

        private static Int32Generator intGenerator { get; } = new Int32Generator(1, 101);

        [Fact]
        public void MethodInfoDelegateFunc0_WrongReturnType() {
            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentException>(
                () => func0MethodInfo.CreateFunc0<ClassUnderTest, double>()
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc0_WithReturnType() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());

            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            var func0 = (Func<ClassUnderTest, int>)func0MethodInfo.CreateFunc0<ClassUnderTest, int>();
            Assert.Equal(sut.Func0(), func0(sut));
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
        public void MethodInfoDelegate_ArgNull_MethodInfo(String functionName, Action<MethodInfo> createFunc) {
            Assert.Throws<ArgumentNullException>(
                () => createFunc(null)
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

        [Fact]
        public void MethodInfoDelegateFunc0() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());

            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc0 = func0MethodInfo.CreateFunc0<ClassUnderTest>();
            Assert.Equal(sut.Func0(), typedFunc0(sut));
            var func0 = func0MethodInfo.CreateFunc0();
            Assert.Equal(sut.Func0(), func0(sut));
        }

        [Fact]
        public void MethodInfoDelegateFunc1() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(1);

            var func1MethodInfo = sutType.GetMethod("Func1", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc1 = func1MethodInfo.CreateFunc1<ClassUnderTest>();
            Assert.Equal(sut.Func1(args[0]), typedFunc1(sut, args[0]));
            var func1 = func1MethodInfo.CreateFunc1();
            Assert.Equal(sut.Func1(args[0]), func1(sut, args[0]));
        }

        [Fact]
        public void MethodInfoDelegateFunc2() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(2);

            var func2MethodInfo = sutType.GetMethod("Func2", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc2 = func2MethodInfo.CreateFunc2<ClassUnderTest>();
            Assert.Equal(sut.Func2(args[0], args[1]), typedFunc2(sut, args[0], args[1]));
            var func2 = func2MethodInfo.CreateFunc2();
            Assert.Equal(sut.Func2(args[0], args[1]), func2(sut, args[0], args[1]));
        }

        [Fact]
        public void MethodInfoDelegateFunc3() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(3);

            var func3MethodInfo = sutType.GetMethod("Func3", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc3 = func3MethodInfo.CreateFunc3<ClassUnderTest>();
            Assert.Equal(sut.Func3(args[0], args[1], args[2]), typedFunc3(sut, args[0], args[1], args[2]));
            var func3 = func3MethodInfo.CreateFunc3();
            Assert.Equal(sut.Func3(args[0], args[1], args[2]), func3(sut, args[0], args[1], args[2]));
        }

        [Fact]
        public void MethodInfoDelegateFunc4() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(4);

            var func4MethodInfo = sutType.GetMethod("Func4", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc4 = func4MethodInfo.CreateFunc4<ClassUnderTest>();
            Assert.Equal(
                sut.Func4(args[0], args[1], args[2], args[3]),
                typedFunc4(sut, args[0], args[1], args[2], args[3])
            );
            var func4 = func4MethodInfo.CreateFunc4();
            Assert.Equal(
                sut.Func4(args[0], args[1], args[2], args[3]),
                func4(sut, args[0], args[1], args[2], args[3])
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc5() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(5);

            var func5MethodInfo = sutType.GetMethod("Func5", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc5 = func5MethodInfo.CreateFunc5<ClassUnderTest>();
            Assert.Equal(
                sut.Func5(args[0], args[1], args[2], args[3], args[4]),
                typedFunc5(sut, args[0], args[1], args[2], args[3], args[4])
            );
            var func5 = func5MethodInfo.CreateFunc5();
            Assert.Equal(
                sut.Func5(args[0], args[1], args[2], args[3], args[4]),
                func5(sut, args[0], args[1], args[2], args[3], args[4])
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc6() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(6);

            var func6MethodInfo = sutType.GetMethod("Func6", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc6 = func6MethodInfo.CreateFunc6<ClassUnderTest>();
            Assert.Equal(
                sut.Func6(args[0], args[1], args[2], args[3], args[4], args[5]),
                typedFunc6(sut, args[0], args[1], args[2], args[3], args[4], args[5])
            );
            var func6 = func6MethodInfo.CreateFunc6();
            Assert.Equal(
                sut.Func6(args[0], args[1], args[2], args[3], args[4], args[5]),
                func6(sut, args[0], args[1], args[2], args[3], args[4], args[5])
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc7() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(7);

            var func7MethodInfo = sutType.GetMethod("Func7", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc7 = func7MethodInfo.CreateFunc7<ClassUnderTest>();
            Assert.Equal(
                sut.Func7(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
                typedFunc7(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6])
            );
            var func7 = func7MethodInfo.CreateFunc7();
            Assert.Equal(
                sut.Func7(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
                func7(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6])
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc8() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(8);

            var func8MethodInfo = sutType.GetMethod("Func8", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc8 = func8MethodInfo.CreateFunc8<ClassUnderTest>();
            Assert.Equal(
                sut.Func8(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
                typedFunc8(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7])
            );
            var func8 = func8MethodInfo.CreateFunc8();
            Assert.Equal(
                sut.Func8(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
                func8(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7])
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc9() {
            var sut = new ClassUnderTest(MethodInfoExtensionsTests.intGenerator.Next());
            var args = generateIntArray(9);

            var func9MethodInfo = sutType.GetMethod("Func9", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc9 = func9MethodInfo.CreateFunc9<ClassUnderTest>();
            Assert.Equal(
                sut.Func9(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
                typedFunc9(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8])
            );
            var func9 = func9MethodInfo.CreateFunc9();
            Assert.Equal(
                sut.Func9(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
                func9(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8])
            );
        }

        [Fact]
        public void MethodInfoCreateAction_ArgNull() {
            MethodInfo nullMI = null;
            Assert.Throws<ArgumentNullException>(
                () => nullMI.CreateAction0()
            );
            Assert.Throws<ArgumentNullException>(
                () => nullMI.CreateAction0<ClassUnderTest>()
            );
            Assert.Throws<ArgumentNullException>(
                () => nullMI.CreateAction1()
            );
            Assert.Throws<ArgumentNullException>(
                () => nullMI.CreateAction1<ClassUnderTest>()
            );
            Assert.Throws<ArgumentNullException>(
                () => nullMI.CreateAction1<ClassUnderTest, int>()
            );
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
            Assert.Contains($"exactly match the generic type parameter T2", ex.Message);
        }

        [Theory]
        [MemberData(nameof(IteratorCount))]
        public void MethodInfoDelegateAction0(int iterateCount) {
            var initialValue = MethodInfoExtensionsTests.intGenerator.Next();
            var sut = new ClassUnderTest(initialValue);
            var expected = new ClassUnderTest(initialValue);

            var action0MethodInfo =
                sutType.GetMethod("incrementModified", BindingFlags.Instance | BindingFlags.Public);
            var typedAction0 = action0MethodInfo.CreateAction0<ClassUnderTest>();
            for (int i = 0; i < iterateCount; i++) {
                typedAction0(sut);
                expected.incrementModified();
            }
            Assert.Equal(expected.modified, sut.modified);

            sut = new ClassUnderTest(initialValue);
            var action0 = action0MethodInfo.CreateAction0();
            for (int i = 0; i < iterateCount; i++) {
                action0(sut);
            }
            Assert.Equal(expected.modified, sut.modified);
        }

        [Theory]
        [MemberData(nameof(IteratorCount))]
        public void MethodInfoDelegateAction1(int iterateCount) {
            var initialValue = MethodInfoExtensionsTests.intGenerator.Next();
            var sut = new ClassUnderTest(initialValue);
            var expected = new ClassUnderTest(initialValue);
            var argsPerIterate = generateIntArray(iterateCount);

            var action1MethodInfo =
                sutType.GetMethod("addToModified", BindingFlags.Instance | BindingFlags.Public);
            var argTypeAction1 = action1MethodInfo.CreateAction1<ClassUnderTest, int>();
            for (int i = 0; i < iterateCount; i++) {
                expected.addToModified(argsPerIterate[i]);
                argTypeAction1(sut, argsPerIterate[i]);
            }
            Assert.Equal(expected.modified, sut.modified);

            sut = new ClassUnderTest(initialValue);
            var typeAction1 = action1MethodInfo.CreateAction1<ClassUnderTest>();
            for (int i = 0; i < iterateCount; i++) {
                typeAction1(sut, argsPerIterate[i]);
            }
            Assert.Equal(expected.modified, sut.modified);

            sut = new ClassUnderTest(initialValue);
            var action1 = action1MethodInfo.CreateAction1();
            for (int i = 0; i < iterateCount; i++) {
                action1(sut, argsPerIterate[i]);
            }
            Assert.Equal(expected.modified, sut.modified);
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

        /// <summary>
        /// Creates a random arguments for the count of args supplied.
        /// </summary>
        private int[] generateIntArray(int argCount) {
            var args = new int[argCount];
            for (int i = 0; i < argCount; i++) {
                args[i] = MethodInfoExtensionsTests.intGenerator.Next();
            }
            return args;
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
    }
}
