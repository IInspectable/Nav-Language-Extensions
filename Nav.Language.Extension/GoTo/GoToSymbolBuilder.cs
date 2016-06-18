#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;
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
                LocationInfo.FromLocation(taskNodeSymbol.Declaration.Location));
        }

        public override TagSpan<GoToTag> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }
            
            return CreateGoToLocationTagSpan(nodeReferenceSymbol.Location,
                LocationInfo.FromLocation(nodeReferenceSymbol.Declaration.Location));
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
            var cnProvider = new SimpleLocationInfoProvider(LocationInfo.FromLocation(
                connectionPointReferenceSymbol.Declaration.Location,
                connectionPointReferenceSymbol.Name, 
                SymbolImageMonikers.ExitConnectionPoint));

            tagSpan.Tag.Provider.Add(cnProvider);

            return tagSpan;
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
            var tagSpan = new SnapshotSpan(_semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.End - sourceLocation.Start);
            var tag     = new GoToTag(provider);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }
    }    
}