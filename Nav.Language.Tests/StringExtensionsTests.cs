using System;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests; 

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
        Assert.That(text.QuotedExtent(1, quotationChar),
                    Is.EqualTo(new TextExtent(start: 1, length: 3)));

        Assert.That(text.QuotedExtent(1, quotationChar, includequotationCharInExtent: true),
                    Is.EqualTo(new TextExtent(start: 0, length: 4)));
    }

    [Test]
    public void TestQuotationExtentMissingEndWithSpaces() {

        char quotationChar = '\'';

        var text = "'Max Foo Bar";
        // Kein Explizites Ende quot
        // -> Extent geht bis zum ersten Leerzeichen
        Assert.That(text.QuotedExtent(1, quotationChar),
                    Is.EqualTo(new TextExtent(start: 1, length: 3)));
        Assert.That(text.QuotedExtent(1, quotationChar, includequotationCharInExtent: true),
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

        Assert.That(text.QuotedExtent(0, quotationChar), Is.EqualTo(TextExtent.Missing)); // H
        Assert.That(text.QuotedExtent(1, quotationChar), Is.EqualTo(TextExtent.Missing)); // i
        Assert.That(text.QuotedExtent(2, quotationChar), Is.EqualTo(TextExtent.Missing)); // 
        Assert.That(text.QuotedExtent(3, quotationChar), Is.EqualTo(TextExtent.Missing)); // '
        // -
        var expectedExtent = new TextExtent(4, 3);                                    // Max
        Assert.That(text.QuotedExtent(4, quotationChar), Is.EqualTo(expectedExtent)); // M
        Assert.That(text.QuotedExtent(5, quotationChar), Is.EqualTo(expectedExtent)); // a
        Assert.That(text.QuotedExtent(6, quotationChar), Is.EqualTo(expectedExtent)); // x
        Assert.That(text.QuotedExtent(7, quotationChar), Is.EqualTo(expectedExtent)); // '
        // -
        Assert.That(text.QuotedExtent(8, quotationChar), Is.EqualTo(TextExtent.Missing)); // !

        // Include Quotation
        Assert.That(text.QuotedExtent(0, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // H
        Assert.That(text.QuotedExtent(1, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // i
        Assert.That(text.QuotedExtent(2, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // 
        Assert.That(text.QuotedExtent(3, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // '
        // -
        var expectedFullExtent = new TextExtent(start: 3, length: 5);                           // Max
        Assert.That(text.QuotedExtent(4, quotationChar, true), Is.EqualTo(expectedFullExtent)); // M
        Assert.That(text.QuotedExtent(5, quotationChar, true), Is.EqualTo(expectedFullExtent)); // a
        Assert.That(text.QuotedExtent(6, quotationChar, true), Is.EqualTo(expectedFullExtent)); // x
        Assert.That(text.QuotedExtent(7, quotationChar, true), Is.EqualTo(expectedFullExtent)); // '
        // -
        Assert.That(text.QuotedExtent(8, quotationChar, true), Is.EqualTo(TextExtent.Missing)); // !
    }

    [Test]
    public void TestInBlock1() {

        char blockStartChar = '[';
        char blockEndChar   = ']';

        var text = "Hi [Max]!";

        Assert.That(text.IsInTextBlock(0, blockStartChar, blockEndChar), Is.False); // H
        Assert.That(text.IsInTextBlock(1, blockStartChar, blockEndChar), Is.False); // i
        Assert.That(text.IsInTextBlock(2, blockStartChar, blockEndChar), Is.False); // 
        Assert.That(text.IsInTextBlock(3, blockStartChar, blockEndChar), Is.False); // [
        // -
        Assert.That(text.IsInTextBlock(4, blockStartChar, blockEndChar), Is.True); // M
        Assert.That(text.IsInTextBlock(5, blockStartChar, blockEndChar), Is.True); // a
        Assert.That(text.IsInTextBlock(6, blockStartChar, blockEndChar), Is.True); // x
        Assert.That(text.IsInTextBlock(7, blockStartChar, blockEndChar), Is.True); // ]
        // -
        Assert.That(text.IsInTextBlock(8, blockStartChar, blockEndChar), Is.False); // !
        Assert.That(text.IsInTextBlock(9, blockStartChar, blockEndChar), Is.False); // Wir erlauben es auch, hinter das Ende zu gehen
    }

    [Test]
    public void TestNestedBlocks1() {

        char blockStartChar = '[';
        char blockEndChar   = ']';

        var text = "Hi[ [Max]!]";

        Assert.That(text.IsInTextBlock(0, blockStartChar, blockEndChar), Is.False); // H
        Assert.That(text.IsInTextBlock(1, blockStartChar, blockEndChar), Is.False); // i
        Assert.That(text.IsInTextBlock(2, blockStartChar, blockEndChar), Is.False); // [
        // -
        Assert.That(text.IsInTextBlock(3, blockStartChar, blockEndChar), Is.True); // 
        Assert.That(text.IsInTextBlock(4, blockStartChar, blockEndChar), Is.True); // [
        // -
        Assert.That(text.IsInTextBlock(5, blockStartChar, blockEndChar), Is.True); // M
        Assert.That(text.IsInTextBlock(6, blockStartChar, blockEndChar), Is.True); // a
        Assert.That(text.IsInTextBlock(7, blockStartChar, blockEndChar), Is.True); // x
        Assert.That(text.IsInTextBlock(8, blockStartChar, blockEndChar), Is.True); // ]
        // -
        Assert.That(text.IsInTextBlock(9,  blockStartChar, blockEndChar), Is.True); // !
        Assert.That(text.IsInTextBlock(10, blockStartChar, blockEndChar), Is.True); // ]
        // -
        Assert.That(text.IsInTextBlock(11, blockStartChar, blockEndChar), Is.False); // Wir erlauben es auch, hinter das Ende zu gehen
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

        var quotatedExtent     = text.QuotedExtent(10, quotationChar);
        var previousIdentifier = text.GetPreviousIdentifier(quotatedExtent.Start - 1);

        Assert.That(previousIdentifier, Is.EqualTo("taskref"));

    }

    [Test]
    public void GetPreviousIdentifierOnTriggerTransition() {
        char   quotationChar = '\'';
        string lineText      = "    Choice_WhatToDoOnButtonF12                  --> OffenePostenÜbersicht if 'Nix zu tun';";

        var quotatedExtent = lineText.QuotedExtent(79, quotationChar);
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

    [Test]
    public void TestGetNewLineCharCount() {

        Assert.That("".GetNewLineCharCount(),            Is.EqualTo(0));
        Assert.That("Foo".GetNewLineCharCount(),         Is.EqualTo(0));
        Assert.That("Foo\n".GetNewLineCharCount(),       Is.EqualTo(1));
        Assert.That("Foo\r\n".GetNewLineCharCount(),     Is.EqualTo(2));
        Assert.That("Foo\r\nNeue".GetNewLineCharCount(), Is.EqualTo(0));
    }
}