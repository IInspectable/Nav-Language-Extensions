using System;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    public class GoToDefinitionTag: ITag, IEquatable<GoToDefinitionTag> {
        
        public GoToDefinitionTag(string fileName) {
            if(fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            Location = new Location(fileName);
        }

        public GoToDefinitionTag(Location location) {
            if(location == null) {
                throw new ArgumentNullException(nameof(location));
            }
            Location = location;
        }
        
        public Location Location { get; }

        public bool Equals(GoToDefinitionTag other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Location.Equals(other.Location);
        }

        public static bool operator ==(GoToDefinitionTag left, GoToDefinitionTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToDefinitionTag left, GoToDefinitionTag right) {
            return !Equals(left, right);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((GoToDefinitionTag)obj);
        }

        public override int GetHashCode() {
            return Location.GetHashCode();
        }
    }
}