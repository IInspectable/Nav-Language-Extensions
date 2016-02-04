#region Using Directives

using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    class IntellisenseController : IIntellisenseController {

        ITextView _textView;
        readonly IList<ITextBuffer> _subjectBuffers;
        readonly IntellisenseControllerProvider _provider;
        // ReSharper disable once NotAccessedField.Local
        IQuickInfoSession _session;

        public IntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers, IntellisenseControllerProvider provider) {
            _textView = textView;
            _subjectBuffers = subjectBuffers;
            _provider = provider;

            _textView.MouseHover += OnTextViewMouseHover;
        }

        void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
            //find the mouse position by mapping down to the subject buffer
            SnapshotPoint? point = _textView.BufferGraph.MapDownToFirstMatch
                (new SnapshotPoint(_textView.TextSnapshot, e.Position),
                    PointTrackingMode.Positive,
                    snapshot => _subjectBuffers.Contains(snapshot.TextBuffer),
                    PositionAffinity.Predecessor);

            if (point != null) {
                ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
                    PointTrackingMode.Positive);

                if (!_provider.QuickInfoBroker.IsQuickInfoActive(_textView)) {
                    _session = _provider.QuickInfoBroker.TriggerQuickInfo(_textView, triggerPoint, true);
                }
            }
        }

        public void Detach(ITextView textView) {
            if (_textView == textView) {
                _textView.MouseHover -= OnTextViewMouseHover;
                _textView = null;
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }
    }
}