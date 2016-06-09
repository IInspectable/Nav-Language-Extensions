#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    sealed class GoToSymbolBuilder : SymbolVisitor<TagSpan<GoToTag>> {

        readonly SemanticModelResult _semanticModelResult;
        readonly ITextBuffer _textBuffer;

        GoToSymbolBuilder(SemanticModelResult semanticModelResult, ITextBuffer textBuffer) {
            _semanticModelResult = semanticModelResult;
            _textBuffer = textBuffer;
        }

        public static TagSpan<GoToTag> Build(SemanticModelResult semanticModelResult, ISymbol source, ITextBuffer textBuffer) {
            var builder = new GoToSymbolBuilder(semanticModelResult, textBuffer);
            return builder.Visit(source);
        }

        public override TagSpan<GoToTag> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return CreateGoToLocationTagSpan(includeSymbol.Location, includeSymbol.FileLocation);
        }

        public override TagSpan<GoToTag> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration == null) {
                return null;
            }           
            
            return CreateGoToLocationTagSpan(taskNodeSymbol.Location, taskNodeSymbol.Declaration.Location);
        }

        public override TagSpan<GoToTag> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }
            
            return CreateGoToLocationTagSpan(nodeReferenceSymbol.Location, nodeReferenceSymbol.Declaration.Location);
        }

        public override TagSpan<GoToTag> VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol.Declaration == null) {
                return null;
            }
            return CreateGoToLocationTagSpan(connectionPointReferenceSymbol.Location, connectionPointReferenceSymbol.Declaration.Location);
        }

        public override TagSpan<GoToTag> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var info = new SignalTriggerCodeGenInfo(signalTriggerSymbol);
            
            return CreateGoToMemberDeclarationTagSpan(signalTriggerSymbol.Location, info.WfsFullyQualifiedName, info.TriggerMethodName);
        }

        TagSpan<GoToTag> CreateGoToLocationTagSpan(Location sourceLocation, Location targetLocation) {

            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag     = new GoToLocationTag(targetLocation);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }

        TagSpan<GoToTag> CreateGoToMemberDeclarationTagSpan(Location sourceLocation, string fullyQualifiedTypeName, string memberName) {

            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag     = new GoToMemberDeclarationTag(_textBuffer, fullyQualifiedTypeName, memberName);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }        
    }    
}