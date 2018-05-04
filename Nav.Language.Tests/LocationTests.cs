using System.Linq;
using NUnit.Framework;
using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class LocationTests {

        [Test]
        public void TrivialLineNumberTest() {

            var syntax = Syntax.ParseTaskDefinition("task F {}");

            var loc=syntax.GetLocation();
            Assert.That(loc.LineRange.Start.Line, Is.EqualTo(0));
            Assert.That(loc.LineRange.End.Line, Is.EqualTo(0));
        }

        [Test]
        public void MultiLineNumberTest() {

            var syntax = Syntax.ParseCodeGenerationUnit(
                    "task T1 {}\r\n"          + // 0
                    "/* Multiline\r\n"        + // 1
                    " Comment */\r\n"         + // 2
                    "task T2 {}\n"            + // 3
                    "task T3 {}\r"            + // 4
                    "//Single Line Comment\r" + // 5
                    "task T4 {}\r\n"            // 6
                    );

            var t1 = syntax.DescendantNodes<TaskDefinitionSyntax>().First(td => td.Identifier.ToString() == "T1");
            Assert.That(t1.GetLocation().LineRange.Start.Line, Is.EqualTo(0));

            var t2 = syntax.DescendantNodes<TaskDefinitionSyntax>().First(td => td.Identifier.ToString() == "T2");
            Assert.That(t2.GetLocation().LineRange.Start.Line, Is.EqualTo(3));

            var t3 = syntax.DescendantNodes<TaskDefinitionSyntax>().First(td => td.Identifier.ToString() == "T3");
            Assert.That(t3.GetLocation().LineRange.Start.Line, Is.EqualTo(4));

            var t4 = syntax.DescendantNodes<TaskDefinitionSyntax>().First(td => td.Identifier.ToString() == "T4");
            Assert.That(t4.GetLocation().LineRange.Start.Line, Is.EqualTo(6));            
        }

        [Test]
        public void MultiLineNumberWithTrailingNewLineTest() {

            var syntax = Syntax.ParseCodeGenerationUnit(
                    "task T1 {}\r\n" // 0
                    );

            var t1 = syntax.DescendantNodes<TaskDefinitionSyntax>().First(td => td.Identifier.ToString() == "T1");
            Assert.That(t1.GetLocation().LineRange.Start.Line, Is.EqualTo(0));          
        }

        [Test]
        public void TextLinesTest() {
            var syntaxTree = SyntaxTree.ParseText(Resources.LargeNav);

            int expectedLine = 0;
            int currentEnd = 0;

            foreach (var lineExtent in syntaxTree.TextLines) {
                // Keine Zeilensprünge
                Assert.That(lineExtent.Line, Is.EqualTo(expectedLine));
                // Lückenlosigkeit
                Assert.That(lineExtent.Extent.Start, Is.EqualTo(currentEnd));

                expectedLine++;
                currentEnd = lineExtent.Extent.End;
            }
            // Vollständige Abdeckung: Achtung End zeigt _hinter_ das Ende!
            Assert.That(currentEnd, Is.EqualTo(Resources.LargeNav.Length+1));
        }
    }
}