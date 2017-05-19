#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // ReSharper disable once InconsistentNaming
    public sealed class TOCodeModel : FileGenerationCodeModel {
        
        TOCodeModel(string syntaxFilePath, TaskCodeModel taskCodeModel, string className, string filePath) 
            : base(taskCodeModel, syntaxFilePath, filePath) {
            ClassName = className ?? String.Empty;
        }

        public string ClassName { get; }

        [NotNull]
        public string Namespace => Task.IwflNamespace;

        public static IEnumerable<TOCodeModel> FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, PathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }
            if (pathProvider == null) {
                throw new ArgumentNullException(nameof(pathProvider));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);
            foreach(var guiNode in taskDefinition.NodeDeclarations.OfType<IGuiNodeSymbol>().Where(n => n.References.Any())) {

                var viewName = guiNode.Name;
                var toClassName = $"{viewName.ToPascalcase()}{CodeGenFacts.ToClassNameSuffix}";
                var filePath = pathProvider.GetToFileName(guiNode.Name+ CodeGenFacts.ToClassNameSuffix);

                var syntaxFileName = pathProvider.GetRelativePath(filePath, pathProvider.SyntaxFileName);

                yield return new TOCodeModel(
                    syntaxFilePath: syntaxFileName,
                    taskCodeModel : taskCodeModel,
                    className     : toClassName,
                    filePath      : pathProvider.IWfsFileName);
            }           
        }
    }
}