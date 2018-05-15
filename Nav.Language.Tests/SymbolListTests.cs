#region Using Directives

using System.Linq;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class SymbolListTests {

        const string Nav = @"
task A
{
    init I1;            
    exit e1;

    I1  --> e1;     
}
";

        [Test]
        public void FindAtStartpoint() {

            var model = ParseModel(Nav);

            // Startpoint of Symbol A
            var symbols = model.Symbols[TextExtent.FromBounds(7, 7), includeOverlapping: true].ToList();

            Assert.That(symbols.Count,   Is.EqualTo(1));
            Assert.That(symbols[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public void FindAtEndPoint() {

            var model = ParseModel(Nav);

            // Endpoint of Symbol A
            var symbols = model.Symbols[TextExtent.FromBounds(8, 8), includeOverlapping: true].ToList();

            Assert.That(symbols.Count,   Is.EqualTo(1));
            Assert.That(symbols[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public void FindAtWholeExtentOfSymbolAIncludingOverlappings() {

            var model = ParseModel(Nav);

            // Whole extent of A including overlappings
            var symbols = model.Symbols[TextExtent.FromBounds(7, 8), includeOverlapping: true].ToList();

            Assert.That(symbols.Count,   Is.EqualTo(1));
            Assert.That(symbols[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public void FindAtWholeExtentOfSymbolANotIncludingOverlappings() {

            var model = ParseModel(Nav);

            // Whole extent of A not including overlapping
            var symbols = model.Symbols[TextExtent.FromBounds(7, 8), includeOverlapping: false].ToList();

            Assert.That(symbols.Count,   Is.EqualTo(1));
            Assert.That(symbols[0].Name, Is.EqualTo("A"));
        }

        CodeGenerationUnit ParseModel(string source) {
            var syntax = Syntax.ParseCodeGenerationUnit(source);

            var model = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax);
            return model;
        }

    }

}