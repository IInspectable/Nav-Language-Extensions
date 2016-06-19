namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    // TODO Diese Klasse sollte besser in die CodeAnalysis Assembly. Momentan gibt es allerdings noch eine Abhängigkeit zu den ImageMonikers...

    public enum LocationKind {
        Unspecified,

        TaskDefinition,
        InitDefinition,
        ExitDefinition,
        TriggerDefinition,

        InitCallDeclaration,
        TaskExitDeclaration,
        TaskDeclaration,
        TriggerDeclaration
    }

    public struct LocationInfo {

        string _errorMessage;
        string _displayName;

        public bool IsValid {
            get { return Location != null; }
        }

        public Location Location { get; private set; }
        public LocationKind Kind { get; private set; }

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

        public static LocationInfo FromError(string errorMessage) {
            return new LocationInfo {
                ErrorMessage = errorMessage
            };
        }

        public static LocationInfo FromLocation(Location location, string displayName="", LocationKind kind =LocationKind.Unspecified) {
            return new LocationInfo {
                Location    = location,
                DisplayName = displayName,
                Kind        = kind,
            };
        }
    }
}