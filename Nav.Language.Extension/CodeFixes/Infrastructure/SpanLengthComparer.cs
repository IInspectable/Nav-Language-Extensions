#region Using Directives

using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    partial class CodeFixSuggestedActionsSource : SemanticModelServiceDependent, ISuggestedActionsSource {

        sealed class SpanLengthComparer: IComparer<Span?> {

            public static readonly SpanLengthComparer Default = new SpanLengthComparer();

            public int Compare(Span? x, Span? y) {
                if(x == null && y == null) {
                    return 0;
                }
                // Kein Span bedeuted hier "unendlich groß"!
                if(x == null) {
                    return 1;
                }
                if(y == null) {
                    return -1;
                }
                return x.Value.Length.CompareTo(y.Value.Length);
            }
        }
    }
}