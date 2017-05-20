#region Using Directives

using System;
using System.IO;
using System.Linq;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class PathProvider: IPathProvider {
        
        public PathProvider(ITaskDefinitionSymbol taskDefinition) {

            TaskDefinitionSyntax syntax = taskDefinition.Syntax;
            var syntaxFile = syntax.SyntaxTree.FileInfo;
            if (syntaxFile == null) {
                throw new ArgumentException("No FileInfo available", nameof(taskDefinition));
            }
            
            string generateToInfo = null;
            var generateToToken = syntax.CodeGenerateToDeclaration?.StringLiteral ?? SyntaxToken.Missing;
            if (!generateToToken.IsMissing) {
                generateToInfo = generateToToken.ToString();
            }

            TaskName = taskDefinition.Name;
            SyntaxFileName = syntaxFile.FullName;

            IwflGeneratedDirectory = PathCombine(syntaxFile.DirectoryName, "IWFL", generateToInfo, "generated");
            WflGeneratedDirectory  = PathCombine(syntaxFile.DirectoryName, "WFL" , generateToInfo, "generated");
            WflDirectory           = PathCombine(syntaxFile.DirectoryName, "WFL" , generateToInfo);                     
        }

        public string TaskName { get; set; }
        public string WflDirectory { get; set; }
        public string WflGeneratedDirectory { get; set; }
        public string IwflGeneratedDirectory { get; }

        public string SyntaxFileName { get; }
        public string WfsBaseFileName   => PathCombine(WflGeneratedDirectory , TaskName + "WFSBase.generated.cs");
        public string IWfsFileName      => PathCombine(IwflGeneratedDirectory, "I" + TaskName + "WFS.generated.cs");
        public string IBeginWfsFileName => PathCombine(WflGeneratedDirectory , "IBegin" + TaskName + "WFS.generated.cs");
        public string WfsFileName       => PathCombine(WflDirectory          , TaskName + "WFS" + ".cs");
        public string OldWfsFileName    => PathCombine(WflDirectory          , "manual", TaskName + "WFS" + ".cs");

        public string GetRelativePath(string fromPath, string toPath) {
            return PathHelper.GetRelativePath(fromPath, toPath);
        }

        // TODO GetToFileName überprüfen
        public string GetToFileName(string toClassName) {
           return PathCombine(IwflGeneratedDirectory, toClassName + ".cs");
        }

        static string PathCombine(string first, params string[] parts) {
            return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
        }
    }
}