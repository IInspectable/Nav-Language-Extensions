#region Using Directives

using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.StatementCompletion {

    sealed class CompletionCommandHandler : IOleCommandTarget {

        ICompletionSession _currentSession;

        public CompletionCommandHandler(IWpfTextView textView, ICompletionBroker broker) {
            _currentSession = null;

            TextView = textView;
            Broker   = broker;
        }

        public IWpfTextView TextView { get; }
        public ICompletionBroker Broker { get; }
        public IOleCommandTarget Next { get; set; }

        char GetTypeChar(IntPtr pvaIn) {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            bool handled = false;
            int hresult = VSConstants.S_OK;

            // 1. Pre-process
            if (pguidCmdGroup == VSConstants.VSStd2K) {
                switch ((VSConstants.VSStd2KCmdID)nCmdId) {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        handled = StartSession();
                        break;
                    case VSConstants.VSStd2KCmdID.RETURN:
                        handled = Complete(false);
                        break;
                    case VSConstants.VSStd2KCmdID.TAB:
                        handled = Complete(true);
                        break;
                    case VSConstants.VSStd2KCmdID.CANCEL:
                        handled = Cancel();
                        break;
                }
            }

            if (!handled)
                hresult = Next.Exec(pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);

            if (ErrorHandler.Succeeded(hresult)) {
                if (pguidCmdGroup == VSConstants.VSStd2K) {
                    switch ((VSConstants.VSStd2KCmdID)nCmdId) {
                        case VSConstants.VSStd2KCmdID.TYPECHAR:
                            char ch = GetTypeChar(pvaIn);
                            if (ch == ':' || ch == '[')
                                StartSession();
                            else if (_currentSession != null)
                                Filter();
                            break;
                        case VSConstants.VSStd2KCmdID.BACKSPACE:
                            Filter();
                            break;
                    }
                }
            }

            return hresult;
        }

        /// <summary>
        /// Narrow down the list of options as the user types input
        /// </summary>
        void Filter() {
            if (_currentSession == null)
                return;

            _currentSession.SelectedCompletionSet.SelectBestMatch();
            _currentSession.SelectedCompletionSet.Recalculate();
        }

        /// <summary>
        /// Cancel the auto-complete session, and leave the text unmodified
        /// </summary>
        bool Cancel() {
            if (_currentSession == null)
                return false;

            _currentSession.Dismiss();

            return true;
        }

        /// <summary>
        /// Auto-complete text using the specified token
        /// </summary>
        bool Complete(bool force) {
            if (_currentSession == null)
                return false;

            if (!_currentSession.SelectedCompletionSet.SelectionStatus.IsSelected && !force) {
                _currentSession.Dismiss();
                return false;
            } else {
                _currentSession.Commit();
                return true;
            }
        }

        /// <summary>
        /// Display list of potential tokens
        /// </summary>
        bool StartSession() {
            if (_currentSession != null)
                return false;

            SnapshotPoint caret = TextView.Caret.Position.BufferPosition;
            ITextSnapshot snapshot = caret.Snapshot;

            if (!Broker.IsCompletionActive(TextView)) {
                _currentSession = Broker.CreateCompletionSession(TextView, snapshot.CreateTrackingPoint(caret, PointTrackingMode.Positive), true);
            } else {
                _currentSession = Broker.GetSessions(TextView)[0];
            }
            _currentSession.Dismissed += (sender, args) => _currentSession = null;

            _currentSession.Start();

            return true;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            if (pguidCmdGroup == VSConstants.VSStd2K) {
                switch ((VSConstants.VSStd2KCmdID)prgCmds[0].cmdID) {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }
            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}