#region Using Directives

using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class NavigationBarProjectItemBuilder {
        
        public static ImmutableList<NavigationBarItem> Build(CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            if (codeGenerationUnitAndSnapshot == null) {
                return ImmutableList<NavigationBarItem>.Empty;
            }

            return new[] {
                new NavigationBarItem(
                    displayName: codeGenerationUnitAndSnapshot.Snapshot.TextBuffer.GetContainingProject()?.Name ?? "Miscellaneous Files",
                    imageIndex : NavigationBarImages.Index.ProjectNode)
            }.ToImmutableList();
        }
    }
}