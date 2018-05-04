using NUnit.Framework;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class VersionStampTests {

        [Test]
        public void TestEqual() {
            var s1 = VersionStamp.Create();
            var s2 = s1;

            Assert.That(s1, Is.EqualTo(s1));

            Assert.That(s1 != s2, Is.False);
            Assert.That(s2 != s1, Is.False);

            Assert.That(s1 == s2, Is.True);
            Assert.That(s2 == s1, Is.True);

            Assert.That(s1 < s2,  Is.False);
            Assert.That(s1 <= s2, Is.True);
            Assert.That(s2 > s1,  Is.False);
            Assert.That(s2 >= s1, Is.True);

            Assert.That(s1 > s2,  Is.False);
            Assert.That(s1 >= s2, Is.True);
            Assert.That(s2 < s1,  Is.False);
            Assert.That(s2 <= s1, Is.True);
        }

        [Test]
        public void TestCreateNewerWithOutDelay() {
            var s1 = VersionStamp.Create();
            var s2 = s1.CreateNewer();

            Assert.That(s1, Is.Not.EqualTo(s2));
        }

        [Test]
        public void TestCreateNewerWithDelay() {
            var s1 = VersionStamp.Create();
            
            Is.True.After(2);

            var s2 = s1.CreateNewer();
            Assert.That(s1, Is.Not.EqualTo(s2));
        }

        [Test]
        public void TestGetNewer() {
            var s1 = VersionStamp.Create();
            var s2 = s1.CreateNewer();

            Assert.That(s2, Is.EqualTo(s1.GetNewer(s2)));
            Assert.That(s2, Is.Not.EqualTo(s1.GetNewer(s1)));

            Assert.That(s1 != s2, Is.True);
            Assert.That(s2 != s1, Is.True);

            Assert.That(s1 == s2, Is.False);
            Assert.That(s2 == s1, Is.False);

            Assert.That(s1 < s2,  Is.True);
            Assert.That(s1 <= s2, Is.True);
            Assert.That(s2 > s1,  Is.True);
            Assert.That(s2 >= s1, Is.True);

            Assert.That(s1 > s2,  Is.False);
            Assert.That(s1 >= s2, Is.False);
            Assert.That(s2 < s1,  Is.False);
            Assert.That(s2 <= s1, Is.False);
        }

    }

}