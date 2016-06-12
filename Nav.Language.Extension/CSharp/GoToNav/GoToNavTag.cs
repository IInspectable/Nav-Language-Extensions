#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pharmatechnik.Nav.Language.Extension.GoTo;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    class GoToNavTag : GoToTag {

        public GoToNavTag(NavTaskInfo navTaskInfo) {
            TaskInfo = navTaskInfo;
        }

        public NavTaskInfo TaskInfo { get; }

        // TODO evtl. sollte die Methode gleich in GoToLocationAsync umgemünzt werden...
        public override Task<Location> GetLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var wpfView = NavLanguagePackage.OpenFileInPreviewTab(TaskInfo.NavFileName);

            var textBuffer = wpfView?.TextBuffer;
            if(textBuffer == null) {
                return null;
            }
            // TODO async
            var syntaxTree = SyntaxTree.ParseText(textBuffer.CurrentSnapshot.GetText(), TaskInfo.NavFileName);
            var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
            if (codeGenerationUnitSyntax == null) {
                return null;
            }

            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

            var task=codeGenerationUnit.Symbols
                                       .OfType<ITaskDefinitionSymbol>()
                                       .FirstOrDefault(t => t.Name == TaskInfo.TaskName);

            var triggerInfo = TaskInfo as NavTriggerInfo;
            if(triggerInfo != null && task != null) {

                var trigger = task.Transitions
                                  .SelectMany(t => t.Triggers)
                                  .FirstOrDefault(t => t.Name == triggerInfo.TriggerName);

                return Task.FromResult(trigger?.Location);
            }

            return Task.FromResult(task?.Syntax.Identifier.GetLocation());
        }
    }
}