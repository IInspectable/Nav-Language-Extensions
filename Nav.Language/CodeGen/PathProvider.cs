#region Using Directives

using System;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProvider {
        
        public PathProvider(string syntaxFileName, string wfsBaseFileName, string iWfsFileName, string iBeginWfsFileName, string wfsFileName, string oldWfsFileName) {
            SyntaxFileName    = syntaxFileName    ?? throw new ArgumentNullException(nameof(syntaxFileName));
            WfsBaseFileName   = wfsBaseFileName   ?? throw new ArgumentNullException(nameof(wfsBaseFileName)); 
            IWfsFileName      = iWfsFileName      ?? throw new ArgumentNullException(nameof(iWfsFileName));
            IBeginWfsFileName = iBeginWfsFileName ?? throw new ArgumentNullException(nameof(iBeginWfsFileName));
            WfsFileName       = wfsFileName       ?? throw new ArgumentNullException(nameof(wfsFileName));
            OldWfsFileName    = oldWfsFileName    ?? throw new ArgumentNullException(nameof(oldWfsFileName));
        }

        // ReSharper disable InconsistentNaming
        public string SyntaxFileName { get; }
        public string WfsBaseFileName { get; }
        public string IWfsFileName { get; }
        public string IBeginWfsFileName { get; }
        public string WfsFileName { get; }
        public string OldWfsFileName { get; }
        // ReSharper restore InconsistentNaming

        public string GetRelativePath(string fromPath, string toPath) {
            return PathHelper.GetRelativePath(fromPath, toPath);
        }        
    }
}