#region Using Directives

using System;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class TextChangeWriterTests {

        [Test]
        public void IsEmpty() {

            Assert.That(TextChange.Empty.IsEmpty, Is.True);
        }

        [Test]
        public void Empty() {

            Assert.That(TextChange.Empty.Extent.Start,    Is.EqualTo(0));
            Assert.That(TextChange.Empty.Extent.End,      Is.EqualTo(0));
            Assert.That(TextChange.Empty.ReplacementText, Is.EqualTo(String.Empty));
        }

        [Test]
        public void SimpleDelete() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewRemove(start: 0, length: 6),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max!"));
        }

        [Test]
        public void DeleteWithReplace() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewRemove(start: 0, length: 6),
                TextChange.NewReplace(start: 9, length: 1, text: "?")
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max?"));
        }

        [Test]
        public void InsertWithReplace() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewReplace(start: 5, length: 0, text: "o"),
                TextChange.NewReplace(start: 9, length: 1, text: "?")
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Halloo Max?"));
        }

        [Test]
        public void DeleteWithReplaceReversed() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewReplace(start: 9, length: 1, text: "?"),
                TextChange.NewRemove(start: 0, length: 6)
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max?"));
        }

        [Test]
        public void OverlappingChanges() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewRemove(start: 3, length: 4),
                TextChange.NewReplace(start: 0, length: 5, text: "?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            Assert.Throws<ArgumentException>(() => writer.ApplyTextChanges(text, textChanges));
        }

        [Test]
        public void ReplaceWithChangesOutOfIndex() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewReplace(start: 10, length: 1, text: "?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            Assert.Throws<ArgumentOutOfRangeException>(() => writer.ApplyTextChanges(text, textChanges));
        }

        [Test]
        public void SimpleInsert() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewReplace(start: 10, length: 0, text: " Wie geht es dir?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Hallo Max! Wie geht es dir?"));
        }

        [Test]
        public void Replacement() {

            string text = "Hallo Max!";

            var textChanges = new[] {
                TextChange.NewReplace(start: 6, length: 4, text: "Moritz! Wie geht es dir?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Hallo Moritz! Wie geht es dir?"));
        }

    }

}