#region Using Directives

using System;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProvider {
        
        public PathProvider(string wfsBaseFile, string iWfsInterfaceFile, string iBeginWfsInterfaceFile, string wfsFile, string oldWfsFile) {
            WfsBaseFile            = wfsBaseFile            ?? throw new ArgumentNullException(nameof(wfsBaseFile)); 
            IWfsInterfaceFile      = iWfsInterfaceFile      ?? throw new ArgumentNullException(nameof(iWfsInterfaceFile));
            IBeginWfsInterfaceFile = iBeginWfsInterfaceFile ?? throw new ArgumentNullException(nameof(iBeginWfsInterfaceFile));
            WfsFile                = wfsFile                ?? throw new ArgumentNullException(nameof(wfsFile));
            OldWfsFile             = oldWfsFile             ?? throw new ArgumentNullException(nameof(oldWfsFile));
        }

        // ReSharper disable InconsistentNaming
        public string WfsBaseFile { get; }
        public string IWfsInterfaceFile { get; }
        public string IBeginWfsInterfaceFile { get; }
        public string WfsFile { get; }
        public string OldWfsFile { get; }
        // ReSharper restore InconsistentNaming

        // TODO CodeModel übergeben?
        [NotNull]
        public static PathProvider FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            TaskDefinitionSyntax syntax = taskDefinition.Syntax;
            var syntaxFile = syntax.SyntaxTree.FileInfo;
            if (syntaxFile == null) {
                throw new ArgumentException("No FileInfo available", nameof(taskDefinition));}

            
            string generateToInfo = null;
            var generateToToken = syntax.CodeGenerateToDeclaration?.StringLiteral??SyntaxToken.Missing;
            if (!generateToToken.IsMissing) {
                generateToInfo = generateToToken.ToString();
            }
            
            var iwflGeneratedDirectory = PathCombine(syntaxFile.DirectoryName, "IWFL", generateToInfo, "generated");
            var wflGeneratedDirectory  = PathCombine(syntaxFile.DirectoryName, "WFL" , generateToInfo, "generated"); 
            var wflDirectory           = PathCombine(syntaxFile.DirectoryName, "WFL" , generateToInfo);

            var taskName = taskDefinition.Name;
            var concreteClassName = taskName + "WFS";

            return new PathProvider(
                wfsBaseFile           : PathCombine(wflGeneratedDirectory , taskName + "WFSBase.generated.cs"), 
                iWfsInterfaceFile     : PathCombine(iwflGeneratedDirectory, "I" + taskName + "WFS.generated.cs"), 
                iBeginWfsInterfaceFile: PathCombine(wflGeneratedDirectory , "IBegin" + taskName + "WFS.generated.cs"), 
                wfsFile               : PathCombine(wflDirectory          , concreteClassName + ".cs"), 
                oldWfsFile            : PathCombine(wflDirectory          , "manual", concreteClassName + ".cs")
                );
        }

        static string PathCombine(string first, params string[] parts) {
            return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
        }
    }
}