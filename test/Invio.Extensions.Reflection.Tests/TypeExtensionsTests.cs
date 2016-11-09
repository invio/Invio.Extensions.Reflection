using System;
using Xunit;
using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class TypeExtensionsTests {

        [Fact]
        public void GetNameWithGenericParameters_ArgNull_Checks() {
            Type nullType = null;

            Assert.Throws<ArgumentNullException>(
                () => nullType.GetNameWithGenericParameters()
            );
        }

        [Fact]
        public void GetNameWithGenericParameters_SimpleType() {
            var type = typeof(String);

            Assert.Equal("String", type.GetNameWithGenericParameters());
        }

        [Fact]
        public void GetNameWithGenericParameters_ComplexType_NoTypes() {
            var type = typeof(ClassUnderTest<,>);

            Assert.Equal(
                "ClassUnderTest`2",
                type.GetNameWithGenericParameters()
            );
        }

        [Fact]
        public void GetNameWithGenericParameters_ComplexType_SimpleTypes() {
            var type = typeof(ClassUnderTest<Object, Object>);

            Assert.Equal(
                "ClassUnderTest`2<System.Object, System.Object>",
                type.GetNameWithGenericParameters()
            );
        }

        private class ClassUnderTest<T, U> { }

    }

}
