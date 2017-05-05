#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public sealed class AddMissingExitTransitionCodeFix: CodeFix {
        
        internal AddMissingExitTransitionCodeFix(INodeReferenceSymbol targetNode, IConnectionPointSymbol connectionPoint, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(codeGenerationUnit, editorSettings) {

            ConnectionPoint = connectionPoint                           ?? throw new ArgumentNullException(nameof(connectionPoint));
            TargetNode      = targetNode                                ?? throw new ArgumentNullException(nameof(targetNode));
            TaskNode        = targetNode.Declaration as ITaskNodeSymbol ?? throw new ArgumentException(nameof(targetNode));
            
            if (TaskNode.Declaration != ConnectionPoint.TaskDeclaration) {
                throw new ArgumentException();
            }
        }

        public override string Name          => "Add Missing Edge";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        public ITaskNodeSymbol TaskNode { get ; }
        public IConnectionPointSymbol ConnectionPoint { get; }
        public INodeReferenceSymbol TargetNode { get; }
        public ITaskDefinitionSymbol ContainingTask => TaskNode.ContainingTask;

        internal bool CanApplyFix() {

            var templateEdge = GetTemplateEdge();

            // 1. Wir brauchen eine vollständige Kante als "Formatvorlage"
            if (templateEdge?.Source == null || templateEdge.EdgeMode == null || templateEdge.Target == null) {
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

            var textChanges  = new List<TextChange?>();

            var sourceName   = $"{TaskNode.Name}{SyntaxFacts.Colon}{ConnectionPoint.Name}";
            var edgeKeyword  = SyntaxFacts.GoToEdgeKeyword;
            var targetName   = GetApplicableTargetName();
            var templateEdge = GetTemplateEdge();
            // Die neue Exit Transition
            var exitTransition = ComposeEdge(templateEdge, sourceName, edgeKeyword, targetName);
            
            // ReSharper disable once PossibleNullReferenceException Check unter CanApplyFix
            var transitionLine = SyntaxTree.GetTextLineExtent(templateEdge.Source.Start);
            textChanges.Add(TryInsert(transitionLine.Extent.End, $"{exitTransition}{EditorSettings.NewLine}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        public TextExtent TryGetSelectionAfterChanges([CanBeNull] CodeGenerationUnit codegenerationUnit) {
  
            var taskDef    = codegenerationUnit?.TryFindTaskDefinition(TargetNode.Declaration?.ContainingTask.Name);
            var taskNode   = taskDef.TryFindNode<ITaskNodeSymbol>(TaskNode.Name);
            var exitEdge   = taskNode?.Outgoings.FirstOrDefault(e => e.ConnectionPoint?.Name == ConnectionPoint.Name);
            var targetNode = exitEdge?.Target;

            return targetNode?.Location.Extent?? TextExtent.Missing;
        }
        
        IEdge GetTemplateEdge() {
            return TargetNode.Edge;
        }

        string GetApplicableTargetName() {
            var guiNode=TaskNode.ContainingTask.NodeDeclarations.OfType<IGuiNodeSymbol>().FirstOrDefault();
            return guiNode?.Name ?? "TO_BE_FILLED";
        }
    }
}