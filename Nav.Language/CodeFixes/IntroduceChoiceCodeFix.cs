#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class IntroduceChoiceCodeFix: CodeFix {

        internal IntroduceChoiceCodeFix(INodeReferenceSymbol nodeReference, CodeFixContext context)
            : base(context) {
            NodeReference = nodeReference ?? throw new ArgumentNullException(nameof(nodeReference));
        }

       
        public override string Name          => "Introduce Choice";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        public INodeReferenceSymbol NodeReference { get; }
        public ITaskDefinitionSymbol ContainingTask => NodeReference.Declaration?.ContainingTask;

        public string SuggestChoiceName() {
            string baseName   = $"Choice_{NodeReference.Name}";
            string choiceName = baseName;
            int number = 1;
            while(!String.IsNullOrEmpty(ValidateChoiceName(choiceName))) {
                choiceName = $"{baseName}{number++}";
            }
            return choiceName;
        }

        internal bool CanApplyFix() {

            return NodeReference.Type == NodeReferenceType.Target &&
                   NodeReference.Declaration   != null &&
                   NodeReference.Edge.Source   != null &&
                   NodeReference.Edge.EdgeMode != null &&
                 !(NodeReference.Declaration is IChoiceNodeSymbol) &&
                 !(NodeReference.Edge.Source.Declaration is IChoiceNodeSymbol);
        }

        public string ValidateChoiceName(string choiceName) {     
            return ContainingTask.ValidateNewNodeName(choiceName);
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

            var choiceDeclaration = $"{GetLineIndent(nodeDeclarationLine)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenChoiceKeywordAndIdentifier(nodeSymbol)}{choiceName}{SyntaxFacts.Semicolon}";
            var choiceTransition  = $"{GetLineIndent(nodeTransitionLine)}{choiceName}{WhiteSpaceBetweenSourceAndEdgeMode(edge, choiceName)}{edge.EdgeMode?.Name}{WhiteSpaceBetweenEdgeModeAndTarget(edge)}{NodeReference.Name}{SyntaxFacts.Semicolon}";

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration: choice NeueChoice;
            textChanges.Add(TryInsert(nodeDeclarationLine.Extent.End, $"{choiceDeclaration}{Context.EditorSettings.NewLine}"));
            // Die Node Reference wird nun umgebogen auf die choice
            textChanges.Add(TryRename(NodeReference, choiceName));
            // Die Edge der choice ist immer '-->'
            textChanges.Add(TryRename(edgeMode, SyntaxFacts.GoToEdgeKeyword));
            // Die neue choice Transition 
            textChanges.Add(TryInsert(nodeTransitionLine.Extent.End, $"{choiceTransition}{Context.EditorSettings.NewLine}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        string WhiteSpaceBetweenChoiceKeywordAndIdentifier(INodeSymbol sampleNode) {
            
            var offset = ColumnsBetweenKeywordAndIdentifier(sampleNode, newKeyword: SyntaxFacts.ChoiceKeyword);
            return new String(' ', offset);
        }
    }
}