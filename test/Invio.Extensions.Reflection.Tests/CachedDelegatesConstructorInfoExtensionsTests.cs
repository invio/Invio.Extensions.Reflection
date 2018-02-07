using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public sealed class CachedDelegatesConstructorInfoExtensionsTests {

        [Fact]
        public void CreateFunc0_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc0<Fake>()
            );
        }

        [Fact]
        public void CreateFunc0_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc0()
            );
        }

        [Fact]
        public void CreateFunc0_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor();

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc0<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc0_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc0<Fake>()
            );
        }

        [Fact]
        public void CreateFunc0_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc0()
            );
        }

        public static IEnumerable<object[]> CreateFunc0_Data {
            get {
                return ToMemberData<Func<Object>>(
                    constructor => constructor.CreateFunc0(),
                    constructor => constructor.CreateFunc0<Fake>(),
                    constructor => constructor.CreateFunc0<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc0_Data))]
        public void CreateFunc0(Func<ConstructorInfo, Func<Object>> createFunc0) {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = createFunc0(constructor);
            var fake = createFake();

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(Guid.Empty, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(1, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc0(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc1_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc1<Fake>()
            );
        }

        [Fact]
        public void CreateFunc1_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc1()
            );
        }

        [Fact]
        public void CreateFunc1_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 1);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc1<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc1_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc1<Fake>()
            );
        }

        [Fact]
        public void CreateFunc1_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc1()
            );
        }

        [Fact]
        public void CreateFunc1_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 2);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc1<Fake>()
            );
        }


        [Fact]
        public void CreateFunc1_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 2);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc1()
            );
        }

        public static IEnumerable<object[]> CreateFunc1_Data {
            get {
                return ToMemberData<Func<Object, Object>>(
                    constructor => constructor.CreateFunc1(),
                    constructor => constructor.CreateFunc1<Fake>(),
                    constructor => constructor.CreateFunc1<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc1_Data))]
        public void CreateFunc1(Func<ConstructorInfo, Func<Object, Object>> createFunc1) {

            // Arrange
            var guid = Guid.NewGuid();
            var constructor = GetConstructor(typeof(Guid));

            // Act
            var createFake = createFunc1(constructor);
            var fake = createFake(guid);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(1, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc1(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc2_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc2<Fake>()
            );
        }

        [Fact]
        public void CreateFunc2_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc2()
            );
        }

        [Fact]
        public void CreateFunc2_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 2);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc2<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc2_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc2<Fake>()
            );
        }

        [Fact]
        public void CreateFunc2_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc2()
            );
        }

        [Fact]
        public void CreateFunc2_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 3);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc2<Fake>()
            );
        }

        [Fact]
        public void CreateFunc2_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 3);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc2()
            );
        }

        public static IEnumerable<object[]> CreateFunc2_Data {
            get {
                return ToMemberData<Func<Object, Object, Object>>(
                    constructor => constructor.CreateFunc2(),
                    constructor => constructor.CreateFunc2<Fake>(),
                    constructor => constructor.CreateFunc2<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc2_Data))]
        public void CreateFunc2(
            Func<ConstructorInfo, Func<Object, Object, Object>> createFunc2) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(typeof(Guid), typeof(String));

            // Act
            var createFake = createFunc2(constructor);
            var fake = createFake(guid, foo);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(1, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc2(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc3_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc3<Fake>()
            );
        }

        [Fact]
        public void CreateFunc3_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc3()
            );
        }

        [Fact]
        public void CreateFunc3_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 3);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc3<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc3_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 2);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc3<Fake>()
            );
        }

        [Fact]
        public void CreateFunc3_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 2);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc3()
            );
        }

        [Fact]
        public void CreateFunc3_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 4);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc3<Fake>()
            );
        }

        [Fact]
        public void CreateFunc3_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 4);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc3()
            );
        }

        public static IEnumerable<object[]> CreateFunc3_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc3(),
                    constructor => constructor.CreateFunc3<Fake>(),
                    constructor => constructor.CreateFunc3<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc3_Data))]
        public void CreateFunc3(
            Func<ConstructorInfo, Func<Object, Object, Object, Object>> createFunc3) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int)
            );

            // Act
            var createFake = createFunc3(constructor);
            var fake = createFake(guid, foo, 2);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc3(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc4_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc4<Fake>()
            );
        }

        [Fact]
        public void CreateFunc4_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc4()
            );
        }

        [Fact]
        public void CreateFunc4_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 4);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc4<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc4_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 3);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc4()
            );
        }

        [Fact]
        public void CreateFunc4_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 3);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc4()
            );
        }

        [Fact]
        public void CreateFunc4_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 5);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc4<Fake>()
            );
        }

        [Fact]
        public void CreateFunc4_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 5);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc4()
            );
        }

        public static IEnumerable<object[]> CreateFunc4_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc4(),
                    constructor => constructor.CreateFunc4<Fake>(),
                    constructor => constructor.CreateFunc4<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc4_Data))]
        public void CreateFunc4(
            Func<ConstructorInfo, Func<Object, Object, Object, Object, Object>> createFunc4) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte)
            );

            // Act
            var createFake = createFunc4(constructor);
            var fake = createFake(guid, foo, 2, (byte)2);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc4(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc5_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc5<Fake>()
            );
        }

        [Fact]
        public void CreateFunc5_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc5()
            );
        }

        [Fact]
        public void CreateFunc5_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 5);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc5<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc5_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 4);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc5<Fake>()
            );
        }

        [Fact]
        public void CreateFunc5_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 4);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc5()
            );
        }

        [Fact]
        public void CreateFunc5_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc5<Fake>()
            );
        }

        [Fact]
        public void CreateFunc5_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc5()
            );
        }

        public static IEnumerable<object[]> CreateFunc5_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc5(),
                    constructor => constructor.CreateFunc5<Fake>(),
                    constructor => constructor.CreateFunc5<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc5_Data))]
        public void CreateFunc5(
            Func<ConstructorInfo,
                Func<Object, Object, Object, Object, Object, Object>> createFunc5) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte),
                typeof(sbyte)
            );

            // Act
            var createFake = createFunc5(constructor);
            var fake = createFake(guid, foo, 2, (byte)2, (sbyte)2);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(2, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc5(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc6_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc6<Fake>()
            );
        }

        [Fact]
        public void CreateFunc6_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc6()
            );
        }

        [Fact]
        public void CreateFunc6_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 6);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc6<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc6_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 5);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc6<Fake>()
            );
        }

        [Fact]
        public void CreateFunc6_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 5);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc6()
            );
        }

        [Fact]
        public void CreateFunc6_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 7);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc6<Fake>()
            );
        }

        [Fact]
        public void CreateFunc6_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 7);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc6()
            );
        }

        public static IEnumerable<object[]> CreateFunc6_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc6(),
                    constructor => constructor.CreateFunc6<Fake>(),
                    constructor => constructor.CreateFunc6<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc6_Data))]
        public void CreateFunc6(
            Func<ConstructorInfo,
                Func<Object, Object, Object, Object, Object, Object, Object>> createFunc6) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte),
                typeof(sbyte),
                typeof(short)
            );

            // Act
            var createFake = createFunc6(constructor);
            var fake = createFake(guid, foo, 2, (byte)2, (sbyte)2, (short)2);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(2, casted.SByte);
            Assert.Equal(2, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc6(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc7_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc7<Fake>()
            );
        }

        [Fact]
        public void CreateFunc7_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc7()
            );
        }

        [Fact]
        public void CreateFunc7_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 7);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc7<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc7_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc7<Fake>()
            );
        }

        [Fact]
        public void CreateFunc7_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 6);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc7()
            );
        }

        [Fact]
        public void CreateFunc7_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 8);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc7<Fake>()
            );
        }

        [Fact]
        public void CreateFunc7_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 8);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc7()
            );
        }

        public static IEnumerable<object[]> CreateFunc7_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc7(),
                    constructor => constructor.CreateFunc7<Fake>(),
                    constructor => constructor.CreateFunc7<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc7_Data))]
        public void CreateFunc7(
            Func<ConstructorInfo,
                Func<Object, Object, Object, Object, Object, Object, Object, Object>> createFunc7) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort)
            );

            // Act
            var createFake = createFunc7(constructor);
            var fake = createFake(guid, foo, 2, (byte)2, (sbyte)2, (short)2, (ushort)2);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(2, casted.SByte);
            Assert.Equal(2, casted.Short);
            Assert.Equal(2, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc7(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc8_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc8<Fake>()
            );
        }

        [Fact]
        public void CreateFunc8_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc8()
            );
        }

        [Fact]
        public void CreateFunc8_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 8);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc8<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc8_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 7);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc8<Fake>()
            );
        }

        [Fact]
        public void CreateFunc8_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 7);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc8()
            );
        }

        [Fact]
        public void CreateFunc8_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 9);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc8<Fake>()
            );
        }

        [Fact]
        public void CreateFunc8_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 9);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc8()
            );
        }

        public static IEnumerable<object[]> CreateFunc8_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc8(),
                    constructor => constructor.CreateFunc8<Fake>(),
                    constructor => constructor.CreateFunc8<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc8_Data))]
        public void CreateFunc8_Typed(
            Func<ConstructorInfo,
                Func<Object, Object, Object, Object, Object, Object, Object, Object, Object>> createFunc8) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(long)
            );

            // Act
            var createFake = createFunc8(constructor);
            var fake = createFake(
                guid,
                foo,
                2,
                (byte)2,
                (sbyte)2,
                (short)2,
                (ushort)2,
                (long)2
            );

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(2, casted.SByte);
            Assert.Equal(2, casted.Short);
            Assert.Equal(2, casted.UShort);
            Assert.Equal(2L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc8(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateFunc9_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc9<Fake>()
            );
        }

        [Fact]
        public void CreateFunc9_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateFunc9()
            );
        }

        [Fact]
        public void CreateFunc9_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor(numberOfParameters: 9);

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc9<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        [Fact]
        public void CreateFunc9_Typed_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 8);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc9<Fake>()
            );
        }

        [Fact]
        public void CreateFunc9_Untyped_TooFewParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 8);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc9()
            );
        }

        [Fact]
        public void CreateFunc9_Typed_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 10);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc9<Fake>()
            );
        }

        [Fact]
        public void CreateFunc9_Untyped_TooManyParameters() {

            // Arrange
            var constructor = GetConstructor(numberOfParameters: 10);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => constructor.CreateFunc9()
            );
        }

        public static IEnumerable<object[]> CreateFunc9_Data {
            get {
                return ToMemberData<Func<Object, Object, Object, Object, Object, Object, Object, Object, Object, Object>>(
                    constructor => constructor.CreateFunc9(),
                    constructor => constructor.CreateFunc9<Fake>(),
                    constructor => constructor.CreateFunc9<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateFunc9_Data))]
        public void CreateFunc9_Typed(
            Func<ConstructorInfo,
                Func<Object, Object, Object, Object, Object, Object, Object, Object, Object, Object>> createFunc9) {

            // Arrange
            var guid = Guid.NewGuid();
            var foo = "foo";
            var constructor = GetConstructor(
                typeof(Guid),
                typeof(String),
                typeof(int),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(long),
                typeof(ulong)
            );

            // Act
            var createFake = createFunc9(constructor);
            var fake = createFake(
                guid,
                foo,
                2,
                (byte)2,
                (sbyte)2,
                (short)2,
                (ushort)2,
                (long)2,
                (ulong)2
            );

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(2, casted.Bar);
            Assert.Equal(2, casted.Byte);
            Assert.Equal(2, casted.SByte);
            Assert.Equal(2, casted.Short);
            Assert.Equal(2, casted.UShort);
            Assert.Equal(2L, casted.Long);
            Assert.Equal(2UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createFunc9(constructor)),
                "The created delegate should be cached."
            );
        }

        [Fact]
        public void CreateArrayFunc_Typed_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateArrayFunc<Fake>()
            );
        }

        [Fact]
        public void CreateArrayFunc_Untyped_Null() {

            // Arrange
            ConstructorInfo constructor = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => constructor.CreateArrayFunc()
            );
        }

        [Fact]
        public void CreateArrayFunc_Typed_MismatchedType() {

            // Arrange
            ConstructorInfo constructor = GetConstructor();

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => constructor.CreateArrayFunc<String>()
            );

            // Assert
            Assert.Equal(
                "Type parameter 'String' does not match the 'DeclaringType' " +
                "of the provided 'ConstructorInfo' object." +
                Environment.NewLine + "Parameter name: constructor",
                exception.Message
            );
        }

        public static IEnumerable<object[]> CreateArrayFunc_DelegateThrowsOnNull_Data {
            get {
                yield return new object[] { GetConstructor() };
                yield return new object[] { GetConstructor(typeof(Guid)) };
                yield return new object[] { GetConstructor(typeof(Guid), typeof(String)) };
            }
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_DelegateThrowsOnNull_Data))]
        public void CreateArrayFunc_Typed_DelegateThrowsOnNull(ConstructorInfo constructor) {

            // Act
            var createFake = constructor.CreateArrayFunc<Fake>();

            // Assert
            Assert.Throws<ArgumentNullException>(
                () => createFake(null)
            );
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_DelegateThrowsOnNull_Data))]
        public void CreateArrayFunc_Untyped_DelegateThrowsOnNull(ConstructorInfo constructor) {

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentNullException>(
                () => createFake(null)
            );
        }

        [Fact]
        public void CreateArrayFunc_Typed_ThrowsOnTooFewParameters() {

            // Arrange
            var constructor = GetConstructor(typeof(Guid), typeof(String));

            // Act
            var createFake = constructor.CreateArrayFunc<Fake>();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { Guid.NewGuid() })
            );
        }

        [Fact]
        public void CreateArrayFunc_Untyped_ThrowsOnTooFewParameters() {

            // Arrange
            var constructor = GetConstructor(typeof(Guid), typeof(String));

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { Guid.NewGuid() })
            );
        }

        [Fact]
        public void CreateArrayFunc_Typed_ThrowsOnTooManyParameters() {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = constructor.CreateArrayFunc<Fake>();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { Guid.NewGuid() })
            );
        }

        [Fact]
        public void CreateArrayFunc_Untyped_ThrowsOnTooManyParameters() {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = constructor.CreateArrayFunc();

            // Assert
            Assert.Throws<ArgumentException>(
                () => createFake(new object[] { Guid.NewGuid() })
            );
        }

        public static IEnumerable<object[]> CreateArrayFunc_Data {
            get {
                return ToMemberData<Func<Object[], Object>>(
                    constructor => constructor.CreateArrayFunc(),
                    constructor => constructor.CreateArrayFunc<Fake>(),
                    constructor => constructor.CreateArrayFunc<IFake>()
                );
            }
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_Data))]
        public void CreateArrayFunc_ParameterlessConstructor(
            Func<ConstructorInfo, Func<Object[], Object>> createArrayFunc) {

            // Arrange
            var constructor = GetConstructor();

            // Act
            var createFake = createArrayFunc(constructor);
            var fake = createFake(new object[0]);

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(Guid.Empty, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(1, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createArrayFunc(constructor)),
                "The created delegate should be cached."
            );
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_Data))]
        public void CreateArrayFunc_SingleParameterConstructor(
            Func<ConstructorInfo, Func<Object[], Object>> createArrayFunc) {

            // Arrange
            var guid = Guid.NewGuid();
            var constructor = GetConstructor(typeof(Guid));

            // Act
            var createFake = createArrayFunc(constructor);
            var fake = createFake(new object[] { guid });

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal("Default", casted.Foo);
            Assert.Equal(1, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createArrayFunc(constructor)),
                "The created delegate should be cached."
            );
        }

        [Theory]
        [MemberData(nameof(CreateArrayFunc_Data))]
        public void CreateArrayFunc_ManyParameterConstructor(
            Func<ConstructorInfo, Func<Object[], Object>> createArrayFunc) {

            // Arrange
            var guid = Guid.NewGuid();
            const string foo = "Foo";
            const int bar = 5;
            var constructor = GetConstructor(typeof(Guid), typeof(String), typeof(int));

            // Act
            var createFake = createArrayFunc(constructor);
            var fake = createFake(new object[] { guid, foo, bar });

            // Assert
            Assert.NotNull(fake);
            Assert.IsType<Fake>(fake);

            var casted = (Fake)fake;
            Assert.Equal(guid, casted.Guid);
            Assert.Equal(foo, casted.Foo);
            Assert.Equal(bar, casted.Bar);
            Assert.Equal(1, casted.Byte);
            Assert.Equal(1, casted.SByte);
            Assert.Equal(1, casted.Short);
            Assert.Equal(1, casted.UShort);
            Assert.Equal(1L, casted.Long);
            Assert.Equal(1UL, casted.ULong);

            Assert.True(
                Object.ReferenceEquals(createFake, createArrayFunc(constructor)),
                "The created delegate should be cached."
            );
        }

        private static ConstructorInfo GetConstructor(int numberOfParameters) {
            if (numberOfParameters < 0) {
                throw new ArgumentOutOfRangeException(
                    $"The {nameof(numberOfParameters)}' argument cannot be negative.",
                    nameof(numberOfParameters)
                );
            }

            var types = new Type[numberOfParameters];

            switch (numberOfParameters) {
                case 10:
                    types[9] = typeof(uint);
                    goto case 9;
                case 9:
                    types[8] = typeof(ulong);
                    goto case 8;
                case 8:
                    types[7] = typeof(long);
                    goto case 7;
                case 7:
                    types[6] = typeof(ushort);
                    goto case 6;
                case 6:
                    types[5] = typeof(short);
                    goto case 5;
                case 5:
                    types[4] = typeof(sbyte);
                    goto case 4;
                case 4:
                    types[3] = typeof(byte);
                    goto case 3;
                case 3:
                    types[2] = typeof(int);
                    goto case 2;
                case 2:
                    types[1] = typeof(String);
                    goto case 1;
                case 1:
                    types[0] = typeof(Guid);
                    goto case 0;
                case 0:
                    break;
                default:
                    throw new NotSupportedException(
                        $"The {typeof(Fake).Name} class does not have a " +
                        $"constructor with {numberOfParameters:N0} arguments"
                    );
            }

            return GetConstructor(types);
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

        public class Fake : IFake {

            public Guid Guid { get; } = Guid.Empty;
            public String Foo { get; } = "Default";
            public int Bar { get; } = 1;
            public byte Byte { get; } = 1;
            public sbyte SByte { get; } = 1;
            public short Short { get; } = 1;
            public ushort UShort { get; } = 1;
            public long Long { get; } = 1;
            public ulong ULong { get; } = 1;
            public uint UInt { get; } = 1;

            public Fake() {}

            public Fake(Guid guid) {
                this.Guid = guid;
            }

            public Fake(
                Guid guid,
                String foo) {

                this.Guid = guid;
                this.Foo = foo;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn,
                short shortIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
                this.Short = shortIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn,
                short shortIn,
                ushort ushortIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
                this.Short = shortIn;
                this.UShort = ushortIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn,
                short shortIn,
                ushort ushortIn,
                long longIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
                this.Short = shortIn;
                this.UShort = ushortIn;
                this.Long = longIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn,
                short shortIn,
                ushort ushortIn,
                long longIn,
                ulong ulongIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
                this.Short = shortIn;
                this.UShort = ushortIn;
                this.Long = longIn;
                this.ULong = ulongIn;
            }

            public Fake(
                Guid guid,
                String foo,
                int bar,
                byte byteIn,
                sbyte sbyteIn,
                short shortIn,
                ushort ushortIn,
                long longIn,
                ulong ulongIn,
                uint uintIn) {

                this.Guid = guid;
                this.Foo = foo;
                this.Bar = bar;
                this.Byte = byteIn;
                this.SByte = sbyteIn;
                this.Short = shortIn;
                this.UShort = ushortIn;
                this.Long = longIn;
                this.ULong = ulongIn;
                this.UInt = uintIn;
            }

        }

        public interface IFake {}

        private static IEnumerable<object[]> ToMemberData<TFunc>(
            params Func<ConstructorInfo, TFunc>[] createFuncs) {

            foreach (var createFunc in createFuncs) {
                yield return new object[] { createFunc };
            }
        }

    }

}
