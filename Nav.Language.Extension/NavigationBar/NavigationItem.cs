namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class NavigationItem {

        public NavigationItem(string displayName, int imageIndex, Location location) {
            Location    = location;
            DisplayName = displayName;
            ImageIndex  = imageIndex;
        }

        public string DisplayName { get; }
        public int ImageIndex { get; }
        public Location Location { get; }
    }
}
