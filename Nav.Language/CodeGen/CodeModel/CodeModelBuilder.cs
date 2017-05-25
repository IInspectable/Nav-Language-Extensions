using System.Collections.Generic;
using System.Linq;

namespace Pharmatechnik.Nav.Language.CodeGen {
    sealed class CodeModelBuilder {
        public static IEnumerable<InitTransitionCodeModel> GetInitTransitions(ITaskDefinitionSymbol taskDefinition, TaskCodeInfo taskCodeInfo) {
            return taskDefinition.NodeDeclarations
                .OfType<IInitNodeSymbol>().SelectMany(n => n.Outgoings)
                .Select(trans => InitTransitionCodeModel.FromInitTransition(trans, taskCodeInfo));  
        }

        public static IEnumerable<ExitTransitionCodeModel> GetExitTransitions(ITaskDefinitionSymbol taskDefinition) {
            // TODO Exit Transitions müssen pro TaskNode immer zusammengefasst werden
            return taskDefinition.NodeDeclarations
                .OfType<ITaskNodeSymbol>()
                .Select(ExitTransitionCodeModel.FromTaskNode);
        }

        public static IEnumerable<TriggerTransitionCodeModel> GetTriggerTransitions(ITaskDefinitionSymbol taskDefinition, TaskCodeInfo taskCodeInfo) {
            return taskDefinition.NodeDeclarations
                .OfType<IGuiNodeSymbol>()
                .SelectMany(n => n.Outgoings)
                .SelectMany(triggerTransition => TriggerTransitionCodeModel.FromTriggerTransition(taskCodeInfo, triggerTransition))
                .OrderBy(st => st.TriggerMethodName.Length).ThenBy(st => st.TriggerMethodName);            
        }

        public static IEnumerable<BeginWrapperCodeModel> GetBeginWrappers(ITaskDefinitionSymbol taskDefinition) {
            return taskDefinition.NodeDeclarations
                .OfType<ITaskNodeSymbol>()
                .Select(BeginWrapperCodeModel.FromTaskNode);
        }

        public static IEnumerable<string> GetCodeDeclarations(ITaskDefinitionSymbol taskDefinition) {
            var codeDeclaration = taskDefinition.Syntax.CodeDeclaration;
            if (codeDeclaration == null) {
                yield break;
            }
            foreach (var code in codeDeclaration.GetGetStringLiterals().Select(sl => sl.ToString().Trim('"'))) {
                yield return code;
            }
        }
    }
}