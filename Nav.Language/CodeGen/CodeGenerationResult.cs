using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationResult {

        public CodeGenerationResult(ITaskDefinitionSymbol taskDefinition) {

            if(taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            TaskDefinition = taskDefinition;
        }

        public string BeginWfsInterface { get; internal set; }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }

    }
}