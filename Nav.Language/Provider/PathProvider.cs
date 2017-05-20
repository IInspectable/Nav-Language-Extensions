#region Using Directives

using System;
using System.IO;
using System.Linq;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class PathProvider: IPathProvider {
        
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

            IwflGeneratedDirectory = PathCombine(syntaxFile.DirectoryName, CodeGenFacts.IwflNamespaceSuffix, generateToInfo, CodeGenFacts.GeneratedFolderName);
            WflGeneratedDirectory  = PathCombine(syntaxFile.DirectoryName, CodeGenFacts.WflNamespaceSuffix , generateToInfo, CodeGenFacts.GeneratedFolderName);
            WflDirectory           = PathCombine(syntaxFile.DirectoryName, CodeGenFacts.WflNamespaceSuffix , generateToInfo);                     
        }

        public string TaskName { get; }
        public string WflDirectory { get; }
        public string WflGeneratedDirectory { get; }
        public string IwflGeneratedDirectory { get; }

        public string SyntaxFileName { get; }
        public string WfsBaseFileName   => PathCombine(WflGeneratedDirectory , $"{TaskName}{CodeGenFacts.WfsBaseClassSuffix}.{CodeGenFacts.GeneratedFileNameSuffix}.{CodeGenFacts.CSharpFileExtension}");
        public string IWfsFileName      => PathCombine(IwflGeneratedDirectory, $"I{TaskName}{CodeGenFacts.WfsClassSuffix}.{CodeGenFacts.GeneratedFileNameSuffix}.{CodeGenFacts.CSharpFileExtension}");
        public string IBeginWfsFileName => PathCombine(WflGeneratedDirectory , $"{CodeGenFacts.BeginInterfacePrefix}{TaskName}{CodeGenFacts.WfsClassSuffix}.{CodeGenFacts.GeneratedFileNameSuffix}.{CodeGenFacts.CSharpFileExtension}");
        public string WfsFileName       => PathCombine(WflDirectory          , $"{TaskName}{CodeGenFacts.WfsClassSuffix}.cs");
        public string LegacyWfsFileName => PathCombine(WflDirectory          , CodeGenFacts.LegacyManualFolderName, $"{TaskName}{CodeGenFacts.WfsClassSuffix}.{CodeGenFacts.CSharpFileExtension}");

        public string GetRelativePath(string fromPath, string toPath) {
            return PathHelper.GetRelativePath(fromPath, toPath);
        }

        // TODO GetToFileName überprüfen
        public string GetToFileName(string toClassName) {
           return PathCombine(IwflGeneratedDirectory, $"{toClassName}.{CodeGenFacts.CSharpFileExtension}");
        }

        static string PathCombine(string first, params string[] parts) {
            return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
        }
    }
}