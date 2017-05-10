#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationResult {

        public CodeGenerationResult(ITaskDefinitionSymbol taskDefinition, string beginWfsInterfaceCode) {
            TaskDefinition        = taskDefinition        ?? throw new ArgumentNullException(nameof(taskDefinition));
            BeginWfsInterfaceCode = beginWfsInterfaceCode ?? throw new ArgumentNullException(nameof(beginWfsInterfaceCode));
        }

        [NotNull]
        public string BeginWfsInterfaceCode { get; internal set; }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
    }
}