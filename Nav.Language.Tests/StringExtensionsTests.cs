using System;
using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class StringExtensionsTests {

        [Test]
        public void TestInQuot1() {

            char quotationChar = '\'';

            var text = "Hi 'Max'!";

            Assert.That(text.IsInQuotation(0, quotationChar), Is.False); // H
            Assert.That(text.IsInQuotation(1, quotationChar), Is.False); // i
            Assert.That(text.IsInQuotation(2, quotationChar), Is.False); // 
            Assert.That(text.IsInQuotation(3, quotationChar), Is.False); // '
            // -
            Assert.That(text.IsInQuotation(4, quotationChar), Is.True); // M
            Assert.That(text.IsInQuotation(5, quotationChar), Is.True); // a
            Assert.That(text.IsInQuotation(6, quotationChar), Is.True); // x
            Assert.That(text.IsInQuotation(7, quotationChar), Is.True); // '
            // -
            Assert.That(text.IsInQuotation(8, quotationChar), Is.False); // !
            Assert.That(text.IsInQuotation(9, quotationChar), Is.False); // Wir erlauben es auch, hinter das Ende zu gehen
        }

        [Test]
        public void TestInQuot2() {

            char quotationChar = '\'';

            var text = "'A'";

            Assert.That(text.IsInQuotation(0, quotationChar), Is.False); // '
            Assert.That(text.IsInQuotation(1, quotationChar), Is.True);  // A
            Assert.That(text.IsInQuotation(2, quotationChar), Is.True);  // '
            Assert.That(text.IsInQuotation(3, quotationChar), Is.False); // 
        }

        [Test]
        public void TestQuotationExtentMissingEnd() {

            char quotationChar = '\'';

            var text = "'Max";
            // Kein Explizites Ende quot und kein terminiernedes Leerzeichen
            // -> Extent geht bis zum Ende
            Assert.That(text.QuotatedExtent(1, quotationChar),
                        Is.EqualTo(new TextExtent(start: 1, length: 3)));

            Assert.That(text.QuotatedExtent(1, quotationChar, includequotationCharInExtent: true),
                        Is.EqualTo(new TextExtent(start: 0, length: 4)));
        }

        [Test]
        public void TestQuotationExtentMissingEndWithSpaces() {

            char quotationChar = '\'';

            var text = "'Max Foo Bar";
            // Kein Explizites Ende quot
            // -> Extent geht bis zum ersten Leerzeichen
            Assert.That(text.QuotatedExtent(1, quotationChar),
                        Is.EqualTo(new TextExtent(start: 1, length: 3)));
            Assert.That(text.QuotatedExtent(1, quotationChar, includequotationCharInExtent: true),
                        Is.EqualTo(new TextExtent(start: 0, length: 4)));
        }

        [Test]
        public void TestInQuotationExtentMissingEnd() {

            char quotationChar = '\'';

            var text = "'Max";
            Assert.That(text.IsInQuotation(0, quotationChar), Is.False);
            Assert.That(text.IsInQuotation(0, quotationChar), Is.False);

            Assert.That(text.IsInQuotation(1, quotationChar), Is.True);
        }

        [Test]
        public void TestQuotationExtent() {

            char quotationChar = '\'';

            var text = "Hi 'Max'!";

            Assert.That(text.QuotatedExtent(0, quotationChar), Is.EqualTo(TextExtent.Missing)); // H
            Assert.That(text.QuotatedExtent(1, quotationChar), Is.EqualTo(TextExtent.Missing)); // i
            Assert.That(text.QuotatedExtent(2, quotationChar), Is.EqualTo(TextExtent.Missing)); // 
            Assert.That(text.QuotatedExtent(3, quotationChar), Is.EqualTo(TextExtent.Missing)); // '
            // -
            var expectedExtent = new TextExtent(4, 3);                                      // Max
            Assert.That(text.QuotatedExtent(4, quotationChar), Is.EqualTo(expectedExtent)); // M
            Assert.That(text.QuotatedExtent(5, quotationChar), Is.EqualTo(expectedExtent)); // a
            Assert.That(text.QuotatedExtent(6, quotationChar), Is.EqualTo(expectedExtent)); // x
            Assert.That(text.QuotatedExtent(7, quotationChar), Is.EqualTo(expectedExtent)); // '
            // -
            Assert.That(text.QuotatedExtent(8, quotationChar), Is.EqualTo(TextExtent.Missing)); // !

            // Include Quotation
            Assert.That(text.QuotatedExtent(0, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // H
            Assert.That(text.QuotatedExtent(1, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // i
            Assert.That(text.QuotatedExtent(2, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // 
            Assert.That(text.QuotatedExtent(3, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // '
            // -
            var expectedFullExtent = new TextExtent(start: 3, length: 5);                             // Max
            Assert.That(text.QuotatedExtent(4, quotationChar, true), Is.EqualTo(expectedFullExtent)); // M
            Assert.That(text.QuotatedExtent(5, quotationChar, true), Is.EqualTo(expectedFullExtent)); // a
            Assert.That(text.QuotatedExtent(6, quotationChar, true), Is.EqualTo(expectedFullExtent)); // x
            Assert.That(text.QuotatedExtent(7, quotationChar, true), Is.EqualTo(expectedFullExtent)); // '
            // -
            Assert.That(text.QuotatedExtent(8, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // !
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

        [Test]
        public void GetStartOfIdentifier() {

            string text = "Bla Foo";

            Assert.That(text.GetStartOfIdentifier(0), Is.EqualTo(0));
            Assert.That(text.GetStartOfIdentifier(1), Is.EqualTo(0));
            Assert.That(text.GetStartOfIdentifier(2), Is.EqualTo(0));
            Assert.That(text.GetStartOfIdentifier(3), Is.EqualTo(-1));
            Assert.That(text.GetStartOfIdentifier(4), Is.EqualTo(4));
            Assert.That(text.GetStartOfIdentifier(5), Is.EqualTo(4));
            Assert.That(text.GetStartOfIdentifier(6), Is.EqualTo(4));

        }

        [Test]
        public void GetExtentOfPreviousIdentifier1() {

            string text = "Foo Bar Baz";

            var fooExtent = new TextExtent(0, 3);
            var barExtent = new TextExtent(4, 3);

            Assert.That(text.Substring(fooExtent), Is.EqualTo("Foo"));
            Assert.That(text.Substring(barExtent), Is.EqualTo("Bar"));

            Assert.That(text.GetExtentOfPreviousIdentifier(0), Is.EqualTo(TextExtent.Missing));
            Assert.That(text.GetExtentOfPreviousIdentifier(1), Is.EqualTo(TextExtent.Missing));
            Assert.That(text.GetExtentOfPreviousIdentifier(2), Is.EqualTo(TextExtent.Missing));
            Assert.That(text.GetExtentOfPreviousIdentifier(3), Is.EqualTo(fooExtent));
            Assert.That(text.GetExtentOfPreviousIdentifier(4), Is.EqualTo(fooExtent));
            Assert.That(text.GetExtentOfPreviousIdentifier(5), Is.EqualTo(fooExtent));
            Assert.That(text.GetExtentOfPreviousIdentifier(6), Is.EqualTo(fooExtent));

            Assert.Throws<IndexOutOfRangeException>(() => text.GetPreviousIdentifier(-1));

            Assert.That(text.GetPreviousIdentifier(0), Is.EqualTo(String.Empty));
            Assert.That(text.GetPreviousIdentifier(1), Is.EqualTo(String.Empty));
            Assert.That(text.GetPreviousIdentifier(2), Is.EqualTo(String.Empty));
            Assert.That(text.GetPreviousIdentifier(3), Is.EqualTo("Foo"));
            Assert.That(text.GetPreviousIdentifier(4), Is.EqualTo("Foo"));
            Assert.That(text.GetPreviousIdentifier(5), Is.EqualTo("Foo"));
            Assert.That(text.GetPreviousIdentifier(6), Is.EqualTo("Foo"));

            Assert.That(text.GetPreviousIdentifier(7),  Is.EqualTo("Bar"));
            Assert.That(text.GetPreviousIdentifier(8),  Is.EqualTo("Bar"));
            Assert.That(text.GetPreviousIdentifier(9),  Is.EqualTo("Bar"));
            Assert.That(text.GetPreviousIdentifier(10), Is.EqualTo("Bar"));

            Assert.Throws<IndexOutOfRangeException>(() => text.GetPreviousIdentifier(11));

        }

        [Test]
        public void GetExtentOfPreviousIdentifier2() {

            char quotationChar = '\'';

            string text = "taskref 'Bl";
            //             ----------^ 10

            var quotatedExtent     = text.QuotatedExtent(10, quotationChar);
            var previousIdentifier = text.GetPreviousIdentifier(quotatedExtent.Start - 1);

            Assert.That(previousIdentifier, Is.EqualTo("taskref"));

        }

        [Test]
        public void GetPreviousIdentifierOnTriggerTransition() {
            char   quotationChar = '\'';
            string lineText      = "    Choice_WhatToDoOnButtonF12                  --> OffenePostenÜbersicht if 'Nix zu tun';";

            var quotatedExtent = lineText.QuotatedExtent(79, quotationChar);
            var identifier     = lineText.GetPreviousIdentifier(quotatedExtent.Start - 1);

            Assert.That(identifier, Is.EqualTo("if"));

        }

        [Test]
        public void TestIndexOfPreviousNonWhitespace() {
            string text = "Foo Bar";

            Assert.That(text.IndexOfPreviousNonWhitespace(0), Is.EqualTo(-1));
            Assert.That(text.IndexOfPreviousNonWhitespace(1), Is.EqualTo(0));
            Assert.That(text.IndexOfPreviousNonWhitespace(2), Is.EqualTo(1));
            Assert.That(text.IndexOfPreviousNonWhitespace(3), Is.EqualTo(2));
            Assert.That(text.IndexOfPreviousNonWhitespace(4), Is.EqualTo(2));
            Assert.That(text.IndexOfPreviousNonWhitespace(5), Is.EqualTo(4));
            Assert.That(text.IndexOfPreviousNonWhitespace(6), Is.EqualTo(5));
            Assert.That(text.IndexOfPreviousNonWhitespace(7), Is.EqualTo(6));
        }

    }

}