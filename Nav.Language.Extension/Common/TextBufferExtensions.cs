#region Using Directives

using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextBufferExtensions {

        [CanBeNull]
        public static ITextDocument GetTextDocument(this ITextBuffer textBuffer) {
            ITextDocument textDoc;
            var rc = textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out textDoc);
            return rc ? textDoc : null;
        }

        [CanBeNull]
        public static Project GetContainingProject(this ITextBuffer textBuffer) {

            var projectItem = NavLanguagePackage.DTE.Solution?.FindProjectItem(textBuffer.GetTextDocument()?.FilePath);

            var projectPath = projectItem?.ContainingProject?.FullName;
            if (projectPath == null) {
                return null;
            }

            var project = NavLanguagePackage.Workspace.CurrentSolution?.Projects?.FirstOrDefault(p => p.FilePath.ToLower() == projectPath.ToLower());

            return project;
        }
    }
}