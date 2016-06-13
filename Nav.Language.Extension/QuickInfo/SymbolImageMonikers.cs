#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    public static class SymbolImageMonikers {

        public static ImageMoniker Include {
            get { return KnownMonikers.ClassFile; }
        }

        public static ImageMoniker TaskDeclaration {
            get { return KnownMonikers.ActivityDiagram; }
        }

        public static ImageMoniker InitConnectionPoint {
            get { return KnownMonikers.InputPin; }
        }

        public static ImageMoniker ExitConnectionPoint {
            get { return KnownMonikers.OutputPin; }
        }

        public static ImageMoniker EndConnectionPoint {
            get { return KnownMonikers.ActivityFinalNode; }
        }

        public static ImageMoniker TaskDefinition {
            get { return KnownMonikers.ActivityDiagram; }
        }

        public static ImageMoniker InitNode {
            get { return KnownMonikers.InputPin; }
        }

        public static ImageMoniker ExitNode {
            get { return KnownMonikers.OutputPin; }
        }

        public static ImageMoniker EndNode {
            get { return KnownMonikers.ActivityFinalNode; }
        }

        public static ImageMoniker TaskNode {
            get { return KnownMonikers.ActivityDiagram; }
        }

        public static ImageMoniker ChoiceNode {
            get { return KnownMonikers.DecisionNode; }
        }

        public static ImageMoniker ViewNode {
            get { return KnownMonikers.WindowsForm; }
        }

        public static ImageMoniker DialogNode {
            get { return KnownMonikers.Dialog; }
        }

        public static ImageMoniker SignalTrigger {
            get { return KnownMonikers.EventTrigger; }
        }
    }
}