#region Using Directives

using System.Collections.Immutable;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class ProjectItemBuilder {

        public const int CSharpProjectImageIndex = 197;

        public static ImmutableList<NavigationItem> Build(SemanticModelResult semanticModelResult) {

            if (semanticModelResult == null) {
                return ImmutableList<NavigationItem>.Empty;
            }

            return new[] {
                new NavigationItem(
                    displayName    : semanticModelResult.Snapshot.TextBuffer.GetContainingProject()?.Name ?? "Miscellaneous Files",
                    imageIndex     : CSharpProjectImageIndex,
                    location       : null,
                    navigationPoint: -1)
            }.ToImmutableList();
        }
    }
}