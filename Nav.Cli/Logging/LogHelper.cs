#region Using Directives

using System;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Generator;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Logging {

    static class LogHelper {

        public static string GetFileIdentity(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            //if (diag?.Location.FilePath?.ToLower() == fileSpec?.FilePath.ToLower()) {
            //    return fileSpec?.Identity ?? diag?.Location.FilePath;
            //}
            // TODO �berpr�fen, ob das bereits die L�sung ist
            return PathHelper.GetRelativePath(Environment.CurrentDirectory, diag.Location.FilePath);
            //return diag?.Location.FilePath;
        }
    }
}