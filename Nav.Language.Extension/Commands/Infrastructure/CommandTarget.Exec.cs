#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    partial class CommandTarget: IOleCommandTarget {

        public virtual int Exec(ref Guid pguidCmdGroup, uint commandId, uint executeInformation, IntPtr pvaIn, IntPtr pvaOut) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var subjectBuffer = GetSubjectBufferContainingCaret();

            if (subjectBuffer == null) {
                return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            var contentType = subjectBuffer.ContentType;

            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
                return ExecuteVisualStudio97(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut, subjectBuffer, contentType);
            }

            if (pguidCmdGroup == VSConstants.VSStd2K) {
                return ExecuteVisualStudio2000(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut, subjectBuffer, contentType);
            }

            return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
        }

        private int ExecuteVisualStudio97(ref Guid pguidCmdGroup, uint commandId, uint executeInformation, IntPtr pvaIn, IntPtr pvaOut, ITextBuffer subjectBuffer, IContentType contentType) {

            ThreadHelper.ThrowIfNotOnUIThread();

            int result       = VSConstants.S_OK;
            var guidCmdGroup = pguidCmdGroup;

            void ExecuteNextCommandTarget() {
                result = NextCommandTarget.Exec(ref guidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            switch ((VSConstants.VSStd97CmdID) commandId) {

                case VSConstants.VSStd97CmdID.ViewCode:
                    ExecuteViewCode(subjectBuffer, contentType, ExecuteNextCommandTarget);
                    break;
                default:
                    return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            return result;
        }

        protected virtual int ExecuteVisualStudio2000(ref Guid pguidCmdGroup, uint commandId, uint executeInformation, IntPtr pvaIn, IntPtr pvaOut, ITextBuffer subjectBuffer, IContentType contentType) {

            ThreadHelper.ThrowIfNotOnUIThread();

            int result       = VSConstants.S_OK;
            var guidCmdGroup = pguidCmdGroup;

            void ExecuteNextCommandTarget() {
                result = NextCommandTarget.Exec(ref guidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            switch ((VSConstants.VSStd2KCmdID) commandId) {

                default:
                    ExecuteNextCommandTarget();
                    break;
            }

            return result;
        }

        protected void ExecuteViewCode(ITextBuffer subjectBuffer, IContentType contentType, Action executeNextCommandTarget) {
            HandlerService.Execute(
                args: new ViewCodeCommandArgs(WpfTextView, subjectBuffer),
                lastHandler: executeNextCommandTarget);
        }

    }

}