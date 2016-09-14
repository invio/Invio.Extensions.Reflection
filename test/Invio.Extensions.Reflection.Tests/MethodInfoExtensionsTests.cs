using System;
using System.Collections.Generic;
using System.Reflection;

using Xunit;

using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class MethodInfoExtensionsTests {
        public static TheoryData ClassUnderTestCreateFuncs {
            get {
                var theoryData = new TheoryData<String, Action<MethodInfo>>();
                var actions = MakeCreateFuncActions<MethodInfoExtensionsTests>();
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

        private Random random { get; }

        public MethodInfoExtensionsTests() {
            this.random = new Random();
        }

        [Fact]
        public void MethodInfoDelegateFunc0_WrongReturnType() {
            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentException>(
                () => func0MethodInfo.CreateFunc0<ClassUnderTest, double>()
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc0_WithReturnType() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));

            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            var func0 = (Func<ClassUnderTest, int>)func0MethodInfo.CreateFunc0<ClassUnderTest, int>();
            Assert.Equal(sut.Func0(), func0(sut));
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

            Assert.Throws<NotSupportedException>(
                () => createFunc(methodInfo)
            );
        }

        [Fact]
        public void MethodInfoDelegateFunc0() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));

            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc0 = func0MethodInfo.CreateFunc0<ClassUnderTest>();
            Assert.Equal(sut.Func0(), typedFunc0(sut));
            var func0 = func0MethodInfo.CreateFunc0();
            Assert.Equal(sut.Func0(), func0(sut));
        }

        [Fact]
        public void MethodInfoDelegateFunc1() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(1);

            var func1MethodInfo = sutType.GetMethod("Func1", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc1 = func1MethodInfo.CreateFunc1<ClassUnderTest>();
            Assert.Equal(sut.Func1(args[0]), typedFunc1(sut, args[0]));
            var func1 = func1MethodInfo.CreateFunc1();
            Assert.Equal(sut.Func1(args[0]), func1(sut, args[0]));
        }

        [Fact]
        public void MethodInfoDelegateFunc2() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(2);

            var func2MethodInfo = sutType.GetMethod("Func2", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc2 = func2MethodInfo.CreateFunc2<ClassUnderTest>();
            Assert.Equal(sut.Func2(args[0], args[1]), typedFunc2(sut, args[0], args[1]));
            var func2 = func2MethodInfo.CreateFunc2();
            Assert.Equal(sut.Func2(args[0], args[1]), func2(sut, args[0], args[1]));
        }

        [Fact]
        public void MethodInfoDelegateFunc3() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(3);

            var func3MethodInfo = sutType.GetMethod("Func3", BindingFlags.Instance | BindingFlags.Public);
            var typedFunc3 = func3MethodInfo.CreateFunc3<ClassUnderTest>();
            Assert.Equal(sut.Func3(args[0], args[1], args[2]), typedFunc3(sut, args[0], args[1], args[2]));
            var func3 = func3MethodInfo.CreateFunc3();
            Assert.Equal(sut.Func3(args[0], args[1], args[2]), func3(sut, args[0], args[1], args[2]));
        }

        [Fact]
        public void MethodInfoDelegateFunc4() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(4);

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
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(5);

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
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(6);

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
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(7);

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
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(8);

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
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(9);

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
        public void CreateCompatibleDelegate_ArgNull_Checks() {
            var methodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            Assert.Throws<ArgumentNullException>(
                () => MethodInfoExtensions.CreateCompatibleDelegate<int>(instanceType: null, methodInfo: methodInfo)
            );
            Assert.Throws<ArgumentNullException>(
                () => MethodInfoExtensions.CreateCompatibleDelegate<int>(typeof(ClassUnderTest), methodInfo: null)
            );
        }

        /// <summary>
        /// Creates a random arguments for the count of args supplied.
        /// </summary>
        private int[] getArgumentArray(int argCount) {
            var args = new int[argCount];
            for (int i = 0; i < argCount; i++) {
                args[i] = this.random.Next(1, 100);
            }
            return args;
        }

        private static readonly Type sutType = typeof(ClassUnderTest);
        class ClassUnderTest {
            private readonly int modifier;
            private int modified;

            public ClassUnderTest(int modifier) {
                this.modifier = modifier;
                this.modified = modifier;
            }

            public void incrementModified() {
                modified++;
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
