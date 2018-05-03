using System;
using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class ExtentTests {
        [Test]
        public void TestMissingExtent() {
            var missing = TextExtent.Missing;

            Assert.That(missing.IsMissing, Is.True);
            Assert.That(missing.IsEmpty, Is.True);
            Assert.That(missing.IsEmptyOrMissing, Is.True);

            Assert.That(missing.Start, Is.EqualTo(-1));
            Assert.That(missing.End, Is.EqualTo(-1));
            Assert.That(missing.Length, Is.EqualTo(0));

            Assert.That(missing.ToString(), Is.EqualTo("<missing>"));
        }

        [Test]
        public void TestEmptyExtent() {
            var empty = TextExtent.Empty;

            Assert.That(empty.IsMissing, Is.False);
            Assert.That(empty.IsEmpty, Is.True);
            Assert.That(empty.IsEmptyOrMissing, Is.True);

            Assert.That(empty.Start, Is.EqualTo(0));
            Assert.That(empty.End, Is.EqualTo(0));
            Assert.That(empty.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestBoundConstruction() {
            var extent = TextExtent.FromBounds(1,9);

            Assert.That(extent.IsMissing, Is.False);
            Assert.That(extent.IsEmpty, Is.False);
            Assert.That(extent.IsEmptyOrMissing, Is.False);

            Assert.That(extent.Start, Is.EqualTo(1));
            Assert.That(extent.End, Is.EqualTo(9));
            Assert.That(extent.Length, Is.EqualTo(8));
        }

        [Test]
        public void TestInvalidConstructionMinus1StartNonZeroLength() {

            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(()=> new TextExtent(-1, 9));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TextExtent(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TextExtent(-1, -1));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TestInvalidConstructionMinus1StartNegativeLength() {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => new TextExtent(-1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TextExtent(-1, -2));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TestManualEmpty() {

            var extent = TextExtent.FromBounds(0, 0);
            Assert.That(extent.IsEmpty, Is.True);

            extent = new TextExtent(0, 0);
            Assert.That(extent.IsEmpty, Is.True);
        }

        [Test]
        public void TestContains() {
            var extent = TextExtent.FromBounds(1, 10);
            

            // Selbst
            Assert.That(extent.Contains(extent), Is.True);

            Assert.That(extent.Contains(TextExtent.FromBounds(2, 9)), Is.True);
            Assert.That(extent.Contains(TextExtent.FromBounds(1, 1)), Is.True);
            Assert.That(extent.Contains(TextExtent.FromBounds(10, 10)), Is.True);

            Assert.That(extent.Contains(TextExtent.FromBounds(2, 11)), Is.False);
            Assert.That(extent.Contains(TextExtent.FromBounds(0, 9)), Is.False);
            Assert.That(extent.Contains(TextExtent.FromBounds(0, 11)), Is.False);

        }
    }
}