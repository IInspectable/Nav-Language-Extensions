#region Using Directives

using System.Collections.Immutable;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class ProjectItemBuilder {
        
        public static ImmutableList<NavigationItem> Build(SemanticModelResult semanticModelResult) {

            if (semanticModelResult == null) {
                return ImmutableList<NavigationItem>.Empty;
            }

            return new[] {
                new NavigationItem(
                    displayName: semanticModelResult.Snapshot.TextBuffer.GetContainingProject()?.Name ?? "Miscellaneous Files",
                    imageIndex : NavigationBarImages.Index.ProjectNode)
            }.ToImmutableList();
        }
    }
}