using System;
using System.Reflection;

using Peddler;
using Xunit;

using Invio.Xunit;

namespace Invio.Extensions.Reflection {

    [UnitTest]
    public class FieldInfoExtensionsTests {

        [Fact]
        public void CreateGetter() {
            var sut = new ClassUnderTest();

            var secretFieldInfo = sutType.GetField("secret", BindingFlags.Instance | BindingFlags.NonPublic);
            var secretGetter = secretFieldInfo.CreateGetter<ClassUnderTest>();
            Assert.Equal(sut.SecretRevealed, secretGetter(sut));
        }

        [Fact]
        public void CreateGetter_ArgNull_Check() {
            FieldInfo nullFieldInfo = null;

            Assert.Throws<ArgumentNullException>(
                () => nullFieldInfo.CreateGetter<ClassUnderTest>()
            );
        }

        [Fact]
        public void CreateGetter_Static_Field() {
            var staticFieldInfo = sutType.GetField("staticField", BindingFlags.Static | BindingFlags.NonPublic);

            Assert.Throws<NotSupportedException>(
                () => staticFieldInfo.CreateGetter<ClassUnderTest>()
            );
        }

        [Fact]
        public void CreateSetter() {
            var sut = new ClassUnderTest();

            var secretFieldInfo = sutType.GetField("secret", BindingFlags.Instance | BindingFlags.NonPublic);
            var secretSetter = secretFieldInfo.CreateSetter<ClassUnderTest>();
            var secretHaxed = "All you hax belongs to us.";
            secretSetter(sut, secretHaxed);
            Assert.Equal(secretHaxed, sut.SecretRevealed);
        }

        [Fact]
        public void CreateSetter_ArgNull_Check() {
            FieldInfo nullFieldInfo = null;

            Assert.Throws<ArgumentNullException>(
                () => nullFieldInfo.CreateSetter<ClassUnderTest>()
            );
        }

        [Fact]
        public void CreateSetter_Static_Field() {
            var staticFieldInfo = sutType.GetField("staticField", BindingFlags.Static | BindingFlags.NonPublic);

            Assert.Throws<NotSupportedException>(
                () => staticFieldInfo.CreateSetter<ClassUnderTest>()
            );
        }

        private static Type sutType = typeof(ClassUnderTest);
        class ClassUnderTest {
            private static String staticField = "NotSupported";
            public static String StaticFieldRevealed { get { return staticField; } }

            private String secret = "HaxMePlz";
            public String SecretRevealed { get { return secret; } }
        }
    }
}
