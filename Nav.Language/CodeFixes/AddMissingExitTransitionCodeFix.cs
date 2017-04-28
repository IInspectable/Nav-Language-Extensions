#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class AddMissingExitTransitionCodeFix: CodeFix {
        public INodeReferenceSymbol TargetNode { get; }

        public AddMissingExitTransitionCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit, INodeReferenceSymbol targetNode, IConnectionPointSymbol connectionPoint) 
            : base(editorSettings, codeGenerationUnit) {

            ConnectionPoint = connectionPoint ?? throw new ArgumentNullException(nameof(connectionPoint));
            TargetNode      = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            TaskNode        = targetNode.Declaration as ITaskNodeSymbol ?? throw new ArgumentException(nameof(targetNode));
            
            if (TaskNode.Declaration != ConnectionPoint.TaskDeclaration) {
                throw new ArgumentException();
            }
        }

        public ITaskNodeSymbol TaskNode { get ; }
        public IConnectionPointSymbol ConnectionPoint { get; }
        public ITaskDefinitionSymbol ContainingTask => TaskNode.ContainingTask;

        public override bool CanApplyFix() {

            var transitionReference = GetReferenceEdge();

            // 1. Wir brauchen eine vollständige Transition
            if (transitionReference?.Source == null || transitionReference.EdgeMode == null || transitionReference.Target == null) {
                return false;
            }
            // 2. Es darf noch keine ExitTransition mit dem Verbindungspunkt geben
            return TaskNode.Outgoings
                              .Where(trans=>trans.ConnectionPoint!=null)
                              // ReSharper disable once PossibleNullReferenceException
                              .All(o => o.ConnectionPoint.Declaration != ConnectionPoint);
        }
       
        public IList<TextChange> GetTextChanges() {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            var targetName = GetApplicableTargetName();
            var sourceName = $"{TaskNode.Name}{SyntaxFacts.Colon}{ConnectionPoint.Name}";
            var sampleEdge = GetReferenceEdge();
            // ReSharper disable once PossibleNullReferenceException Check unter CanApplyFix
            var transitionLine = SyntaxTree.GetTextLineExtent(sampleEdge.Source.Start);

            var exitTransition = $"{GetSignificantColumn(transitionLine)}{sourceName}{WhiteSpaceBetweenSourceAndEdgeMode(sampleEdge, sourceName)}{SyntaxFacts.GoToEdgeKeyword}{WhiteSpaceBetweenEdgeModeAndTarget(sampleEdge)}{targetName}{SyntaxFacts.Semicolon}";

            var textChanges = new List<TextChange?>();
            // Die neue Exit Transition
            textChanges.Add(NewInsert(transitionLine.Extent.End, $"{exitTransition}{EditorSettings.NewLine}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        IEdge GetReferenceEdge() {
            return TargetNode.Edge;
        }

        string GetApplicableTargetName() {
            var guiNode=TaskNode.ContainingTask.NodeDeclarations.OfType<IGuiNodeSymbol>().FirstOrDefault();
            return guiNode?.Name?? "TO_BE_FILLED";
        }
    }
}