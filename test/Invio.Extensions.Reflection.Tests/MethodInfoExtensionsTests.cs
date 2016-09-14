using System;
using System.Reflection;

using Xunit;

namespace Invio.Extensions.Reflection {

    public class MethodInfoExtensionsTests {

        private Random random { get; }

        public MethodInfoExtensionsTests() {
            this.random = new Random();
        }

        [Fact]
        public void MethodInfoDelegateFunc0() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));

            var func0MethodInfo = sutType.GetMethod("Func0", BindingFlags.Instance | BindingFlags.Public);
            var func0 = (Func<ClassUnderTest, object>)func0MethodInfo.CreateFunc0<ClassUnderTest>();
            Assert.Equal(sut.Func0(), func0(sut));
        }

        [Fact]
        public void MethodInfoDelegateFunc1() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(1);

            var func1MethodInfo = sutType.GetMethod("Func1", BindingFlags.Instance | BindingFlags.Public);
            var func1 = (Func<ClassUnderTest, object, object>)func1MethodInfo.CreateFunc1<ClassUnderTest>();
            Assert.Equal(sut.Func1(args[0]), func1(sut, args[0]));
        }

        [Fact]
        public void MethodInfoDelegateFunc2() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(2);

            var func2MethodInfo = sutType.GetMethod("Func2", BindingFlags.Instance | BindingFlags.Public);
            var func2 = (Func<ClassUnderTest, object, object, object>)func2MethodInfo.CreateFunc2<ClassUnderTest>();
            Assert.Equal(sut.Func2(args[0], args[1]), func2(sut, args[0], args[1]));
        }

        [Fact]
        public void MethodInfoDelegateFunc3() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(3);

            var func3MethodInfo = sutType.GetMethod("Func3", BindingFlags.Instance | BindingFlags.Public);
            var func3 = (Func<ClassUnderTest, object, object, object, object>)
                func3MethodInfo.CreateFunc3<ClassUnderTest>();
            Assert.Equal(sut.Func3(args[0], args[1], args[2]), func3(sut, args[0], args[1], args[2]));
        }

        [Fact]
        public void MethodInfoDelegateFunc4() {
            var sut = new ClassUnderTest(this.random.Next(1, 100));
            var args = getArgumentArray(4);

            var func4MethodInfo = sutType.GetMethod("Func4", BindingFlags.Instance | BindingFlags.Public);
            var func4 = (Func<ClassUnderTest, object, object, object, object, object>)
                func4MethodInfo.CreateFunc4<ClassUnderTest>();
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
            var func5 = (Func<ClassUnderTest, object, object, object, object, object, object>)
                func5MethodInfo.CreateFunc5<ClassUnderTest>();
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
            var func6 = (Func<ClassUnderTest, object, object, object, object, object, object, object>)
                func6MethodInfo.CreateFunc6<ClassUnderTest>();
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
            var func7 = (Func<ClassUnderTest, object, object, object, object, object, object, object, object>)
                func7MethodInfo.CreateFunc7<ClassUnderTest>();
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
            var func8 =
                (Func<ClassUnderTest, object, object, object, object, object, object, object, object, object>)
                func8MethodInfo.CreateFunc8<ClassUnderTest>();
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
            var func9 =
                (Func<ClassUnderTest, object, object, object, object, object, object, object, object, object, object>)
                func9MethodInfo.CreateFunc9<ClassUnderTest>();
            Assert.Equal(
                sut.Func9(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
                func9(sut, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8])
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

            public ClassUnderTest(int modifier) {
                this.modifier = modifier;
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
