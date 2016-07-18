#region Using Directives

using System.Collections.Immutable;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class ProjectItemBuilder {


        public static ImmutableList<NavigationItem> Build(SemanticModelResult semanticModelResult) {

            if (semanticModelResult == null) {
                return ImmutableList<NavigationItem>.Empty;
            }

            return new[] {
                new NavigationItem(
                    displayName    : semanticModelResult.Snapshot.TextBuffer.GetContainingProject()?.Name ?? "Miscellaneous Files",
                    imageIndex     : NavigationImages.Index.ProjectNode,
                    location       : null,
                    navigationPoint: -1)
            }.ToImmutableList();
        }
    }
}