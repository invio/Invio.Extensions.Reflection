using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Invio.Extensions.Reflection {

    public class ConstructorInfoExtensionsTests {

        [Fact]
        public void CreateArrayFunc_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateArrayFunc()
            );
        }

        public static IEnumerable<object[]> CreateArrayFunc_DelegateThrowsOnNull_Data {
            get {
                yield return new object[] { GetConstructor() };
                yield return new object[] { GetConstructor(typeof(Guid)) };
                yield return new object[] { GetConstructor(typeof(String), typeof(int)) };
            }
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_DelegateThrowsOnNull_Data))]
        public void CreateArrayFunc_DelegateThrowsOnNull(ConstructorInfo constructor) {

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentNullException>(
                () => createFake(null)
            );
        }

        [Fact]
        public void CreateArrayFunc_ThrowsOnTooFewParameters() {

            // Arrange
            var constructor = GetConstructor(typeof(String), typeof(int));

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { "Foo" })
            );
        }

        [Fact]
        public void CreateArrayFunc_ThrowsOnTooManyParameters() {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { Guid.NewGuid() })
            );
        }

        [Fact]
        public void CreateArrayFunc_ParameterlessConstructor() {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = constructor.CreateArrayFunc();
            var fake = createFake(new object[0]);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(Guid.Empty, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(5, casted.Bar);
        }

        [Fact]
        public void CreateArrayFunc_SingleParameterConstructor() {

            // Arrange
            var guid = Guid.NewGuid();
            var constructor = GetConstructor(typeof(Guid));

            // Act
            var createFake = constructor.CreateArrayFunc();
            var fake = createFake(new object[] { guid });

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(5, casted.Bar);
        }

        [Fact]
        public void CreateArrayFunc_ManyParameterConstructor() {

            // Arrange
            const string foo = "Foo";
            const int bar = 5;
            var constructor = GetConstructor(typeof(String), typeof(int));

            // Act
            var createFake = constructor.CreateArrayFunc();
            var fake = createFake(new object[] { foo, bar });

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(Guid.Empty, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(bar, casted.Bar);
        }

        private static ConstructorInfo GetConstructor(params Type[] types) {
            if (types == null) {
                throw new ArgumentNullException(nameof(types));
            }

            if (types.Any(t => t == null)) {
                throw new ArgumentException(
                    $"One or more of the provided {typeof(Type).Name} arguments was null.",
                    nameof(types)
                );
            }

            ConstructorInfo constructor;

            if (types.Length == 0) {
                constructor = typeof(Fake).GetConstructor(Type.EmptyTypes);
            } else {
                constructor = typeof(Fake).GetConstructor(types);
            }

            if (constructor == null) {
                throw new ArgumentException(
                    $@"Unable to find a constructor on the '{typeof(Fake).Name}' class with " +
                    $@"the types {String.Join("", "", types.Select(t => t.Name).ToArray())}.",
                    nameof(types)
                );
            }

            return constructor;
        }

        public class Fake {

            public Guid Guid { get; } = Guid.Empty;
            public String Foo { get; } = "Default";
            public int Bar { get; } = 5;

            public Fake() {}

            public Fake(Guid guid) {
                this.Guid = guid;
            }

            public Fake(String foo, int bar) {
                this.Foo = foo;
                this.Bar = bar;
            }

        }

    }

}
