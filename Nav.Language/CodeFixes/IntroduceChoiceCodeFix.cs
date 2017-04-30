#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class IntroduceChoiceCodeFix: CodeFix {

        public IntroduceChoiceCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit, INodeReferenceSymbol nodeReference)
                : base(editorSettings, codeGenerationUnit) {            
            NodeReference  = nodeReference ?? throw new ArgumentNullException(nameof(nodeReference));
        }

        public INodeReferenceSymbol NodeReference { get; }
        public ITaskDefinitionSymbol ContainingTask => NodeReference.Declaration?.ContainingTask;
        
        public override bool CanApplyFix() {

            return NodeReference.Type == NodeReferenceType.Target &&
                   NodeReference.Declaration   != null &&
                   NodeReference.Edge.Source   != null &&
                   NodeReference.Edge.EdgeMode != null &&
                 !(NodeReference.Declaration is IChoiceNodeSymbol) &&
                 !(NodeReference.Edge.Source.Declaration is IChoiceNodeSymbol);
        }

        public string ValidateChoiceName(string choiceName) {

            choiceName = choiceName?.Trim();

            if (!SyntaxFacts.IsValidIdentifier(choiceName)) {
                return DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected.MessageFormat;
            }

            var declaredNodeNames = GetDeclaredNodeNames(ContainingTask);
            if (declaredNodeNames.Contains(choiceName)) {
                return String.Format(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared.MessageFormat, choiceName);
            }

            return null;
        }

        public IList<TextChange> GetTextChanges(string choiceName) {
            
            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            choiceName = choiceName?.Trim();

            var validationMessage = ValidateChoiceName(choiceName);
            if(!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(choiceName));
            }

            var edge       = NodeReference.Edge;
            var edgeMode   = edge.EdgeMode;
            var nodeSymbol = NodeReference.Declaration;

            // ReSharper disable once PossibleNullReferenceException Check ist schon in CanApplyFix passiert
            var nodeDeclarationLine = SyntaxTree.GetTextLineExtent(nodeSymbol.Start);
            var nodeTransitionLine  = SyntaxTree.GetTextLineExtent(NodeReference.End);

            var choiceDeclaration = $"{GetSignificantColumn(nodeDeclarationLine)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenChoiceKeywordAndIdentifier(nodeSymbol)}{choiceName}{SyntaxFacts.Semicolon}";
            var choiceTransition  = $"{GetSignificantColumn(nodeTransitionLine)}{choiceName}{WhiteSpaceBetweenSourceAndEdgeMode(edge, choiceName)}{edge.EdgeMode?.Name}{WhiteSpaceBetweenEdgeModeAndTarget(edge)}{NodeReference.Name}{SyntaxFacts.Semicolon}";

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration: choice NeueChoice;
            textChanges.Add(NewInsert(nodeDeclarationLine.Extent.End, $"{choiceDeclaration}{EditorSettings.NewLine}"));
            // Die Node Reference wird nun umgebogen auf die choice
            textChanges.Add(NewReplace(NodeReference, choiceName));
            // Die Edge der choice ist immer '-->'
            textChanges.Add(NewReplace(edgeMode, SyntaxFacts.GoToEdgeKeyword));
            // Die neue choice Transition 
            textChanges.Add(NewInsert(nodeTransitionLine.Extent.End, $"{choiceTransition}{EditorSettings.NewLine}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        string WhiteSpaceBetweenChoiceKeywordAndIdentifier(INodeSymbol sampleNode) {
            
            var offset = ColumnsBetweenKeywordAndIdentifier(sampleNode, newKeyword: SyntaxFacts.ChoiceKeyword);
            return new String(' ', offset);
        }       
    }
}