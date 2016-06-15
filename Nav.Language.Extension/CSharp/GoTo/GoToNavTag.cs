#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToNavTag : IntraTextGoToTag {

        public GoToNavTag(NavTaskAnnotation navTaskAnnotation) {
            TaskAnnotation = navTaskAnnotation;
        }

        public NavTaskAnnotation TaskAnnotation { get; }

        public override ImageMoniker ImageMoniker {
            get { return TaskAnnotation is NavTriggerAnnotation ? GoToImageMonikers.GoToTriggerDefinition : GoToImageMonikers.GoToTaskDefinition; }
        }

        public override object ToolTip {
            get {
                // TODO Evtl. Visitor um Annotations bauen...
                if (TaskAnnotation is NavTriggerAnnotation) {
                    return "Go To Trigger Definition";
                } else if(TaskAnnotation is NavInitAnnotation) {
                    return "Go To Init Definition";
                } else if (TaskAnnotation is NavExitAnnotation) {
                    return "Go To Exit Transition Definition";
                }
                return "Go To Task Definition"; }
        }

        public override async Task<Location> GoToLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var wpfView = NavLanguagePackage.OpenFileInPreviewTab(TaskAnnotation.NavFileName);

            var textBuffer = wpfView?.TextBuffer;
            if(textBuffer == null) {
                return null;
            }

            var location = await Task.Run(() => { 
                
                var syntaxTree = SyntaxTree.ParseText(textBuffer.CurrentSnapshot.GetText(), TaskAnnotation.NavFileName, cancellationToken);
                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if(codeGenerationUnitSyntax == null) {
                    return null;
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                var task = codeGenerationUnit.Symbols
                                             .OfType<ITaskDefinitionSymbol>()
                                             .FirstOrDefault(t => t.Name == TaskAnnotation.TaskName);

                if (task == null) {
                    return null;
                }
                // TODO If's refaktorieren. Evtl. Visitor um Annotations bauen
                var triggerAnnotation = TaskAnnotation as NavTriggerAnnotation;
                if(triggerAnnotation != null) {

                    var trigger = task.Transitions
                                      .SelectMany(t => t.Triggers)
                                      .FirstOrDefault(t => t.Name == triggerAnnotation.TriggerName);

                    return trigger?.Location;
                }

                var exitAnnotation = TaskAnnotation as NavExitAnnotation;
                if(exitAnnotation!=null) {
                    // TODO: Was wollen wir hier eigentlich "markieren"? Die ganze Transition, oder nur die Quelle?
                    var exitTransition = task.ExitTransitions.FirstOrDefault(et=> et.Source?.Name==exitAnnotation.ExitTaskName);
                    return exitTransition?.Location;
                }

                var initAnnotation = TaskAnnotation as NavInitAnnotation;
                if (initAnnotation != null) {

                    var init = task.NodeDeclarations.OfType<IInitNodeSymbol>()
                                   .FirstOrDefault(n => n.Name == initAnnotation.InitName);

                    return init?.Location;
                }

                return task.Syntax.Identifier.GetLocation();

            }, cancellationToken);

            NavLanguagePackage.GoToLocationInPreviewTab(location);

            return location;
        }
    }
}