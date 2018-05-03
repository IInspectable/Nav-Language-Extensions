#region Using Directives

using System;
using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class TextChangeWriterTests {

        [Test]
        public void SimpleDelete() {

            string text = "Hallo Max!";

            var textChanges = new []{
                NewTextChange(0, 6, ""),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max!"));
        }

        [Test]
        public void DeleteWithReplace() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(0, 6, ""),
                NewTextChange(9, 1, "?")
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max?"));
        }

        [Test]
        public void InsertWithReplace() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(5, 0, "o"),
                NewTextChange(9, 1, "?")
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Halloo Max?"));
        }

        [Test]
        public void DeleteWithReplaceReversed() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(9, 1, "?"),
                NewTextChange(0, 6, "")                
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Max?"));
        }
        
        [Test]
        public void OverlappingChanges() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(3, 4, ""),
                NewTextChange(0, 5, "?"),                
            };

            TextChangeWriter writer = new TextChangeWriter();

            Assert.Throws<ArgumentException>(()=> writer.ApplyTextChanges(text, textChanges));
        }

        [Test]
        public void ReplaceWithChangesOutOfIndex() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(10, 1, "?"),                
            };

            TextChangeWriter writer = new TextChangeWriter();

            Assert.Throws<ArgumentOutOfRangeException>(() => writer.ApplyTextChanges(text, textChanges));
        }

        [Test]
        public void SimpleInsert() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(10, 0, " Wie geht es dir?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Hallo Max! Wie geht es dir?"));
        }

        [Test]
        public void Replacement() {

            string text = "Hallo Max!";

            var textChanges = new[]{
                NewTextChange(6, 4, "Moritz! Wie geht es dir?"),
            };

            TextChangeWriter writer = new TextChangeWriter();

            var result = writer.ApplyTextChanges(text, textChanges);

            Assert.That(result, Is.EqualTo("Hallo Moritz! Wie geht es dir?"));
        }

        TextChange NewTextChange(int start, int length, string text) {
            return TextChange.NewReplace(new TextExtent( start, length), text);
        }
    }    
}