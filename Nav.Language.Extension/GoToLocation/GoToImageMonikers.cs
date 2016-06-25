#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

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