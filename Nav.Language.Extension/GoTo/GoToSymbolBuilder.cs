#region Using Directives

using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

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
            return CreateGoToLocationTagSpan(includeSymbol.Location,
                LocationInfo.FromLocation(includeSymbol.FileLocation));
        }

        public override TagSpan<GoToTag> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            if(taskDefinitionSymbol.Syntax.Identifier.IsMissing) {
                return null;
            }

            var info     = new TaskCodeGenInfo(taskDefinitionSymbol);
            var provider = new TaskDeclarationLocationInfoProvider(_textBuffer, info);
            
            return CreateTagSpan(taskDefinitionSymbol.Location, provider);
        }
        
        public override TagSpan<GoToTag> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration == null) {
                return null;
            }           
            
            return CreateGoToLocationTagSpan(taskNodeSymbol.Location,
                LocationInfo.FromLocation(taskNodeSymbol.Declaration.Location, $"task {taskNodeSymbol.Declaration.Name}", LocationKind.TaskDefinition));
        }

        public override TagSpan<GoToTag> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }
            
            var tagSpan = CreateGoToLocationTagSpan(nodeReferenceSymbol.Location,
                LocationInfo.FromLocation(nodeReferenceSymbol.Declaration.Location, "Node Declaration", LocationKind.NodeDeclaration));

            var nodeTagSpan = Visit(nodeReferenceSymbol.Declaration);
            if(nodeTagSpan!=null && nodeTagSpan.Tag.Provider.Any()) {
                tagSpan.Tag.Provider.AddRange(nodeTagSpan.Tag.Provider);
            }
     
            return tagSpan;
        }

        public override TagSpan<GoToTag> VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol.Declaration == null) {
                return null;
            }

            // GoTo Exit Declaration
            var info     = new TaskExitCodeGenInfo(connectionPointReferenceSymbol);
            var provider = new TaskExitDeclarationLocationInfoProvider(_textBuffer, info);
            var tagSpan  = CreateTagSpan(connectionPointReferenceSymbol.Location, provider);

            // GoTo Exit Definition
            var defProvider = new SimpleLocationInfoProvider(LocationInfo.FromLocation(
                connectionPointReferenceSymbol.Declaration.Location,
                connectionPointReferenceSymbol.Name, 
                LocationKind.ExitDefinition));

            tagSpan.Tag.Provider.Add(defProvider);

            return tagSpan;
        }

        public override TagSpan<GoToTag> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            var info     = new TaskBeginCodeGenInfo(initNodeSymbol);
            var provider = new TaskBeginDeclarationLocationInfoProvider(_textBuffer, info);

            return CreateTagSpan(initNodeSymbol.Location, provider);
        }

        public override TagSpan<GoToTag> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var info     = new SignalTriggerCodeGenInfo(signalTriggerSymbol);
            var provider = new TriggerDeclarationLocationInfoProvider(_textBuffer, info);

            return CreateTagSpan(signalTriggerSymbol.Location, provider);
        }

        TagSpan<GoToTag> CreateGoToLocationTagSpan(Location sourceLocation, LocationInfo targetLocation) {

            var provider = new SimpleLocationInfoProvider(targetLocation);

            return CreateTagSpan(sourceLocation, provider);
        }
        
        TagSpan<GoToTag> CreateTagSpan(Location sourceLocation, ILocationInfoProvider provider) {
            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.Length);
            var tag     = new GoToTag(provider);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }
    }    
}