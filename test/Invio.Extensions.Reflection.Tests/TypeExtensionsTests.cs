using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [InlineData(typeof(string), typeof(object))]
        [InlineData(typeof(ClassUnderTest<int, string>), typeof(ClassUnderTest<int, string>))]
        [InlineData(typeof(ClassUnderTest<int, string>), typeof(ClassUnderTest<,>))]
        [InlineData(typeof(List<string>), typeof(IList<string>))]
        [InlineData(typeof(List<int>), typeof(IList<>))]
        [InlineData(typeof(IList<int>), typeof(IList<>))]
        [InlineData(typeof(IDictionary<string, int>), typeof(IDictionary<,>))]
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

        [Fact]
        public void GetGenericInterfaceTypes_HasMatch() {
            var result = typeof(HashSet<String>).GetGenericInterfaceTypes(typeof(ISet<>));

            Assert.NotNull(result);
            var resultArray = result.ToArray();
            Assert.Single(resultArray);
            Assert.Equal(typeof(ISet<String>), resultArray[0]);
        }

        [Fact]
        public void GetGenericInterfaceTypes_NoMatch() {
            var result = typeof(List<String>).GetGenericInterfaceTypes(typeof(ISet<>));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetGenericInterfaceTypes_MultipleMatches() {
            var result = typeof(MultipleImplementationClass).GetGenericInterfaceTypes(typeof(ISimpleGenericInterface<>));

            Assert.NotNull(result);
            var resultArray = result.ToArray();
            Assert.Equal(2, resultArray.Length);
            Assert.Contains(typeof(ISimpleGenericInterface<String>), resultArray);
            Assert.Contains(typeof(ISimpleGenericInterface<Int32>), resultArray);
        }

        private class ClassUnderTest<T, U> { }

        private class ChildUnderTest<T> : ClassUnderTest<T, string> {}

        private class GrandchildUnderTest : ChildUnderTest<int> {}

        private interface ISimpleGenericInterface<in T> {
            void Action(T input);
        }

        private class MultipleImplementationClass : ISimpleGenericInterface<String>, ISimpleGenericInterface<Int32> {
            public void Action(String input) {
            }

            public void Action(Int32 input) {
            }
        }
    }

}
