using System;
using System.Reflection;
using Invio.Xunit;
using Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public sealed class AutoImplementedPropertyInfoExtensionsTests {

        [Fact]
        public void IsAutoImplemented_NullProperty() {

            // Arrange

            PropertyInfo property = null;

            // Act

            var exception = Record.Exception(
                () => property.IsAutoImplemented()
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData(nameof(PublicOnlyGetterStatic))]
        [InlineData(nameof(PublicGetterAndSetterStatic))]
        [InlineData(nameof(PublicOnlyGetterInstance))]
        [InlineData(nameof(PublicGetterAndSetterInstance))]
        [InlineData(nameof(PrivateWithInitialAssignment))]
        [InlineData(nameof(PublicWithInitialAssignment))]
        [InlineData(nameof(InternalOnlyGetterStatic))]
        [InlineData(nameof(InternalOnlyGetterInstance))]
        [InlineData(nameof(PrivateOnlyGetterStatic))]
        [InlineData(nameof(PrivateOnlyGetterInstance))]
        public void IsAutoImplemented_AreAutoImplemented(String propertyName) {

            // Arrange

            var property = this.GetProperty(propertyName);

            // Act

            var isAutoImplemented = property.IsAutoImplemented();

            // Assert

            Assert.True(isAutoImplemented);
        }

        [Theory]
        [InlineData(nameof(OnlySetterInstance))]
        [InlineData(nameof(OnlySetterStatic))]
        [InlineData(nameof(PublicManualStaticBackingField))]
        [InlineData(nameof(PublicManualInstanceBackingField))]
        [InlineData(nameof(ManualOnlyGetterStatic))]
        [InlineData(nameof(ManualOnlyGetterInstance))]
        public void IsAutoImplemented_NotAutoImpleted(String propertyName) {

            // Arrange

            var property = this.GetProperty(propertyName);

            // Act

            var isAutoImplemented = property.IsAutoImplemented();

            // Assert

            Assert.False(isAutoImplemented);
        }

        private PropertyInfo GetProperty(string propertyName) {
            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.NonPublic | BindingFlags.Public;

            return this.GetType().GetProperty(propertyName, flags);
        }

        // Test cases for various sanity checks

        public static object PublicOnlyGetterStatic { get; }
        public static object PublicGetterAndSetterStatic { get; }
        public object PublicOnlyGetterInstance { get; }
        public object PublicGetterAndSetterInstance { get; }
        internal static object InternalOnlyGetterStatic { get; }
        internal object InternalOnlyGetterInstance { get; }
        private static object PrivateOnlyGetterStatic { get; }
        private object PrivateOnlyGetterInstance { get; }

        private static object PrivateWithInitialAssignment { get; set; } = new object();
        public object PublicWithInitialAssignment { get; set; } = new object();

        private static object onlySetterStaticBackingField;
        public static object OnlySetterStatic {
            set { onlySetterStaticBackingField = value; }
        }

        private object onlySetterInstanceBackingField;
        public object OnlySetterInstance {
            set { this.onlySetterInstanceBackingField = value; }
        }

        private static object manualOnlyGetterStaticBackingField = new object();
        internal static object ManualOnlyGetterStatic {
            get { return manualOnlyGetterStaticBackingField; }
        }

        private object manualOnlyGetterInstanceBackingField = new object();
        internal object ManualOnlyGetterInstance => this.manualOnlyGetterInstanceBackingField;

        private static object manualStaticBackingField;
        public static object PublicManualStaticBackingField {
            get { return manualStaticBackingField; }
            set { manualStaticBackingField = value; }
        }

        private object manualInstanceBackingField;
        public object PublicManualInstanceBackingField {
            get { return this.manualInstanceBackingField; }
            set { this.manualInstanceBackingField = value; }
        }

    }

}
