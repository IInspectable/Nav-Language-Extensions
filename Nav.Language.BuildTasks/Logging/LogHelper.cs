#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    static class LogHelper {

        public static string GetFileIdentity(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            if (diag?.Location.FilePath?.ToLower() == fileSpec?.FilePath.ToLower()) {
                return fileSpec?.Identity ?? diag?.Location.FilePath;
            }
            return diag?.Location.FilePath;
        }
    }
}