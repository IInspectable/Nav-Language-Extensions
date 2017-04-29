#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(RenameChoiceActionProvider))]
    class RenameChoiceActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public RenameChoiceActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var editorSettings     = parameter.GetEditorSettings();
            var codeGenerationUnit = parameter.SemanticModelResult.CodeGenerationUnit;
            var renameCodeFixes    = parameter.Symbols
                                              .Select(symbol => Renamer.TryFindRenameCodeFix(symbol, editorSettings, codeGenerationUnit))
                                              .Where(codeFix => codeFix?.CanApplyFix()==true); 

            var actions = renameCodeFixes.Select(codeFix => new RenameChoiceAction(
                codeFix         : codeFix,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action}, 
                title           : "Rename Choice", 
                priority        : SuggestedActionSetPriority.Medium, 
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }
    }
}