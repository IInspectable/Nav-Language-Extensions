using NUnit.Framework;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class SyntaxErrorTests {

        [Test]
        public void TestSyntaxErrors() {
            var syntaxErrors = SyntaxTree.ParseText(Resources.AllRules)
                                         .Diagnostics;
            Assert.That(syntaxErrors.Length, Is.EqualTo(0));
        }

    }

}