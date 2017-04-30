#region Using Directives

using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    sealed class GoToSymbolBuilder : SymbolVisitor<TagSpan<GoToTag>> {

        readonly CodeGenerationUnitAndSnapshot _codeGenerationUnitAndSnapshot;
        readonly ITextBuffer _textBuffer;

        GoToSymbolBuilder(CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextBuffer textBuffer) {
            _codeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot;
            _textBuffer = textBuffer;
        }

        public static TagSpan<GoToTag> Build(CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ISymbol source, ITextBuffer textBuffer) {
            var builder = new GoToSymbolBuilder(codeGenerationUnitAndSnapshot, textBuffer);
            return builder.Visit(source);
        }

        public override TagSpan<GoToTag> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return CreateGoToLocationTagSpan(includeSymbol.Location,
                LocationInfo.FromLocation(
                    location    : includeSymbol.FileLocation, 
                    displayName : includeSymbol.FileName, 
                    imageMoniker: ImageMonikers.Include));
        }

        public override TagSpan<GoToTag> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            if(taskDefinitionSymbol.Syntax.Identifier.IsMissing) {
                return null;
            }

            var codeModel = new TaskCodeModel(taskDefinitionSymbol);
            var provider  = new TaskDeclarationLocationInfoProvider(_textBuffer, codeModel);
            
            return CreateTagSpan(taskDefinitionSymbol.Location, provider);
        }

        public override TagSpan<GoToTag> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            if (taskDeclarationSymbol.IsIncluded || taskDeclarationSymbol.Origin==TaskDeclarationOrigin.TaskDefinition) {
                return null;
            }

            var codeModel = new TaskDeclarationCodeModel(taskDeclarationSymbol);
            var provider  = new TaskIBeginInterfaceDeclarationLocationInfoProvider(_textBuffer, codeModel);

            return CreateTagSpan(taskDeclarationSymbol.Location, provider);
        }

        public override TagSpan<GoToTag> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration == null) {
                return null;
            }           
            
            return CreateGoToLocationTagSpan(taskNodeSymbol.Location,
                LocationInfo.FromLocation(
                    location    : taskNodeSymbol.Declaration.Location, 
                    displayName : $"Task {taskNodeSymbol.Declaration.Name}", 
                    imageMoniker: ImageMonikers.FromSymbol(taskNodeSymbol)));
        }

        public override TagSpan<GoToTag> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }
            
            var tagSpan = CreateGoToLocationTagSpan(nodeReferenceSymbol.Location,
                LocationInfo.FromLocation(
                    location    : nodeReferenceSymbol.Declaration.Location, 
                    displayName : "Node Declaration", 
                    imageMoniker: ImageMonikers.GoToNodeDeclaration));

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
            var codeModel = new TaskExitCodeModel(connectionPointReferenceSymbol);
            var provider  = new TaskExitDeclarationLocationInfoProvider(_textBuffer, codeModel);
            var tagSpan   = CreateTagSpan(connectionPointReferenceSymbol.Location, provider);

            // GoTo Exit Definition
            var defProvider = new SimpleLocationInfoProvider(LocationInfo.FromLocation(
                connectionPointReferenceSymbol.Declaration.Location,
                $"Exit {connectionPointReferenceSymbol.Name}",
                ImageMonikers.ExitConnectionPoint));

            tagSpan.Tag.Provider.Add(defProvider);

            return tagSpan;
        }

        public override TagSpan<GoToTag> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            var codeModel = new TaskBeginCodeModel(initNodeSymbol);
            var provider  = new TaskBeginDeclarationLocationInfoProvider(_textBuffer, codeModel);

            return CreateTagSpan(initNodeSymbol.Location, provider);
        }

        public override TagSpan<GoToTag> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var codeModel = new SignalTriggerCodeModel(signalTriggerSymbol);
            var provider  = new TriggerDeclarationLocationInfoProvider(_textBuffer, codeModel);

            return CreateTagSpan(signalTriggerSymbol.Location, provider);
        }

        TagSpan<GoToTag> CreateGoToLocationTagSpan(Location sourceLocation, LocationInfo targetLocation) {

            var provider = new SimpleLocationInfoProvider(targetLocation);

            return CreateTagSpan(sourceLocation, provider);
        }
        
        TagSpan<GoToTag> CreateTagSpan(Location sourceLocation, ILocationInfoProvider provider) {
            var tagSpan = new SnapshotSpan(_codeGenerationUnitAndSnapshot.Snapshot, sourceLocation.Start, sourceLocation.Length);
            var tag     = new GoToTag(provider);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }
    }    
}