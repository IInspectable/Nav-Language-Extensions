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
                var extent = ExpandToLeadingAndTrailingTrivia(node.Syntax);
                textChanges.Add(TryRemove(extent));
            }
            return textChanges.OfType<TextChange>().ToList();
        }

        TextExtent ExpandToLeadingAndTrailingTrivia(SyntaxNode node) {
            
            var start = GetLeadingTriviaExtent(node).Start;
            var end   = GetTrailingTriviaExtent(node).End;
          
            return TextExtent.FromBounds(start, end);
        }

        // TODO GetLeadingTriviaExtent => nach SyntaxNode
        private TextExtent GetLeadingTriviaExtent(SyntaxNode node) {
            var startLine = SyntaxTree.GetTextLineExtent(node.Start);

            var start = startLine.Extent.Start;
            var end   = node.Start;
            
            var leadingExtent = TextExtent.FromBounds(startLine.Extent.Start, node.Start);
            var nwsToken = SyntaxTree.Tokens[leadingExtent]
                                     .Reverse()
                                     .SkipWhile(token => token.Classification == SyntaxTokenClassification.Whitespace || token.Classification == SyntaxTokenClassification.Comment)
                                     .FirstOrDefault();
            if (!nwsToken.IsMissing) {
                start = nwsToken.End;
            }

            return TextExtent.FromBounds(start, end);
        }

        // TODO GetTrailingTriviaExtent => nach SyntaxNode
        TextExtent GetTrailingTriviaExtent(SyntaxNode node) {
            
            var endLine = SyntaxTree.GetTextLineExtent(node.End);

            var start = node.Start;
            var end   = endLine.Extent.End;

            var trailingExtent = TextExtent.FromBounds(node.End, endLine.Extent.End);

            var endToken=SyntaxTree.Tokens[trailingExtent]
                                   .SkipWhile(token => token.Classification == SyntaxTokenClassification.Whitespace || token.Classification == SyntaxTokenClassification.Comment)
                                   .FirstOrDefault();

            if (!endToken.IsMissing) {
                end = endToken.Type == SyntaxTokenType.NewLine ? endToken.End : endToken.Start;
            }

            return TextExtent.FromBounds(start, end);
        }
    }
}