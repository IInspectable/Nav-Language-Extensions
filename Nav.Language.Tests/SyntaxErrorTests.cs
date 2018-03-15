using System.Linq;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class SyntaxErrorTests {

        [Test]
        public void TestSyntaxErrors() {
            var syntaxErrors = SyntaxTree.ParseText(Resources.AllRules)
                                         .Diagnostics;
            Assert.That(syntaxErrors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Nav0000UnexpectedCharacter() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                I1 ---> e1;  
              //---^
            }
            ";

            var syntaxErrors = SyntaxTree.ParseText(nav)
                                         .Diagnostics;

            var expected = new[] {DiagnosticDescriptors.Syntax.Nav0000UnexpectedCharacter};

            Assert.That(syntaxErrors.Select(diag => diag.Descriptor), Is.EquivalentTo(expected));

        }

    }

}