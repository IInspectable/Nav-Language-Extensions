using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    public sealed class LocationItem {

        public LocationResult LocationResult { get; set; }
        public ImageMoniker Image { get; set; }
        public string DisplayString { get; set; }
    }
}
