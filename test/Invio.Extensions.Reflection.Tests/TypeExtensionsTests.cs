using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public sealed class TypeExtensionsTests {

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

        [Fact]
        public void IsDerivativeOf_NullExtendedType() {
            Type type = null;

            Assert.Throws<ArgumentNullException>(
                () => type.IsDerivativeOf(typeof(Object))
            );
        }

        [Fact]
        public void IsDerivativeOf_NullParentType() {
            var type = typeof(string);

            Assert.Throws<ArgumentNullException>(
                () => type.IsDerivativeOf(null)
            );
        }

        [Theory]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(typeof(ClassUnderTest<int, string>), typeof(ClassUnderTest<int, string>))]
        [InlineData(typeof(ClassUnderTest<int, string>), typeof(ClassUnderTest<,>))]
        [InlineData(typeof(List<string>), typeof(IList<string>))]
        [InlineData(typeof(List<int>), typeof(IList<>))]
        [InlineData(typeof(ArgumentNullException), typeof(ArgumentException))]
        [InlineData(typeof(IList), typeof(IEnumerable))]
        [InlineData(typeof(IList<>), typeof(IEnumerable<>))]
        [InlineData(typeof(IList<>), typeof(IEnumerable))]
        [InlineData(typeof(IEnumerable<string>), typeof(IEnumerable<object>))]
        [InlineData(typeof(ChildUnderTest<int>), typeof(ClassUnderTest<int, string>))]
        [InlineData(typeof(ChildUnderTest<>), typeof(ClassUnderTest<,>))]
        [InlineData(typeof(GrandchildUnderTest), typeof(ClassUnderTest<int, string>))]
        [InlineData(typeof(GrandchildUnderTest), typeof(ClassUnderTest<,>))]
        [InlineData(typeof(GrandchildUnderTest), typeof(ChildUnderTest<int>))]
        [InlineData(typeof(GrandchildUnderTest), typeof(ChildUnderTest<>))]
        public void IsDerivativeOf_Matches(Type type, Type parentType) {
            Assert.True(type.IsDerivativeOf(parentType));
        }

        [Theory]
        [InlineData(typeof(object), typeof(string))]
        [InlineData(typeof(ArgumentException), typeof(ArgumentNullException))]
        [InlineData(typeof(IEnumerable<>), typeof(IEnumerable<string>))]
        [InlineData(typeof(IDictionary<,>), typeof(IDictionary<int, string>))]
        [InlineData(typeof(IEnumerable<object>), typeof(IEnumerable<string>))]
        [InlineData(typeof(GrandchildUnderTest), typeof(ChildUnderTest<string>))]
        public void IsDerivativeOf_Mismatches(Type type, Type parentType) {
            Assert.False(type.IsDerivativeOf(parentType));
        }

        private class ClassUnderTest<T, U> { }

        private class ChildUnderTest<T> : ClassUnderTest<T, string> {}

        private class GrandchildUnderTest : ChildUnderTest<int> {}

    }

}
