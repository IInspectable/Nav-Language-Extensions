#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    sealed class GoToDefinitionSymbolBuilder : SymbolVisitor<TagSpan<GoToDefinitionTag>> {

        readonly SemanticModelResult _semanticModelResult;
        
        GoToDefinitionSymbolBuilder(SemanticModelResult semanticModelResult) {
            _semanticModelResult = semanticModelResult;
        }

        public static TagSpan<GoToDefinitionTag> Build(SemanticModelResult semanticModelResult, ISymbol source) {
            var builder = new GoToDefinitionSymbolBuilder(semanticModelResult);
            return builder.Visit(source);
        }

        public override TagSpan<GoToDefinitionTag> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return CreateTagSpan(includeSymbol.Location, includeSymbol.FileLocation);
        }

        public override TagSpan<GoToDefinitionTag> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration == null) {
                return null;
            }           
            
            return CreateTagSpan(taskNodeSymbol.Location, taskNodeSymbol.Declaration.Location);
        }

        public override TagSpan<GoToDefinitionTag> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }
            
            return CreateTagSpan(nodeReferenceSymbol.Location, nodeReferenceSymbol.Declaration.Location);
        }

        public override TagSpan<GoToDefinitionTag> VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol.Declaration == null) {
                return null;
            }
            return CreateTagSpan(connectionPointReferenceSymbol.Location, connectionPointReferenceSymbol.Declaration.Location);
        }

        TagSpan<GoToDefinitionTag> CreateTagSpan(Location sourceLocation, Location targetLocation) {

            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag     = new GoToDefinitionTag(targetLocation);

            return new TagSpan<GoToDefinitionTag>(tagSpan, tag);
        }
    }
}