#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    sealed class GoToDefinitionSymbolBuilder : SymbolVisitor<TagSpan<GoToDefinitionTag>> {

        readonly SemanticModelResult _semanticModelResult;
        readonly ITextBuffer _textBuffer;

        GoToDefinitionSymbolBuilder(SemanticModelResult semanticModelResult, ITextBuffer textBuffer) {
            _semanticModelResult = semanticModelResult;
            _textBuffer = textBuffer;
        }

        public static TagSpan<GoToDefinitionTag> Build(SemanticModelResult semanticModelResult, ISymbol source, ITextBuffer textBuffer) {
            var builder = new GoToDefinitionSymbolBuilder(semanticModelResult, textBuffer);
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

        public override TagSpan<GoToDefinitionTag> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var info = new SignalTriggerCodeGenInfo(signalTriggerSymbol);
            
            return CreateTagSpan(signalTriggerSymbol.Location, info.WfsFullyQualifiedName, info.TriggerMethodName);
        }

        TagSpan<GoToDefinitionTag> CreateTagSpan(Location sourceLocation, Location targetLocation) {

            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag     = new GoToLocationTag(targetLocation);

            return new TagSpan<GoToDefinitionTag>(tagSpan, tag);
        }

        TagSpan<GoToDefinitionTag> CreateTagSpan(Location sourceLocation, string fullyQualifiedMetadataName, string memberName) {

            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag = new GoToMemberDeclarationTag(fullyQualifiedMetadataName, memberName, _textBuffer);

            return new TagSpan<GoToDefinitionTag>(tagSpan, tag);
        }        
    }    
}