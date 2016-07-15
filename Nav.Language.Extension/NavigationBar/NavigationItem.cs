using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class NavigationItem {

        public NavigationItem(string displayName, int imageIndex, Location location, int navigationPoint, ImmutableList<NavigationItem> children=null) {
            Location        = location;
            NavigationPoint = navigationPoint;
            DisplayName     = displayName;
            ImageIndex      = imageIndex;
            Children        = children?? ImmutableList<NavigationItem>.Empty;
        }

        public string DisplayName { get; }
        public int ImageIndex { get; }
        [CanBeNull]
        public Location Location { get; }
        public int NavigationPoint { get; }

        [NotNull]
        public ImmutableList<NavigationItem> Children { get; set; }

        public int Start {
            get { return Location?.Start ?? 0; }
        }

        public int End {
            get { return Location?.End ?? 0; }
        }
    }
}
