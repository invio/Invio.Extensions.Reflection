using System;
using System.Reflection;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public sealed class CachedDelegatesFieldInfoExtensionsTests {

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
        public void CreateGetter_BothTyped_InvalidContravariantDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<FakeBase, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'FakeBase', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateGetter_BothTyped_InvalidContravariantFieldType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.FakeField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<Fake, FakeBaseField>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TField' was 'FakeBaseField', which is not " +
                "assignable to the field's value type of 'FakeField'." +
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
        public void CreateGetter_BothTyped_ValidCovariantDeclaringType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));

            // Act

            var getter = field.CreateGetter<Fake, FakeBaseField>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateGetter_BothTyped_ValidCovariantFieldType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));

            // Act

            var getter = field.CreateGetter<FakeBase, FakeField>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateGetter_BothTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getterOne = fieldOne.CreateGetter<Fake, String>();
            var getterTwo = fieldTwo.CreateGetter<Fake, String>();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
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
        public void CreateSetter_BothTyped_InvalidContravariantDeclaringType() {

            // Arrangen

            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<FakeBase, String>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'FakeBase', which is not " +
                "assignable to the field's declaring type of 'Fake'." +
                Environment.NewLine + "Parameter name: field",
                exception.Message
            );
        }

        [Fact]
        public void CreateSetter_BothTyped_InvalidContravariantFieldType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.FakeField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<Fake, FakeBaseField>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TField' was 'FakeBaseField', which is not " +
                "assignable to the field's value type of 'FakeField'." +
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
        public void CreateSetter_BothTyped_ValidCovariantDeclaringType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));
            var value = new FakeField();

            // Act

            var setter = field.CreateSetter<Fake, FakeBaseField>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateSetter_BothTyped_ValidCovariantFieldType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));
            var value = new FakeField();

            // Act

            var setter = field.CreateSetter<FakeBase, FakeField>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateSetter_BothTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var setterOne = fieldOne.CreateSetter<Fake, String>();
            var setterTwo = fieldTwo.CreateSetter<Fake, String>();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
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
        public void CreateGetter_BaseTyped_InvalidContravariantDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var exception = Record.Exception(
                () => field.CreateGetter<FakeBase>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'FakeBase', which is not " +
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
        public void CreateGetter_BaseTyped_ValidCovariantDeclaringType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));

            // Act

            var getter = field.CreateGetter<Fake>();
            var value = getter(instance);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateGetter_BaseTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getterOne = fieldOne.CreateGetter<Fake>();
            var getterTwo = fieldTwo.CreateGetter<Fake>();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
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
        public void CreateSetter_BaseTyped_InvalidContravariantDeclaringType() {

            // Arrange

            var field = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var exception = Record.Exception(
                () => field.CreateSetter<FakeBase>()
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                "Type parameter 'TBase' was 'FakeBase', which is not " +
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
        public void CreateSetter_BaseTyped_ValidCovariantDeclaringType() {

            // Arrange

            var instance = new Fake();
            var field = typeof(FakeBase).GetField(nameof(FakeBase.FakeBaseField));
            var value = new FakeField();

            // Act

            var setter = field.CreateSetter<Fake>();
            setter(instance, value);

            // Assert

            Assert.Equal(instance.FakeBaseField, value);
        }

        [Fact]
        public void CreateSetter_BaseTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var setterOne = fieldOne.CreateSetter<Fake>();
            var setterTwo = fieldTwo.CreateSetter<Fake>();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
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
        public void CreateGetter_NeitherTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var getterOne = fieldOne.CreateGetter();
            var getterTwo = fieldTwo.CreateGetter();

            // Assert

            Assert.True(Object.ReferenceEquals(getterOne, getterTwo));
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

        [Fact]
        public void CreateSetter_NeitherTyped_Caches() {

            // Arrange

            var fieldOne = typeof(Fake).GetField(nameof(Fake.InstanceField));
            var fieldTwo = typeof(Fake).GetField(nameof(Fake.InstanceField));

            // Act

            var setterOne = fieldOne.CreateSetter();
            var setterTwo = fieldTwo.CreateSetter();

            // Assert

            Assert.True(Object.ReferenceEquals(setterOne, setterTwo));
        }

        private class Fake : FakeBase {

            public static String StaticField = "NotSupported";
            public String InstanceField = "Supported";

            public FakeField FakeField = new FakeField();

        }

        public abstract class FakeBase {

            public FakeBaseField FakeBaseField;

        }

        private class FakeField : FakeBaseField {}

        public abstract class FakeBaseField {}

    }

}
