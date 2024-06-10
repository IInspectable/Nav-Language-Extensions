using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Pharmatechnik.Nav.Language.CodeGen; 

sealed class CodeModelBuilder {

    public static IEnumerable<InitTransitionCodeModel> GetInitTransitions(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {
        return taskDefinition.NodeDeclarations
                             .OfType<IInitNodeSymbol>()
                              // TODO Was ist mit Inits, die keine Outgoings haben, also eigentlich unbenutzt sind?
                             .Select(initNode => InitTransitionCodeModel.FromInitTransition(containingTask, initNode));
    }
        
    public static IEnumerable<ParameterCodeModel> GetTaskBeginParameter(ITaskDefinitionSymbol taskDefinition) {

        var usedTaskDeclarations = GetImplementedTaskNodes(taskDefinition)
                                  .Select(taskNode => taskNode.Declaration)
                                  .Distinct()
                                  .ToImmutableList();
            
        var taskBegins = ParameterCodeModel.GetTaskBeginsAsParameter(usedTaskDeclarations)
                                           .OrderBy(p => p.ParameterName)
                                           .ToImmutableList();
        return taskBegins;
    }

    public static IEnumerable<BeginWrapperCodeModel> GetBeginWrappers(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {
        // Wir wollen für Tasks, die ausschließlich via Concats aufgerufen werden, keine BeginXY Methoden generieren.
        return GetDirectReachableTaskNodes(taskDefinition).Select(tns => BeginWrapperCodeModel.FromTaskNode(containingTask, tns));
    }

    public static IEnumerable<BeginWrapperCodeModel> GetAllBeginWrappers(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {
        return GetReachableTaskNodes(taskDefinition).Select(tns => BeginWrapperCodeModel.FromTaskNode(containingTask, tns));
    }

    public static IEnumerable<ExitTransitionCodeModel> GetExitTransitions(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {
        return GetReachableTaskNodes(taskDefinition)
              .Where(taskNode => !taskNode.CodeNotImplemented())
              .Select(taskNode => ExitTransitionCodeModel.FromTaskNode(containingTask, taskNode));
    }

    static ImmutableList<ITaskNodeSymbol> GetImplementedTaskNodes(ITaskDefinitionSymbol taskDefinition) {

        var relevantTaskNodes = taskDefinition.NodeDeclarations
                                              .OfType<ITaskNodeSymbol>()
                                              .Where(taskNode => !taskNode.CodeDoNotInject())
                                              .Where(taskNode => !taskNode.CodeNotImplemented())
                                              .Distinct();

        return relevantTaskNodes.ToImmutableList();
    }


    /// <summary>
    ///  Liefert alle direkt, also nicht via Concats aufgerufene Tasks
    /// </summary>
    static ImmutableList<ITaskNodeSymbol> GetDirectReachableTaskNodes(ITaskDefinitionSymbol taskDefinition) {

        var relevantTaskNodes = taskDefinition.NodeDeclarations
                                              .OfType<ITaskNodeSymbol>()
                                              .Where(taskNode => taskNode.Incomings.OfType<IConcatableEdge>().Any(e => e.ConcatTransition == null))
                                              .Distinct();

        return relevantTaskNodes.ToImmutableList();
    }

    static ImmutableList<ITaskNodeSymbol> GetReachableTaskNodes(ITaskDefinitionSymbol taskDefinition) {

        var relevantTaskNodes = taskDefinition.NodeDeclarations
                                              .OfType<ITaskNodeSymbol>()
                                              .Where(taskNode => taskNode.IsReachable())
                                              .Distinct();

        return relevantTaskNodes.ToImmutableList();
    }

    public static IEnumerable<TriggerTransitionCodeModel> GetTriggerTransitions(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {
        // Trigger Transitions sind per Defininition "used"
        return taskDefinition.TriggerTransitions
                             .SelectMany(triggerTransition => TriggerTransitionCodeModel.FromTriggerTransition(containingTask, triggerTransition))
                             .OrderBy(st => st.TriggerName.Length)
                             .ThenBy(st => st.TriggerName);
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

    public static IEnumerable<ParameterCodeModel> GetTaskParameter(ITaskDefinitionSymbol taskDefinition) {
        var code          = GetTaskParameterSyntaxes();
        var taskParameter = ParameterCodeModel.FromParameterSyntaxes(code);
        return taskParameter;

        IEnumerable<ParameterSyntax> GetTaskParameterSyntaxes() {
            var paramList = taskDefinition.Syntax.CodeParamsDeclaration?.ParameterList;
            if(paramList == null) {
                yield break;
            }

            foreach(var p in paramList) {
                yield return p;
            }
        }
    }
    
}