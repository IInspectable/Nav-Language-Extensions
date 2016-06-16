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

    public class GoToClassDeclarationTag : GoToTag, ITag, IEquatable<GoToClassDeclarationTag> {

        readonly string _fullyQualifiedTypeName;
        readonly ITextBuffer _sourceBuffer;

        public GoToClassDeclarationTag(ITextBuffer sourceBuffer, string fullyQualifiedTypeName) {

            _sourceBuffer = sourceBuffer;
            _fullyQualifiedTypeName = fullyQualifiedTypeName;
        }

        public override async Task<Location> GetLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return null;
            }

            var location = await LocationFinder.FindClassDeclaration(project, _fullyQualifiedTypeName, cancellationToken)
                                               .ConfigureAwait(false);
           
            return location;
        }

        #region Equality members

        public bool Equals(GoToClassDeclarationTag other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_fullyQualifiedTypeName, other._fullyQualifiedTypeName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoToClassDeclarationTag)obj);
        }

        public override int GetHashCode() {
            return _fullyQualifiedTypeName?.GetHashCode() ?? 0;
        }

        public static bool operator ==(GoToClassDeclarationTag left, GoToClassDeclarationTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToClassDeclarationTag left, GoToClassDeclarationTag right) {
            return !Equals(left, right);
        }

        #endregion
    }
}