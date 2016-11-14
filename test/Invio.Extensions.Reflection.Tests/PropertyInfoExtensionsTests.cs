using System;
using System.Reflection;
using Peddler;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class PropertyInfoExtensionsTests {

        // CreateGetter<TBase, TProperty>()

        [Fact]
        public void CreateGetter_BothTyped_PublicGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getter = property.CreateGetter<Fake, Int32>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateGetter_BothTyped_PrivateGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateGetter));

            // Act

            var getter = property.CreateGetter<Fake, Int32>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.PrivateGetterValue, value);
        }

        [Fact]
        public void CreateGetter_BothTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getterOne = propertyOne.CreateGetter<Fake, Int32>();
            var getterTwo = propertyTwo.CreateGetter<Fake, Int32>();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
        }

        [Fact]
        public void CreateGetter_BothTyped_NoGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoGetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoGetter' property on 'Fake' does not have a get accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_BothTyped_InvalidDeclaringType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<String, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the property's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_InvalidPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TProperty' was 'String', which is not " +
                "assignable to the property's value type of 'Int32'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        // CreateSetter<TBase, TProperty>()

        [Fact]
        public void CreateSetter_BothTyped_PublicSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var value = 100;

            // Act

            var setter = property.CreateSetter<Fake, Int32>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateSetter_BothTyped_PrivateSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateSetter));
            var value = 100;

            // Act

            var setter = property.CreateSetter<Fake, Int32>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.PrivateSetter, value);
        }

        [Fact]
        public void CreateSetter_BothTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var setterOne = propertyOne.CreateSetter<Fake, Int32>();
            var setterTwo = propertyTwo.CreateSetter<Fake, Int32>();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
        }

        [Fact]
        public void CreateSetter_BothTyped_NoSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoSetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoSetter' property on 'Fake' does not have a set accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_BothTyped_InvalidDeclaringType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<String, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the property's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_InvalidPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TProperty' was 'String', which is not " +
                "assignable to the property's value type of 'Int32'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake, Int32>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        // CreateGetter<TBase>()

        [Fact]
        public void CreateGetter_BaseTyped_PublicGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getter = property.CreateGetter<Fake>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateGetter_BaseTyped_PrivateGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateGetter));

            // Act

            var getter = property.CreateGetter<Fake>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.PrivateGetterValue, value);
        }

        [Fact]
        public void CreateGetter_BaseTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getterOne = propertyOne.CreateGetter<Fake>();
            var getterTwo = propertyTwo.CreateGetter<Fake>();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
        }

        [Fact]
        public void CreateGetter_BaseTyped_NoGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoGetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoGetter' property on 'Fake' does not have a get accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BaseTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_BaseTyped_InvalidDeclaringType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the property's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BaseTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        // CreateSetter<TBase>()

        [Fact]
        public void CreateSetter_BaseTyped_PublicSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var value = 100;

            // Act

            var setter = property.CreateSetter<Fake>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateSetter_BaseTyped_PrivateSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateSetter));
            var value = 100;

            // Act

            var setter = property.CreateSetter<Fake>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.PrivateSetter, value);
        }

        [Fact]
        public void CreateSetter_BaseTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var setterOne = propertyOne.CreateSetter<Fake>();
            var setterTwo = propertyTwo.CreateSetter<Fake>();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
        }

        [Fact]
        public void CreateSetter_BaseTyped_NoSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoSetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoSetter' property on 'Fake' does not have a set accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BaseTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_BaseTyped_InvalidDeclaringType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the property's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BaseTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        // CreateGetter()

        [Fact]
        public void CreateGetter_NeitherTyped_PublicGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getter = property.CreateGetter();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateGetter_NeitherTyped_PrivateGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateGetter));

            // Act

            var getter = property.CreateGetter();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.PrivateGetterValue, value);
        }

        [Fact]
        public void CreateGetter_NeitherTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var getterOne = propertyOne.CreateGetter();
            var getterTwo = propertyTwo.CreateGetter();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
        }

        [Fact]
        public void CreateGetter_NeitherTyped_NoGetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoGetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoGetter' property on 'Fake' does not have a get accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_NeitherTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_NeitherTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateGetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        // CreateSetter()

        [Fact]
        public void CreateSetter_NeitherTyped_PublicSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var value = 100;

            // Act

            var setter = property.CreateSetter();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.NormalProperty, value);
        }

        [Fact]
        public void CreateSetter_NeitherTyped_PrivateSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.PrivateSetter));
            var value = 100;

            // Act

            var setter = property.CreateSetter();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.PrivateSetter, value);
        }

        [Fact]
        public void CreateSetter_NeitherTyped_Caches() {

            // Arrange

            var propertyOne = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));
            var propertyTwo = typeof(Fake).GetProperty(nameof(Fake.NormalProperty));

            // Act

            var setterOne = propertyOne.CreateSetter();
            var setterTwo = propertyTwo.CreateSetter();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
        }

        [Fact]
        public void CreateSetter_NeitherTyped_NoSetter() {

            // Arrange

            var instance = new Fake();
            var property = typeof(Fake).GetProperty(nameof(Fake.NoSetter));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'NoSetter' property on 'Fake' does not have a set accessor." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_NeitherTyped_Null() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_NeitherTyped_StaticPropertyType() {

            // Arrange

            var property = typeof(Fake).GetProperty(nameof(Fake.StaticProperty));

            // Act

            var exception = Record.Exception(
                () => property.CreateSetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "The 'StaticProperty' property is static." +
                Environment.NewLine + "Parameter name: property",
                exception.Message
            );
        }

        private class Fake {

            public static Int32 StaticProperty { get; set; }

            public Int32 NormalProperty { get; set; }

            public Int32 PrivateGetterValue { get { return this.PrivateGetter; } }
            public Int32 PrivateGetter { private get; set; } = 7331;
            public Int32 PrivateSetter { get; private set; } = 1337;

            public Int32 NoGetterValue;
            public Int32 NoGetter { set { this.NoGetterValue = value; } }
            public Int32 NoSetter { get; } = 1337;

        }

    }

}
