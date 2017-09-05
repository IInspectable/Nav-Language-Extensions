#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class PathProviderFactory: IPathProviderFactory {

        public static readonly PathProviderFactory Default = new PathProviderFactory();

        [NotNull]
        public virtual IPathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition) {

            if(taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            TaskDefinitionSyntax syntax = taskDefinition.Syntax;
            var syntaxFile = syntax.SyntaxTree.FileInfo;
            if (syntaxFile == null) {
                throw new ArgumentException("No FileInfo available", nameof(taskDefinition));
            }

            var syntaxFileName = syntaxFile.FullName;
            var taskName = taskDefinition.Name;

            string generateToInfo = null;
            var generateToToken = syntax.CodeGenerateToDeclaration?.StringLiteral ?? SyntaxToken.Missing;
            if (!generateToToken.IsMissing) {
                generateToInfo = generateToToken.ToString();
            }
            
            return new PathProvider(syntaxFileName, taskName, generateToInfo);
        }
    }
}