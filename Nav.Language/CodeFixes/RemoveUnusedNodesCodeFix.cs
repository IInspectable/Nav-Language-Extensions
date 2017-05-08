#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public class RemoveUnusedNodesCodeFix : CodeFix {

        internal RemoveUnusedNodesCodeFix(ITaskDefinitionSymbol taskDefinitionSymbol, CodeFixContext context)
            : base(context) {
            TaskDefinition = taskDefinitionSymbol ?? throw new ArgumentNullException(nameof(taskDefinitionSymbol));
        }
        
        public override string Name              => "Remove Unused Nodes";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;
        public ITaskDefinitionSymbol TaskDefinition { get; }
       
        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<INodeSymbol> GetCanditates() {
            return TaskDefinition.NodeDeclarations.Where(n=> n.References.Count==0);
        }
        
        public IList<TextChange> GetTextChanges() {
           
            var textChanges = new List<TextChange?>();

            foreach(var node in GetCanditates()) {
                textChanges.Add(TryRemove(node.Syntax.GetFullExtent()));
            }
            return textChanges.OfType<TextChange>().ToList();
        }     
    }
}