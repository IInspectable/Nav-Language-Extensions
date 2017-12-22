#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskExitCodeInfo {

        TaskExitCodeInfo(TaskCodeInfo containingTaskCodeInfo, string taskNodeName) {
            ContainingTaskCodeInfo = containingTaskCodeInfo ?? throw new ArgumentNullException(nameof(containingTaskCodeInfo));
            var nodeNamePascalcase = (taskNodeName ?? String.Empty).ToPascalcase();

            AfterMethodName      = $"{CodeGenFacts.ExitMethodPrefix}{nodeNamePascalcase}";
            AfterLogicMethodName = $"{CodeGenFacts.ExitMethodPrefix}{nodeNamePascalcase}{CodeGenFacts.LogicMethodSuffix}";
        }

        public TaskCodeInfo ContainingTaskCodeInfo { get; }
        public string AfterMethodName              { get; }
        public string AfterLogicMethodName         { get; }

        public static TaskExitCodeInfo FromConnectionPointReference(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol == null) {
                throw new ArgumentNullException(nameof(connectionPointReferenceSymbol));
            }

            var exitTransition = connectionPointReferenceSymbol.ExitTransition;

            var containingTaskCodeInfo = TaskCodeInfo.FromTaskDefinition(exitTransition.ContainingTask);

            return new TaskExitCodeInfo(containingTaskCodeInfo, exitTransition.SourceReference?.Name);
        }

        internal static TaskExitCodeInfo FromTaskNode(ITaskNodeSymbol taskNode, 
                                                      TaskCodeInfo containingTaskCodeInfo) {

            if (taskNode == null) {
                throw new ArgumentNullException(nameof(taskNode));
            }

            containingTaskCodeInfo= containingTaskCodeInfo ?? TaskCodeInfo.FromTaskDefinition( taskNode.ContainingTask);
          
            return new TaskExitCodeInfo(containingTaskCodeInfo, taskNode.Name);            
        }              
    }
}