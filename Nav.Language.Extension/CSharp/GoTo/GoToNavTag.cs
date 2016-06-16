#region Using Directives

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;
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

        public override async Task<LocationResult> GetLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var textBuffer = NavLanguagePackage.GetOpenTextBufferForFile(TaskAnnotation.NavFileName);
            string sourceText;
            if (textBuffer != null) {
                sourceText = textBuffer.CurrentSnapshot.GetText();
            } else {
                sourceText = File.ReadAllText(TaskAnnotation.NavFileName);
            }

            var location = await LocationFinder.FindNavLocationAsync(sourceText, TaskAnnotation, cancellationToken)
                                               .ConfigureAwait(false);

            return location;
        }    
    }
}