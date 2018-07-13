using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class StringExtensionsTests {
    
        [Test]
        public void TestInQuot() {

            char quot='\'';

            var text = "Hi 'Max'!";

            Assert.That(text.IsInQuotation(0, quot), Is.False); // H
            Assert.That(text.IsInQuotation(1, quot), Is.False); // i
            Assert.That(text.IsInQuotation(2, quot), Is.False); // 
            Assert.That(text.IsInQuotation(3, quot), Is.False); // '
            // -
            Assert.That(text.IsInQuotation(4, quot), Is.True); // M
            Assert.That(text.IsInQuotation(5, quot), Is.True); // a
            Assert.That(text.IsInQuotation(6, quot), Is.True); // x
            Assert.That(text.IsInQuotation(7, quot), Is.True); // '
            // -
            Assert.That(text.IsInQuotation(8, quot), Is.False); // !
        }

        [Test]
        public void TestQuotationExtentMissingEnd() {

            char quot ='\'';

            var text = "'Max";
            var extent = text.QuotatedExtent(0, quot);
            Assert.That(extent, Is.EqualTo(TextExtent.Missing));
        }

        [Test]
        public void TestInQuotationExtentMissingEnd() {

            char quot ='\'';

            var text   = "'Max";
            Assert.That(text.IsInQuotation(0, quot), Is.False);
            Assert.That(text.IsInQuotation(1, quot), Is.True);
        }

        [Test]
        public void TestQuotationExtent() {

            char quot ='\'';

            var text = "Hi 'Max'!";

            Assert.That(text.QuotatedExtent(0, quot), Is.EqualTo(TextExtent.Missing)); // H
            Assert.That(text.QuotatedExtent(1, quot), Is.EqualTo(TextExtent.Missing));  // i
            Assert.That(text.QuotatedExtent(2, quot), Is.EqualTo(TextExtent.Missing)); // 
            Assert.That(text.QuotatedExtent(3, quot), Is.EqualTo(TextExtent.Missing)); // '
            // -
            var expectedExtent = new TextExtent(4, 3); // Max
            Assert.That(text.QuotatedExtent(4, quot), Is.EqualTo(expectedExtent)); // M
            Assert.That(text.QuotatedExtent(5, quot), Is.EqualTo(expectedExtent)); // a
            Assert.That(text.QuotatedExtent(6, quot), Is.EqualTo(expectedExtent)); // x
            Assert.That(text.QuotatedExtent(7, quot), Is.EqualTo(expectedExtent)); // '
            // -
            Assert.That(text.QuotatedExtent(8, quot), Is.EqualTo(TextExtent.Missing)); // !
        }

        [Test, TestCaseSource(nameof(GetMatchPartsTestCases))]
        public void Test(MatchPartsTestCase testCase) {

            var parts = PatternMatcher.Default.GetMatchedParts(testCase.Input, testCase.Pattern);

            Assert.That(parts, Is.EqualTo(testCase.Expected));
        }

        public static IEnumerable<MatchPartsTestCase> GetMatchPartsTestCases() {
            yield return new MatchPartsTestCase {
                Input    = "InputString",
                Pattern  = "is",
                Expected = {E(0, 1), E(5, 1)}
            };

            yield return new MatchPartsTestCase {
                Input    = "inputstring",
                Pattern  = "is",
                Expected = {E(0, 1), E(5, 1)}
            };

            yield return new MatchPartsTestCase {
                Input    = "InputString",
                Pattern  = "InSt",
                Expected = {E(0, 2), E(5, 2)}
            };

            // Kein Match
            yield return new MatchPartsTestCase {
                Input   = "InputString",
                Pattern = "Foo",
            };

            // TODO Dieser Test verhält sich nicht ideal. Eigentlich sollten vorzugsweise
            // die Großbuchstaben selektiert werden. Im Falle des "t" wird jedoch bereits
            // das erste Vorkommen ("Parts") als match gewertet.
            yield return new MatchPartsTestCase {
                Input    = "MatchPartsTestCase",
                Pattern  = "mptc",
                Expected = {E(0, 1), E(5, 1), E(8, 1), E(13, 2)}
            };

            TextExtent E(int start, int length) {
                return new TextExtent(start, length);
            }
        }

        public class MatchPartsTestCase {

            public string           Input    { get; set; }
            public string           Pattern  { get; set; }
            public List<TextExtent> Expected { get; } = new List<TextExtent>();

            public override string ToString() {
                return $"{Input} ({Pattern})";
            }

        }

    }

}