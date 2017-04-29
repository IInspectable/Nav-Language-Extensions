#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class RenameChoiceAction : CodeFixAction {

        public RenameChoiceAction(SymbolRenameCodeFix codeFix,
                                  CodeFixActionsParameter parameter, 
                                  CodeFixActionContext context): base(context, parameter) {

            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }
        // TODO Allgemeiner Rename Fix: Namen/Texte(Icons generalisiere
        public SymbolRenameCodeFix CodeFix { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.Symbol);
        public override string DisplayText       => $"Rename choice '{CodeFix.Symbol.Name}'";
        public override ImageMoniker IconMoniker => ImageMonikers.RenameNode;

        public override void Invoke(CancellationToken cancellationToken) {
            
            var newChoiceName = Context.DialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : "Rename choice",
                defaultResonse: CodeFix.Symbol.Name,
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : CodeFix.ValidateSymbolName
            )?.Trim();

            if (String.IsNullOrEmpty(newChoiceName)) {
                return;
            }

            ApplyTextChanges(
                undoDescription: DisplayText, 
                waitMessage    : $"Renaming choice '{CodeFix.Symbol.Name}'...", 
                textChanges    : CodeFix.GetTextChanges(newChoiceName));
        }
    }
}