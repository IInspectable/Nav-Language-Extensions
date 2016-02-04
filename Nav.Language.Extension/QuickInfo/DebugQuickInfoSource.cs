#region Using Directives

using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    sealed class DebugQuickInfoSource : ParserServiceDependent, IQuickInfoSource {

        public DebugQuickInfoSource(ITextBuffer textBuffer): base(textBuffer) {
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan) {
            applicableToSpan = null;

            var modifier = ModifierKeys.Control | ModifierKeys.Shift;
            if((Keyboard.Modifiers & modifier) != modifier) {
                return;
            }

            var parseResult = ParserService.ParseResult;
            if (parseResult == null) {
                return;
            }
            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(parseResult.Snapshot);
            if(!subjectTriggerPoint.HasValue) {
                return;
            }

            var triggerToken = parseResult.SyntaxTree.Tokens.FindAtPosition(subjectTriggerPoint.Value.Position);

            if(triggerToken.IsMissing || triggerToken.Parent == null) {
                return;
            }

            var location = triggerToken.GetLocation();
            qiContent.Add($"{triggerToken.Type} ({triggerToken.Classification}) Ln {location?.StartLine + 1} Ch {location?.StartCharacter + 1}\r\n{triggerToken.Parent?.GetType().Name}");

            applicableToSpan = parseResult.Snapshot.CreateTrackingSpan(
                triggerToken.Start,
                triggerToken.Length,
                SpanTrackingMode.EdgeExclusive);
        }
    }
}