#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    static class GoToImageMonikers {

        /// <summary>
        /// Nav file --> C# xyWFS class
        /// </summary>
        public static ImageMoniker GoToTaskDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// Nav file --> C# AfterXYLogic
        /// </summary>
        public static ImageMoniker GoToTaskExitDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        // C# --> C#
        /// <summary>
        /// C# BeginXYCall --> Implementierung der BeginLogic des aufgerufenen Tasks
        /// </summary>
        public static ImageMoniker GoToInitCallDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// Nav Trogger --> C# 
        /// </summary>
        public static ImageMoniker GoToTriggerDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// C# --> Trigger Definition im Nav File
        /// </summary>
        public static ImageMoniker GoToTriggerDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }
        /// <summary>
        /// C# --> Task Definition im Nav File
        /// </summary>
        public static ImageMoniker GoToTaskDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }
    }
}