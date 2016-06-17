#region Using Directives

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToNavTaskAnnotationTag : IntraTextGoToTag {

        public GoToNavTaskAnnotationTag(NavTaskAnnotation navTaskAnnotation): base(new NavTaskAnnotationLocationInfoProvider(navTaskAnnotation)) {
            TaskAnnotation = navTaskAnnotation;
        }

        public NavTaskAnnotation TaskAnnotation { get; }

        public override ImageMoniker ImageMoniker {
            get { return TaskAnnotation is NavTriggerAnnotation ? GoToImageMonikers.GoToTriggerDefinition : GoToImageMonikers.GoToTaskDefinition; }
        }

        public override object ToolTip {
            get {
                // TODO Tooltip Texte zentralisieren
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
    }
}