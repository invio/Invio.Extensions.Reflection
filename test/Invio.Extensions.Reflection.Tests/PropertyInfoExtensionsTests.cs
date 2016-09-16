using System;
using System.Reflection;

using Peddler;
using Xunit;

using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class PropertyInfoExtensionsTests {
        [Fact]
        public void CreateGetter() {
            var readonlyPropertyInfo =
                sutType.GetProperty("ReadonlyProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertGetterValue(readonlyPropertyInfo, sut => sut.ReadonlyProperty, nonPublic: true);
        }

        [Fact]
        public void CreateGetter_ArgNull_Check() {
            PropertyInfo propertyInfo = null;

            AssertGetterException<ArgumentNullException>(propertyInfo);
        }

        [Fact]
        public void CreateGetter_NoGetterProperty() {
            var propertyInfo =
                sutType.GetProperty("NoGetter", BindingFlags.Instance | BindingFlags.Public);

            AssertGetterException<ArgumentException>(propertyInfo);
        }

        [Fact]
        public void CreateGetter_StaticProperty() {
            var propertyInfo =
                sutType.GetProperty("StaticProperty", BindingFlags.Static | BindingFlags.Public);

            AssertGetterException<NotSupportedException>(propertyInfo);
        }

        [Fact]
        public void CreateGetter_PrivateGetter() {
            var propertyInfo =
                sutType.GetProperty("PrivateGetterProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertGetterValue(propertyInfo, _ => ClassUnderTest.PrivateGetterPropertyValue, nonPublic: true);
        }

        [Fact]
        public void CreateGetter_NonPublicGetter() {
            var propertyInfo =
                sutType.GetProperty("PrivateGetterProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertGetterException<ArgumentException>(propertyInfo, nonPublic: false);
        }

        private void AssertGetterValue(
            PropertyInfo propertyInfo,
            Func<ClassUnderTest, Int32> getExpectedValue,
            bool nonPublic = false) {
            var sut = new ClassUnderTest();

            var getterReturnType = propertyInfo.CreateGetter<ClassUnderTest, Int32>(nonPublic);
            Assert.Equal(getExpectedValue(sut), getterReturnType(sut));

            var getterStrongTyped = propertyInfo.CreateGetter<ClassUnderTest>(nonPublic);
            Assert.Equal(getExpectedValue(sut), getterStrongTyped(sut));

            var propertyGetter = propertyInfo.CreateGetter(nonPublic);
            Assert.Equal(getExpectedValue(sut), propertyGetter(sut));
        }

        private void AssertGetterException<TException>(PropertyInfo propertyInfo, bool nonPublic = false)
            where TException : Exception {
            Assert.Throws<TException>(
                () => propertyInfo.CreateGetter<ClassUnderTest, Int32>(nonPublic)
            );

            Assert.Throws<TException>(
                () => propertyInfo.CreateGetter<ClassUnderTest>(nonPublic)
            );

            Assert.Throws<TException>(
                () => propertyInfo.CreateGetter(nonPublic)
            );
        }

        [Fact]
        public void CreateSetter() {
            var propertyInfo =
                sutType.GetProperty("NormalProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertSetterValue(propertyInfo, sut => sut.NormalProperty, nonPublic: true);
        }

        [Fact]
        public void CreateSetter_ArgNull_Check() {
            PropertyInfo propertyInfo = null;

            AssertSetterException<ArgumentNullException>(propertyInfo, nonPublic: false);
        }

        [Fact]
        public void CreateSetter_PrivateSetter() {
            var propertyInfo =
                sutType.GetProperty("PrivateSetterProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertSetterValue(propertyInfo, sut => sut.PrivateSetterProperty, nonPublic: true);
        }

        [Fact]
        public void CreateSetter_NoSetterProperty() {
            var propertyInfo =
                sutType.GetProperty("PrivateSetterProperty", BindingFlags.Instance | BindingFlags.Public);

            AssertSetterException<ArgumentException>(propertyInfo, nonPublic: false);
        }

        [Fact]
        public void CreateSetter_StaticProperty() {
            var propertyInfo =
                sutType.GetProperty("StaticProperty", BindingFlags.Static | BindingFlags.Public);

            AssertSetterException<NotSupportedException>(propertyInfo);
        }

        private static Int32Generator intGenerator = new Int32Generator(1, 10001);
        private void AssertSetterValue(
            PropertyInfo propertyInfo,
            Func<ClassUnderTest, Int32> getValue,
            bool nonPublic = false) {
            var sut = new ClassUnderTest();
            var value = PropertyInfoExtensionsTests.intGenerator.Next();

            var setterParameterType = propertyInfo.CreateSetter<ClassUnderTest, Int32>();
            setterParameterType(sut, value);
            Assert.Equal(value, getValue(sut));

            var setterStrongTyped = propertyInfo.CreateSetter<ClassUnderTest>();
            setterStrongTyped(sut, value);
            Assert.Equal(value, getValue(sut));

            var propertySetter = propertyInfo.CreateSetter();
            propertySetter(sut, value);
            Assert.Equal(value, getValue(sut));
        }

        private void AssertSetterException<TException>(PropertyInfo propertyInfo, bool nonPublic = false)
            where TException : Exception {
            Assert.Throws<TException>(
                () => propertyInfo.CreateSetter<ClassUnderTest, Int32>(nonPublic)
            );

            Assert.Throws<TException>(
                () => propertyInfo.CreateSetter<ClassUnderTest>(nonPublic)
            );

            Assert.Throws<TException>(
                () => propertyInfo.CreateSetter(nonPublic)
            );
        }

        private static Type sutType = typeof(ClassUnderTest);
        class ClassUnderTest {
            public static Int32 StaticProperty { get; set; }
            public Int32 ReadonlyProperty { get; } = 1337;
            public const Int32 PrivateGetterPropertyValue = 7331;
            public Int32 PrivateGetterProperty { private get;  set; } = PrivateGetterPropertyValue;
            public Int32 PrivateSetterProperty { get; private set; }
            public Int32 NormalProperty { get; set; }

            private Int32 noGetter;
            public Int32 NoGetter {
                set {
                    noGetter = value;
                }
            }
        }
    }
}
