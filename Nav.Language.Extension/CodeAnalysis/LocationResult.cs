namespace Pharmatechnik.Nav.Language.Extension.CodeAnalysis {

    public struct LocationResult {
        public Location Location { get; private set; }
        public string ErrorMessage { get; private set; }

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