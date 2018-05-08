using NUnit.Framework;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class SourceTextTests {

        [Test]
        public void TestEmpty() {

            SourceText st = SourceText.Empty;

            Assert.That(st.Text,            Is.EqualTo(string.Empty));
            Assert.That(st.Length,          Is.EqualTo(0));
            Assert.That(st.FileInfo,        Is.Null);
            Assert.That(st.TextLines.Count, Is.EqualTo(1));

            var tl = st.GetTextLineAtPosition(0);

            Assert.That(tl.ToString(), Is.EqualTo(string.Empty));

        }

        [Test]
        public void TestSingleLine() {
            const string testText = "hello There!";

            SourceText st = SourceText.From(testText);

            Assert.That(st.Text,            Is.EqualTo(testText));
            Assert.That(st.Length,          Is.EqualTo(testText.Length));
            Assert.That(st.FileInfo,        Is.Null);
            Assert.That(st.TextLines.Count, Is.EqualTo(1));

            var tl = st.GetTextLineAtPosition(0);

            Assert.That(tl.ToString(), Is.EqualTo(testText));
        }

        [Test]
        public void TestLineAndEmptyLine() {
            const string testText = "hello There!\r\n";

            SourceText st = SourceText.From(testText);

            Assert.That(st.Text,            Is.EqualTo(testText));
            Assert.That(st.Length,          Is.EqualTo(testText.Length));
            Assert.That(st.FileInfo,        Is.Null);
            Assert.That(st.TextLines.Count, Is.EqualTo(2));


            Assert.That(st.TextLines[0].ToString(), Is.EqualTo("hello There!\r\n"));
            Assert.That(st.TextLines[1].ToString(), Is.EqualTo(""));
        }

        [Test]
        public void TextLinesTest() {
            var syntaxTree = SyntaxTree.ParseText(Resources.LargeNav);

            int expectedLine = 0;
            int currentEnd   = 0;

            foreach (var lineExtent in syntaxTree.SourceText.TextLines) {
                // Keine Zeilensprünge
                Assert.That(lineExtent.Line, Is.EqualTo(expectedLine));
                // Lückenlosigkeit
                Assert.That(lineExtent.Start, Is.EqualTo(currentEnd));

                expectedLine++;
                currentEnd = lineExtent.End;
            }
            Assert.That(currentEnd, Is.EqualTo(Resources.LargeNav.Length));
        }

    }

}