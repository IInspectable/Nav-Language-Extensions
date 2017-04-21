#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    partial class CommandTarget : IOleCommandTarget {
        
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {

            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                return QueryVisualStudio2000Status(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }

            return NextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
        
        int QueryVisualStudio2000Status(ref Guid pguidCmdGroup, uint commandCount, OLECMD[] prgCmds, IntPtr commandText) {
            switch((VSConstants.VSStd2KCmdID) prgCmds[0].cmdID) {
                case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                    return QueryCommentBlockStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);

                case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                    return QueryUncommentBlockStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);

                case CmdidNextHighlightedReference:
                    return QueryNavigateHighlightedReferenceStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText, NavigateDirection.Down);
                case CmdidPreviousHighlightedReference:
                    return QueryNavigateHighlightedReferenceStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText, NavigateDirection.Up);

                case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                    return QueryCompleteWordStatus(prgCmds);

                case VSConstants.VSStd2KCmdID.BACKTAB:
                    return QueryBackTabStatus(prgCmds);
                default:
                    return NextCommandTarget.QueryStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
            }
        }

        int QueryUncommentBlockStatus(ref Guid pguidCmdGroup, uint commandCount, OLECMD[] prgCmds, IntPtr commandText) {
            return GetCommandState(
                createArgs   : (v, b) => new UncommentSelectionCommandArgs(v, b),
                pguidCmdGroup: ref pguidCmdGroup,
                commandCount : commandCount,
                prgCmds      : prgCmds,
                commandText  : commandText);
        }

        int QueryCommentBlockStatus(ref Guid pguidCmdGroup, uint commandCount, OLECMD[] prgCmds, IntPtr commandText) {
            return GetCommandState(
                createArgs   : (v, b) => new CommentSelectionCommandArgs(v, b),
                pguidCmdGroup: ref pguidCmdGroup,
                commandCount : commandCount,
                prgCmds      : prgCmds,
                commandText  : commandText);
        }

        int QueryNavigateHighlightedReferenceStatus(ref Guid pguidCmdGroup, uint commandCount, OLECMD[] prgCmds, IntPtr commandText, NavigateDirection navigateDirection) {

            return GetCommandState(
                createArgs   : (v, b) => new NavigateToHighlightedReferenceCommandArgs(v, b, navigateDirection),
                pguidCmdGroup: ref pguidCmdGroup,
                commandCount : commandCount,
                prgCmds      : prgCmds,
                commandText  : commandText);
        }

        int QueryBackTabStatus(OLECMD[] prgCmds) {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
            return VSConstants.S_OK;
        }

        int QueryCompleteWordStatus(OLECMD[] prgCmds) {
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
            return VSConstants.S_OK;
        }

        int GetCommandState<T>(
            Func<ITextView, ITextBuffer, T> createArgs,
            ref Guid pguidCmdGroup,
            uint commandCount,
            OLECMD[] prgCmds,
            IntPtr commandText)
            where T : CommandArgs {
            var result = VSConstants.S_OK;

            var guidCmdGroup = pguidCmdGroup;

            CommandState ExecuteNextCommandTarget() {
                result = NextCommandTarget.QueryStatus(ref guidCmdGroup, commandCount, prgCmds, commandText);

                // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
                var isAvailable = ((OLECMDF) prgCmds[0].cmdf & OLECMDF.OLECMDF_ENABLED) == OLECMDF.OLECMDF_ENABLED;
                var isChecked   = ((OLECMDF) prgCmds[0].cmdf & OLECMDF.OLECMDF_LATCHED) == OLECMDF.OLECMDF_LATCHED;
                // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
                return new CommandState(isAvailable, isChecked, GetText(commandText));
            }

            CommandState commandState;
            var subjectBuffer = GetSubjectBufferContainingCaret();
            if(subjectBuffer == null) {
                commandState = ExecuteNextCommandTarget();
            } else {
                commandState = HandlerService.GetCommandState(
                    args       : createArgs(WpfTextView, subjectBuffer),
                    lastHandler: ExecuteNextCommandTarget);
            }

            var enabled = commandState.IsAvailable ? OLECMDF.OLECMDF_ENABLED : OLECMDF.OLECMDF_INVISIBLE;
            var latched = commandState.IsChecked   ? OLECMDF.OLECMDF_LATCHED : OLECMDF.OLECMDF_NINCHED;
            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            prgCmds[0].cmdf = (uint) (enabled | latched | OLECMDF.OLECMDF_SUPPORTED);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags

            if (!string.IsNullOrEmpty(commandState.DisplayText) && GetText(commandText) != commandState.DisplayText) {
                SetText(commandText, commandState.DisplayText);
            }

            return result;
        }

        static unsafe string GetText(IntPtr pCmdTextInt) {
            if(pCmdTextInt == IntPtr.Zero) {
                return string.Empty;
            }

            OLECMDTEXT* pText = (OLECMDTEXT*) pCmdTextInt;

            // Punt early if there is no text in the structure.
            if(pText->cwActual == 0) {
                return string.Empty;
            }

            return new string((char*) &pText->rgwz, 0, (int) pText->cwActual);
        }

        static unsafe void SetText(IntPtr pCmdTextInt, string text) {
            OLECMDTEXT* pText = (OLECMDTEXT*) pCmdTextInt;

            // If, for some reason, we don't get passed an array, we should just bail
            if(pText->cwBuf == 0) {
                return;
            }

            fixed(char* pinnedText = text) {
                char* src = pinnedText;
                char* dest = (char*) (&pText->rgwz);

                // Don't copy too much, and make sure to reserve space for the terminator
                int length = Math.Min(text.Length, (int) pText->cwBuf - 1);

                for(int i = 0; i < length; i++) {
                    *dest++ = *src++;
                }

                // Add terminating NUL
                *dest = '\0';

                pText->cwActual = (uint) length;
            }
        }
    }
}