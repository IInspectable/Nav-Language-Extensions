#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    static class GoToImageMonikers {

        // Diese blöde Mapping bricht leider das Open-Closed-Prinzip...
        public static ImageMoniker GetMoniker(LocationKind kind) {
            switch(kind) {

                // Alles was in Richtung Nav Code geht, gilt als Definition
                case LocationKind.TaskDefinition:
                    return SymbolImageMonikers.TaskDefinition;

                case LocationKind.InitDefinition:
                    return SymbolImageMonikers.InitConnectionPoint;

                case LocationKind.ExitDefinition:
                    return SymbolImageMonikers.ExitConnectionPoint;

                case LocationKind.SignalTriggerDefinition:
                    return SymbolImageMonikers.SignalTrigger;

                // Alles was in Richtung c# Code geht, gilt als Declaration

                case LocationKind.TaskDeclaration:
                    return KnownMonikers.ClassPublic;

                case LocationKind.InitCallDeclaration:
                    return KnownMonikers.MethodPublic;

                case LocationKind.TaskExitDeclaration:
                    return KnownMonikers.MethodPublic;
                    
                case LocationKind.TriggerDeclaration:
                    return KnownMonikers.MethodPublic;

                case LocationKind.NodeDeclaration:
                    return KnownMonikers.GoToReference;

                case LocationKind.Unspecified:
                default:
                    return KnownMonikers.GoToDefinition;
            }
        }

        /// <summary>
        /// Nav file --> C# file
        /// </summary>
        public static ImageMoniker Declaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// C# file --> Nav file
        /// </summary>
        public static ImageMoniker Definition {
            get { return KnownMonikers.GoToDeclaration; }
        }
    }
}