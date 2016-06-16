#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToTriggerDeclarationTag : GoToTag, ITag, IEquatable<GoToTriggerDeclarationTag> {

        readonly string _fullyQualifiedWfsBaseName;
        readonly string _triggerMethodName;
        readonly ITextBuffer _sourceBuffer;

        public GoToTriggerDeclarationTag(ITextBuffer sourceBuffer, string fullyQualifiedWfsBaseName, string triggerMethodName) {

            _sourceBuffer              = sourceBuffer;
            _fullyQualifiedWfsBaseName = fullyQualifiedWfsBaseName;
            _triggerMethodName         = triggerMethodName;
        }

        public override async Task<Location> GetLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return null;
            }

            var location = await LocationFinder.FindTriggerLocation(project, _fullyQualifiedWfsBaseName, _triggerMethodName, cancellationToken)
                                               .ConfigureAwait(false);

            return location;
        }
        
        #region Equality members
        
        public bool Equals(GoToTriggerDeclarationTag other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_fullyQualifiedWfsBaseName, other._fullyQualifiedWfsBaseName) && string.Equals(_triggerMethodName, other._triggerMethodName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoToTriggerDeclarationTag) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((_fullyQualifiedWfsBaseName?.GetHashCode() ?? 0)*397) ^ (_triggerMethodName?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(GoToTriggerDeclarationTag left, GoToTriggerDeclarationTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToTriggerDeclarationTag left, GoToTriggerDeclarationTag right) {
            return !Equals(left, right);
        }

        #endregion
    }
}