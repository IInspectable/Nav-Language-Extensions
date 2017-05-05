#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public static class IntroduceChoiceCodeFixProvider {
        
        public static IEnumerable<IntroduceChoiceCodeFix> SuggestCodeFixes([CanBeNull] ISymbol symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            if (symbol == null) {
                return Enumerable.Empty<IntroduceChoiceCodeFix>();
            }

            var provider = new Visitor(
                editorSettings ?? throw new ArgumentNullException(nameof(editorSettings)),
                codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit))
            );

            return provider.Visit(symbol).Where(cf => cf.CanApplyFix());
        }

        sealed class Visitor : SymbolVisitor<IEnumerable<IntroduceChoiceCodeFix>> {

            public Visitor(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                EditorSettings     = editorSettings;
                CodeGenerationUnit = codeGenerationUnit;
            }

            EditorSettings EditorSettings { get; }
            CodeGenerationUnit CodeGenerationUnit { get; }
            
            protected override IEnumerable<IntroduceChoiceCodeFix> DefaultVisit(ISymbol symbol) {
                yield break;
            }

            public override IEnumerable<IntroduceChoiceCodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                yield return new IntroduceChoiceCodeFix(nodeReferenceSymbol, CodeGenerationUnit, EditorSettings);
            }
        }
    }

}