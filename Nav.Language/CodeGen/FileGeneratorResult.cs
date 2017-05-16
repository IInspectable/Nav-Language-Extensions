#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class FileGeneratorResult {

        public FileGeneratorResult(ITaskDefinitionSymbol taskDefinition, FileGeneratorAction action, string fileName) {
            TaskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            FileName       = fileName       ?? throw new ArgumentNullException(nameof(fileName));
            Action         = action;
        }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        public string FileName { get; }
        public FileGeneratorAction Action { get; }
    }
}