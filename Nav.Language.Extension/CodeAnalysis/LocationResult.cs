namespace Pharmatechnik.Nav.Language.Extension.CodeAnalysis {

    public struct LocationResult {

        string _errorMessage;

        public bool IsValid {
            get { return Location != null; }
        }

        public Location Location { get; private set; }

        public string ErrorMessage {
            get { return _errorMessage??string.Empty; }
            private set { _errorMessage = value; }
        }

        public static LocationResult FromError(string errorMessage) {
            return new LocationResult {
                ErrorMessage = errorMessage
            };
        }

        public static LocationResult FromLocation(Location location) {
            return new LocationResult {
                Location = location
            };
        }
    }
}