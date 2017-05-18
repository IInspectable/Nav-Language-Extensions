#region Using Directives

using System;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProvider {
        
        public PathProvider(string syntaxFile, string wfsBaseFile, string iWfsInterfaceFile, string iBeginWfsInterfaceFile, string wfsFile, string oldWfsFile) {
            SyntaxFile             = syntaxFile             ?? throw new ArgumentNullException(nameof(syntaxFile));
            WfsBaseFile            = wfsBaseFile            ?? throw new ArgumentNullException(nameof(wfsBaseFile)); 
            IWfsInterfaceFile      = iWfsInterfaceFile      ?? throw new ArgumentNullException(nameof(iWfsInterfaceFile));
            IBeginWfsInterfaceFile = iBeginWfsInterfaceFile ?? throw new ArgumentNullException(nameof(iBeginWfsInterfaceFile));
            WfsFile                = wfsFile                ?? throw new ArgumentNullException(nameof(wfsFile));
            OldWfsFile             = oldWfsFile             ?? throw new ArgumentNullException(nameof(oldWfsFile));
        }

        // ReSharper disable InconsistentNaming
        public string SyntaxFile { get; }
        public string WfsBaseFile { get; }
        public string IWfsInterfaceFile { get; }
        public string IBeginWfsInterfaceFile { get; }
        public string WfsFile { get; }
        public string OldWfsFile { get; }
        // ReSharper restore InconsistentNaming

        public string GetRelativePath(string fromPath, string toPath) {
            return PathHelper.GetRelativePath(fromPath, toPath);
        }        
    }
}