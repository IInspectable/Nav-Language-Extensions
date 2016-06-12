#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToLocationTag : GoToTag, ITag, IEquatable<GoToLocationTag> {

        public GoToLocationTag(string fileName) {
            if (fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            Location = new Location(fileName);
        }

        public GoToLocationTag(Location location) {
            if (location == null) {
                throw new ArgumentNullException(nameof(location));
            }
            Location = location;
        }

        public Location Location { get; }
        
        public override Task<Location> GoToLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            NavLanguagePackage.GoToLocationInPreviewTab(Location);
            return Task.FromResult(Location);
        }

        #region Equality members

        public bool Equals(GoToLocationTag other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Location.Equals(other.Location);
        }

        public static bool operator ==(GoToLocationTag left, GoToLocationTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToLocationTag left, GoToLocationTag right) {
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
            return Equals((GoToLocationTag)obj);
        }

        public override int GetHashCode() {
            return Location.GetHashCode();
        }

        #endregion
    }
}