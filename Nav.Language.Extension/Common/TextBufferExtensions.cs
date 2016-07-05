#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextBufferExtensions {

        static readonly Logger Logger = Logger.Create(typeof(TextBufferExtensions));

        [CanBeNull]
        public static ITextDocument GetTextDocument(this ITextBuffer textBuffer) {
            ITextDocument textDoc;

            textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out textDoc);

            if(textDoc == null) {
                Logger.Warn($"{nameof(GetTextDocument)}: There's no ITextDocument for the {nameof(ITextBuffer)}");
                return null;
            }

            return textDoc;
        }

        [CanBeNull]
        public static Project GetContainingProject(this ITextBuffer textBuffer) {

            var dteSolution = NavLanguagePackage.DTE.Solution;
            if(dteSolution == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: There's no DTE solution");
                return null;
            }

            var projectItem = dteSolution.FindProjectItem(textBuffer.GetTextDocument()?.FilePath);

            if(projectItem == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Unable to find a DTE project item with the path '{textBuffer.GetTextDocument()?.FilePath}'");
                return null;
            }

            var containingProject = projectItem.ContainingProject;
            if(containingProject == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Project item with the path '{textBuffer.GetTextDocument()?.FilePath}' has no containing project.");
                return null;
            }

            var projectPath = containingProject.FullName;
            if (string.IsNullOrEmpty(projectPath)) {
                Logger.Warn($"{nameof(GetContainingProject)}: Containing project '{containingProject.Name}' for the item with the path '{textBuffer.GetTextDocument()?.FilePath}' has no full path.");
                return null;
            }

            var roslynSolution = NavLanguagePackage.Workspace.CurrentSolution;
            if(roslynSolution == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: No roslyn solution available");
                return null;
            }

            var project = roslynSolution.Projects.FirstOrDefault(p => p.FilePath.ToLower() == projectPath.ToLower());
            if (project == null) {
                Logger.Warn($"{nameof(GetContainingProject)}: Unable to find a roslyn project for the project '{projectPath.ToLower()}'.\nRoslyn Projects:\n{ProjectPaths(roslynSolution.Projects)}");
                return null;
            }
            return project;
        }

        static string ProjectPaths(IEnumerable<Project> projects) {
            return projects.Aggregate(new StringBuilder(), (sb, project) => sb.AppendLine(project.FilePath), sb=>sb.ToString());
        }
    }
}