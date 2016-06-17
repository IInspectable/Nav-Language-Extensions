#region Using Directives

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToWfsDeclarationTag : GoToTag, ITag, IEquatable<GoToWfsDeclarationTag> {

        readonly string _fullyQualifiedTypeName;
        readonly ITextBuffer _sourceBuffer;

        public GoToWfsDeclarationTag(ITextBuffer sourceBuffer, string fullyQualifiedTypeName) {

            _sourceBuffer = sourceBuffer;
            _fullyQualifiedTypeName = fullyQualifiedTypeName;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError($"Das Projekt konnte nicht ermittelt werden."));
            }

            var location = await LocationFinder.FindWfsDeclarationAsync(project, _fullyQualifiedTypeName, cancellationToken)
                                               .ConfigureAwait(false);
           
            return ToEnumerable(location);
        }

        #region Equality members

        public bool Equals(GoToWfsDeclarationTag other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_fullyQualifiedTypeName, other._fullyQualifiedTypeName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoToWfsDeclarationTag)obj);
        }

        public override int GetHashCode() {
            return _fullyQualifiedTypeName?.GetHashCode() ?? 0;
        }

        public static bool operator ==(GoToWfsDeclarationTag left, GoToWfsDeclarationTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToWfsDeclarationTag left, GoToWfsDeclarationTag right) {
            return !Equals(left, right);
        }

        #endregion
    }    
}