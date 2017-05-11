#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationResult {

        public CodeGenerationResult(ITaskDefinitionSymbol taskDefinition, string iBeginWfsInterfaceCode) {
            TaskDefinition         = taskDefinition         ?? throw new ArgumentNullException(nameof(taskDefinition));
            IBeginWfsInterfaceCode = iBeginWfsInterfaceCode ?? throw new ArgumentNullException(nameof(iBeginWfsInterfaceCode));
        }

        [NotNull]
        // ReSharper disable once InconsistentNaming
        public string IBeginWfsInterfaceCode { get; internal set; }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
    }
}