#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    sealed class IntroduceChoiceCodeFixProvider : SymbolVisitor<IEnumerable<IntroduceChoiceCodeFix>> {

        IntroduceChoiceCodeFixProvider(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            EditorSettings = editorSettings;
            CodeGenerationUnit = codeGenerationUnit;
        }

        EditorSettings EditorSettings { get; }
        CodeGenerationUnit CodeGenerationUnit { get; }

        public static IEnumerable<IntroduceChoiceCodeFix> TryGetCodeFix(ISymbol symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            var provider = new IntroduceChoiceCodeFixProvider(
                editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings)),
                codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit))
            );

            return provider.Visit(symbol).Where(cf => cf.CanApplyFix());
        }

        protected override IEnumerable<IntroduceChoiceCodeFix> DefaultVisit(ISymbol symbol) {
            yield break;
        }

        public override IEnumerable<IntroduceChoiceCodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            yield return new IntroduceChoiceCodeFix(nodeReferenceSymbol, CodeGenerationUnit, EditorSettings);
        }
    }
}