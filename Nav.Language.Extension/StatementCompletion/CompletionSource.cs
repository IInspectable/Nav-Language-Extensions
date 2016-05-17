#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.StatementCompletion {

    internal class CompletionSource : ICompletionSource {

        readonly ITextBuffer _buffer;
        bool _disposed;

        public CompletionSource(ITextBuffer buffer) {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
            if (_disposed)
                throw new ObjectDisposedException("OokCompletionSource");

            List<Completion> completions = new List<Completion>()
            {
                new Completion("Ook!"),
                new Completion("Ook."),
                new Completion("Ook?")
            };

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var snapshotPoint = session.GetTriggerPoint(snapshot);
            if (snapshotPoint != null) {

                var triggerPoint = (SnapshotPoint)snapshotPoint;

                var line = triggerPoint.GetContainingLine();
                SnapshotPoint start = triggerPoint;

                while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar())) {
                    start -= 1;
                }

                var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

                completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
            }
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}
