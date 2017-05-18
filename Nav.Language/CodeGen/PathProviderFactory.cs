#region Using Directives

using System;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProviderFactory {

        public static readonly PathProviderFactory Default = new PathProviderFactory();

        [NotNull]
        public virtual PathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition) {

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
            var wfsFileName = taskName + "WFS";

            return new PathProvider(
                syntaxFileName   : syntaxFile.FullName,
                wfsBaseFileName  : PathCombine(wflGeneratedDirectory , taskName + "WFSBase.generated.cs"), 
                iWfsFileName     : PathCombine(iwflGeneratedDirectory, "I" + taskName + "WFS.generated.cs"), 
                iBeginWfsFileName: PathCombine(wflGeneratedDirectory , "IBegin" + taskName + "WFS.generated.cs"), 
                wfsFileName      : PathCombine(wflDirectory          , wfsFileName + ".cs"), 
                oldWfsFileName   : PathCombine(wflDirectory          , "manual", wfsFileName + ".cs")
            );
        }

        static string PathCombine(string first, params string[] parts) {
            return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
        }
    }
}