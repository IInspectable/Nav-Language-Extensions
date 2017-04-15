#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.OLE.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    partial class CommandTarget : IOleCommandTarget {

       const VSConstants.VSStd2KCmdID CmdidNextHighlightedReference     = (VSConstants.VSStd2KCmdID)2400;
       const VSConstants.VSStd2KCmdID CmdidPreviousHighlightedReference = (VSConstants.VSStd2KCmdID)2401;

        public virtual int Exec(ref Guid pguidCmdGroup, uint commandId, uint executeInformation, IntPtr pvaIn, IntPtr pvaOut) {
            var subjectBuffer = GetSubjectBufferContainingCaret();

            if(subjectBuffer == null) {
                return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            var contentType = subjectBuffer.ContentType;

            if(pguidCmdGroup == VSConstants.VSStd2K) {
                return ExecuteVisualStudio2000(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut, subjectBuffer, contentType);
            }

            return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
        }

        protected virtual int ExecuteVisualStudio2000(ref Guid pguidCmdGroup, uint commandId, uint executeInformation, IntPtr pvaIn, IntPtr pvaOut, ITextBuffer subjectBuffer, IContentType contentType) {
            int result = VSConstants.S_OK;
            var guidCmdGroup = pguidCmdGroup;

            void ExecuteNextCommandTarget() {
                result = NextCommandTarget.Exec(ref guidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            switch((VSConstants.VSStd2KCmdID) commandId) {
                case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                    ExecuteCommentBlock(subjectBuffer, ExecuteNextCommandTarget);
                    break;

                case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                    ExecuteUncommentBlock(subjectBuffer, ExecuteNextCommandTarget);
                    break;

                case CmdidNextHighlightedReference:
                    ExecuteNextHighlightedReference(subjectBuffer, contentType, ExecuteNextCommandTarget);
                    break;

                case CmdidPreviousHighlightedReference:
                    ExecutePreviousHighlightedReference(subjectBuffer, contentType, ExecuteNextCommandTarget);
                    break;

                default:
                    return NextCommandTarget.Exec(ref pguidCmdGroup, commandId, executeInformation, pvaIn, pvaOut);
            }

            return result;
        }

        protected void ExecuteUncommentBlock(ITextBuffer subjectBuffer, Action executeNextCommandTarget) {
            HandlerService.Execute(
                args       : new UncommentSelectionCommandArgs(WpfTextView, subjectBuffer),
                lastHandler: executeNextCommandTarget);
        }

        protected void ExecuteCommentBlock(ITextBuffer subjectBuffer, Action executeNextCommandTarget) {
            HandlerService.Execute(
                args       : new CommentSelectionCommandArgs(WpfTextView, subjectBuffer),
                lastHandler: executeNextCommandTarget);
        }

        protected void ExecutePreviousHighlightedReference(ITextBuffer subjectBuffer, IContentType contentType, Action executeNextCommandTarget) {
            HandlerService.Execute(
                args       : new NavigateToHighlightedReferenceCommandArgs(WpfTextView, subjectBuffer, NavigateDirection.Up),
                lastHandler: executeNextCommandTarget);
        }

        protected void ExecuteNextHighlightedReference(ITextBuffer subjectBuffer, IContentType contentType, Action executeNextCommandTarget) {
            HandlerService.Execute(
                args       : new NavigateToHighlightedReferenceCommandArgs(WpfTextView, subjectBuffer, NavigateDirection.Down),
                lastHandler: executeNextCommandTarget);
        }
    }
}