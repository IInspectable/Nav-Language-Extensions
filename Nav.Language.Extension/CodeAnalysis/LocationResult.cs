using Microsoft.VisualStudio.Imaging.Interop;

namespace Pharmatechnik.Nav.Language.Extension.CodeAnalysis {

    public struct LocationResult {

        string _errorMessage;
        string _displayName;

        public bool IsValid {
            get { return Location != null; }
        }

        public Location Location { get; private set; }
        public ImageMoniker Moniker { get; private set; }

        public string DisplayName {
            get {
                if(string.IsNullOrEmpty(_displayName)) {
                    return Location?.FilePath ?? string.Empty;
                }
                return _displayName;
            }
            private set { _displayName = value; }
        }

        public string ErrorMessage {
            get { return _errorMessage??string.Empty; }
            private set { _errorMessage = value; }
        }

        public static LocationResult FromError(string errorMessage) {
            return new LocationResult {
                ErrorMessage = errorMessage
            };
        }

        public static LocationResult FromLocation(Location location, string displayName="", ImageMoniker imageMoniker=default(ImageMoniker)) {
            return new LocationResult {
                Location    = location,
                DisplayName = displayName,
                Moniker     = imageMoniker,
            };
        }
    }
}