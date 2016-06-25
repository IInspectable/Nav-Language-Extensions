#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    static class GoToImageMonikers {

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