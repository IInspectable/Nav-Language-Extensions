#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class RemoveUnnecessaryQuotationsCodeFix : CodeFix {

        internal RemoveUnnecessaryQuotationsCodeFix(TransitionDefinitionBlockSyntax transitionDefinitionBlock, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings)
            : base(codeGenerationUnit, editorSettings) {
            TransitionDefinitionBlock = transitionDefinitionBlock ?? throw new ArgumentNullException(nameof(transitionDefinitionBlock));
        }

        public static IEnumerable<RemoveUnnecessaryQuotationsCodeFix> TryGetCodeFixes(SyntaxNode syntaxNode, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            return RemoveUnnecessaryQuotationsCodeFixProvider.TryGetCodeFixes(syntaxNode, codeGenerationUnit, editorSettings);
        }

        public override string Name          => "Remove Unnecessary Quotation Marks";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        public TransitionDefinitionBlockSyntax TransitionDefinitionBlock { get; }
       
        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<StringLiteralSyntax> GetCanditates() {
            return TransitionDefinitionBlock.TransitionDefinitions
                                            .Select(t => t.Trigger)
                                            .OfType<SignalTriggerSyntax>()
                                            .SelectMany(t => t.IdentifierOrStringList)
                                            .OfType<StringLiteralSyntax>();
        }


        public IList<TextChange> GetTextChanges() {
           
            var textChanges = new List<TextChange?>();

            foreach(var stringLiteralSyntax in GetCanditates()) {
                textChanges.Add(TryRemove(TextExtent.FromBounds(stringLiteralSyntax.Start, stringLiteralSyntax.Start + 1)));
                textChanges.Add(TryRemove(TextExtent.FromBounds(stringLiteralSyntax.End-1, stringLiteralSyntax.End)));
            }
            return textChanges.OfType<TextChange>().ToList();
        }            
    }
}