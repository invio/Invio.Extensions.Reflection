using System;
using System.Reflection;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class FieldInfoExtensionsTests {

        [Fact]
        public void CreateGetter_BothTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_BothTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_InvalidDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<String, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_InvalidFieldType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake, Guid>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TField' was 'Guid', which is not " +
                "assignable to the field's value type of 'String'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getter = field.CreateGetter<Fake, String>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.InstanceField, value);
        }

        [Fact]
        public void CreateSetter_BothTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_BothTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_InvalidDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<String, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_InvalidFieldType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake, Guid>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TField' was 'Guid', which is not " +
                "assignable to the field's value type of 'String'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var value = "Updated";

            // Act

            var setter = field.CreateSetter<Fake, String>();
            setter(instance, value);

            // Assert

            Assert.Equal(value, instance.InstanceField);
        }

        [Fact]
        public void CreateGetter_BaseTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_BaseTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BaseTyped_InvalidDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BaseTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getter = field.CreateGetter<Fake>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.InstanceField, value);
        }

        [Fact]
        public void CreateSetter_BaseTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_BaseTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BaseTyped_InvalidDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'String', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BaseTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var value = "Updated";

            // Act

            var setter = field.CreateSetter<Fake>();
            setter(instance, value);

            // Assert

            Assert.Equal(value, instance.InstanceField);
        }

        [Fact]
        public void CreateGetter_NeitherTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateGetter_NeitherTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_NeitherTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getter = field.CreateGetter();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.InstanceField, value);
        }

        [Fact]
        public void CreateSetter_NeitherTyped_Null() {

            // Arrange

            FieldInfo field = null;

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void CreateSetter_NeitherTyped_StaticField() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.StaticField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"The '{field.Name}' field is static." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_NeitherTyped() {

            // Arrange

            var instance = new Fake();
            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var value = "Updated";

            // Act

            var setter = field.CreateSetter();
            setter(instance, value);

            // Assert

            Assert.Equal(value, instance.InstanceField);
        }

        private class Fake {

            public static String StaticField = "NotSupported";
            public String InstanceField = "Supported";

        }

    }

}
