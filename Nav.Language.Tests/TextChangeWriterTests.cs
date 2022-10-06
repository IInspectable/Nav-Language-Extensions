#region Using Directives

using System;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests; 

[TestFixture]
public class TextChangeWriterTests {

    [Test]
    public void IsEmpty() {

        Assert.That(TextChange.Empty.IsEmpty, Is.True);
    }

    [Test]
    public void Empty() {

        Assert.That(TextChange.Empty.Extent.Start,    Is.EqualTo(0));
        Assert.That(TextChange.Empty.Extent.End,      Is.EqualTo(0));
        Assert.That(TextChange.Empty.ReplacementText, Is.EqualTo(String.Empty));
    }

    [Test]
    public void SimpleDelete() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewRemove(start: 0, length: 6),
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Max!"));
    }

    [Test]
    public void DeleteWithReplace() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewRemove(start: 0, length: 6),
            TextChange.NewReplace(start: 9, length: 1, text: "?")
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Max?"));
    }

    [Test]
    public void InsertWithReplace() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewReplace(start: 5, length: 0, text: "o"),
            TextChange.NewReplace(start: 9, length: 1, text: "?")
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Halloo Max?"));
    }

    [Test]
    public void DeleteWithReplaceReversed() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewReplace(start: 9, length: 1, text: "?"),
            TextChange.NewRemove(start: 0, length: 6)
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Max?"));
    }

    [Test]
    public void OverlappingChanges() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewRemove(start: 3, length: 4),
            TextChange.NewReplace(start: 0, length: 5, text: "?"),
        };

        var writer = new TextChangeWriter();

        Assert.Throws<ArgumentException>(() => writer.ApplyTextChanges(text, textChanges));
    }

    [Test]
    public void ReplaceWithChangesOutOfIndex() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewReplace(start: 10, length: 1, text: "?"),
        };

        var writer = new TextChangeWriter();

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.ApplyTextChanges(text, textChanges));
    }

    [Test]
    public void SimpleInsert() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewReplace(start: 10, length: 0, text: " Wie geht es dir?"),
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Hallo Max! Wie geht es dir?"));
    }

    [Test]
    public void Replacement() {

        var text = "Hallo Max!";

        var textChanges = new[] {
            TextChange.NewReplace(start: 6, length: 4, text: "Moritz! Wie geht es dir?"),
        };

        var writer = new TextChangeWriter();

        var result = writer.ApplyTextChanges(text, textChanges);

        Assert.That(result, Is.EqualTo("Hallo Moritz! Wie geht es dir?"));
    }

}