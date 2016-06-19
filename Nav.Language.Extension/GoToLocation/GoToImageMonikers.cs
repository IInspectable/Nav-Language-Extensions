#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    static class GoToImageMonikers {

        // Diese blöde Mapping bricht leider das Open-Closed-Prinzip... gehören ohnenhin überarbeitet...
        public static ImageMoniker GetMoniker(LocationKind kind) {
            switch(kind) {

                case LocationKind.TaskDefinition:
                    return TaskDefinition;

                case LocationKind.InitDefinition:
                    return InitDefinition;

                case LocationKind.ExitDefinition:
                    return ExitDefinition;

                case LocationKind.TriggerDefinition:
                    return TriggerDefinition;

                case LocationKind.InitCallDeclaration:
                    return InitCallDeclaration;

                case LocationKind.TaskExitDeclaration:
                    return TaskExitDeclaration;

                case LocationKind.TaskDeclaration:
                    return TaskDeclaration;

                case LocationKind.TriggerDeclaration:
                    return TriggerDeclaration;

                case LocationKind.Unspecified:
                default:
                    return KnownMonikers.GoToDefinition;
            }
        }

        /// <summary>
        /// Nav file --> C# xyWFS class
        /// </summary>
        public static ImageMoniker TaskDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// Nav file --> C# AfterXYLogic
        /// </summary>
        public static ImageMoniker TaskExitDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }
        
        /// <summary>
        /// Nav Trigger --> C# 
        /// </summary>
        public static ImageMoniker TriggerDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// C# --> Trigger Definition im Nav File
        /// </summary>
        public static ImageMoniker TriggerDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }

        /// <summary>
        /// C# --> Task Definition im Nav File
        /// </summary>
        public static ImageMoniker TaskDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }

        /// <summary>
        /// C# --> Init Definition im Nav File
        /// </summary>
        public static ImageMoniker InitDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }

        /// <summary>
        /// C# --> Exit Transition Definition im Nav File
        /// </summary>
        public static ImageMoniker ExitDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }


        // C# --> C#
        /// <summary>
        /// C# BeginXYCall --> Implementierung der BeginLogic des aufgerufenen Tasks
        /// </summary>
        public static ImageMoniker InitCallDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }
    }
}