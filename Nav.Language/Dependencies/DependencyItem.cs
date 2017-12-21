using System;

namespace Pharmatechnik.Nav.Language.Dependencies {

    public sealed class DependencyItem : IEquatable<DependencyItem> {

        DependencyItem(string taskName, Location location) {
            TaskName = taskName;
            Location = location;
        }
        
        public static DependencyItem FromSymbol(ISymbol symbol) {
            return new DependencyItem(symbol.Name, symbol.Location);
        }
        
        public string TaskName { get; }
        public Location Location { get; }
        
        #region Equality members

        public bool Equals(DependencyItem other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TaskName, other.TaskName) && Equals(Location, other.Location);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DependencyItem item && Equals(item);
        }

        public override int GetHashCode() {
            unchecked {
                return ((TaskName != null ? TaskName.GetHashCode() : 0) * 397) ^ (Location != null ? Location.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DependencyItem left, DependencyItem right) {
            return Equals(left, right);
        }

        public static bool operator !=(DependencyItem left, DependencyItem right) {
            return !Equals(left, right);
        }

        #endregion       
    }
}