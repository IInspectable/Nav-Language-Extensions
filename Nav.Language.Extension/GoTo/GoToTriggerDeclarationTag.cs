#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToTriggerDeclarationTag : GoToTag, ITag,
            IEquatable<GoToTriggerDeclarationTag> {

        readonly string _fullyQualifiedWfsBaseName;
        readonly string _triggerMethodName;
        readonly ITextBuffer _sourceBuffer;

        public GoToTriggerDeclarationTag(ITextBuffer sourceBuffer, string fullyQualifiedWfsBaseName, string triggerMethodName) {

            _sourceBuffer              = sourceBuffer;
            _fullyQualifiedWfsBaseName = fullyQualifiedWfsBaseName;
            _triggerMethodName         = triggerMethodName;
        }

        public override async Task<Location> GoToLocationAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return null;
            }

            var location = await Task.Run(() => {
                //Thread.Sleep(4000);
                var compilation   = project.GetCompilationAsync(cancellationToken).Result;
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(_fullyQualifiedWfsBaseName);
                if(wfsBaseSymbol == null) {
                    return null;
                }

                // Wir kennen de facto nur den Baisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived        = SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken)
                                                 .Result;
                var memberSymbol   = derived?.SelectMany(d=>d.GetMembers(_triggerMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    return null;
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    return null;
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return new Location(textExtent, lineExtent, filePath);

            }, cancellationToken).ConfigureAwait(false);

            NavLanguagePackage.GoToLocationInPreviewTab(location);

            return location;
        }

        static IImmutableSet<T> ToImmutableSet<T>(T item) {
            return new[] { item }.ToImmutableHashSet();
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