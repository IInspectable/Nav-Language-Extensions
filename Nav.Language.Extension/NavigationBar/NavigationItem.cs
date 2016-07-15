namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class NavigationItem {

        public NavigationItem(string displayName, int imageIndex, Location location, int navigationPoint) {
            Location    = location;
            NavigationPoint = navigationPoint;
            DisplayName = displayName;
            ImageIndex  = imageIndex;
        }

        public string DisplayName { get; }
        public int ImageIndex { get; }
        public Location Location { get; }
        public int NavigationPoint { get; }

        public int Start {
            get { return Location?.Start ?? 0; }
        }

        public int End {
            get { return Location?.End ?? 0; }
        }
    }
}
