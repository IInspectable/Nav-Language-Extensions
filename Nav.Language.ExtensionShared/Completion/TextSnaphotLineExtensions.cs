#region Using Directives

using System.IO;
using System.Linq;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion;

interface INavCommand {

}interface INavCommandBody {

}
interface IBeginMessageBoxWith2ButtonsWFS {
    INavCommandBody Begin(string message);
}

class TO {

};

class ConcatCommand: INavCommandBody {

    public string          NodeName { get; init; }
    public TO              TO       { get; init; }
    public INavCommandBody ContinuationBody     { get; init; }

}

class DokumentationshinweiseTO: TO {

}

abstract class DokumentationshinweiseWfsBase {
    const string                    DokumentationshinweiseTONodeName = "DokumentationshinweiseAuswahl";
    IBeginMessageBoxWith2ButtonsWFS _messageBoxWith2Buttons;
    INavCommandBody BeginMessagebox(string text) {
        return null;
    }

    public void OnF12Click() {

        var callContext = new OnF12ClickLogicCallContext(this);
        var body        = OnF12ClickLogic(callContext);
        switch (body) {

            case ConcatCommand { NodeName: DokumentationshinweiseTONodeName } cc:
                //  return GotoGUI(dokumentationshinweiseAuswahlTO).Concat(DokumentationshinweiseTOContinuation(cc.ContinuationBody));
                DokumentationshinweiseTOContinuation(cc.ContinuationBody);
                break;
        }

        INavCommand DokumentationshinweiseTOContinuation(INavCommandBody continuationBody) {
            return null;
        }
    }

    protected abstract INavCommandBody OnF12ClickLogic(OnF12ClickLogicCallContext callContext);

    protected class OnF12ClickLogicCallContext(DokumentationshinweiseWfsBase wfs) {

        public DokumentationshinweiseTOConcat Show(DokumentationshinweiseTO to) => new(wfs, to);

        public class DokumentationshinweiseTOConcat(DokumentationshinweiseWfsBase wfs, DokumentationshinweiseTO to) {

            public INavCommandBody BeginMessagebox(string message) {
                return new ConcatCommand {
                    NodeName         = DokumentationshinweiseTONodeName,
                    TO               = to,
                    ContinuationBody = wfs._messageBoxWith2Buttons.Begin(message),
                };
            }

        }

    }

}

class DokumentationshinweiseWfs: DokumentationshinweiseWfsBase {

    protected override INavCommandBody OnF12ClickLogic(OnF12ClickLogicCallContext callContext) {

        var to = new DokumentationshinweiseTO();

        return callContext.Show(to)
                          .BeginMessagebox("Foo");

    }

}

static class TextSnaphotLineExtensions {

    public static SnapshotPoint GetStartOfIdentifier(this ITextSnapshotLine line, SnapshotPoint start) {
        while (start > line.Start && SyntaxFacts.IsIdentifierCharacter((start - 1).GetChar())) {
            start -= 1;
        }

        return start;
    }

    public static SnapshotPoint? GetPreviousNonWhitespace(this ITextSnapshotLine line, SnapshotPoint start) {

        if (start == line.Start) {
            return null;
        }

        do {
            start -= 1;
        } while (start > line.Start && char.IsWhiteSpace(start.GetChar()));

        return start;
    }

    public static SnapshotSpan? GetSpanOfPreviousIdentifier(this ITextSnapshotLine line, SnapshotPoint start) {

        var wordEnd = line.GetPreviousNonWhitespace(start);
        if (wordEnd == null) {
            return null;
        }

        var wordStart = line.GetStartOfIdentifier(wordEnd.Value);

        return new SnapshotSpan(wordStart, wordEnd.Value + 1);
    }

    public static SnapshotPoint GetStartOfFileNamePart(this ITextSnapshotLine line, SnapshotPoint start) {
        while (start > line.Start && IsFileNameChar((start - 1).GetChar())) {
            start -= 1;
        }

        return start;
    }

    static bool IsFileNameChar(this char ch) {
        return Path.GetInvalidFileNameChars().All(c => ch != c);
    }

    public static SnapshotPoint GetStartOfEdge(this ITextSnapshotLine line, SnapshotPoint start) {
        while (start > line.Start && IsEdgeChar((start - 1).GetChar())) {
            start -= 1;
        }

        return start;
    }

    static bool IsEdgeChar(this char ch) {
        return SyntaxFacts.EdgeKeywords.SelectMany(k => k).Contains(ch);
    }

}