using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class StringExtensionsTests {

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