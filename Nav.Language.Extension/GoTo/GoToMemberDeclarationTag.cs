#region Using Directives

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToMemberDeclarationTag : GoToTag, ITag,
            IEquatable<GoToMemberDeclarationTag> {

        readonly string _fullyQualifiedTypeName;
        readonly string _memberName;
        readonly ITextBuffer _sourceBuffer;

        public GoToMemberDeclarationTag(ITextBuffer sourceBuffer, string fullyQualifiedTypeName, string memberName) {

            _sourceBuffer           = sourceBuffer;
            _fullyQualifiedTypeName = fullyQualifiedTypeName;
            _memberName             = memberName;
        }

        public override Task<Location> GetLocationAsync() {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return Task.FromResult(Location.None);
            }

            return Task.Run(() =>  {
                
                var compilation    = project.GetCompilationAsync().Result;
                var typeSymbol     = compilation?.GetTypeByMetadataName(_fullyQualifiedTypeName);
                var memberSymbol   = typeSymbol?.GetMembers(_memberName).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    return Location.None;
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    return Location.None;
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                var location = new Location(textExtent, lineExtent, filePath);

                return location;
            });
        }

        #region Equality members

        public bool Equals(GoToMemberDeclarationTag other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_fullyQualifiedTypeName, other._fullyQualifiedTypeName) && string.Equals(_memberName, other._memberName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoToMemberDeclarationTag) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((_fullyQualifiedTypeName?.GetHashCode() ?? 0)*397) ^ (_memberName?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(GoToMemberDeclarationTag left, GoToMemberDeclarationTag right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoToMemberDeclarationTag left, GoToMemberDeclarationTag right) {
            return !Equals(left, right);
        }

        #endregion
    }

}