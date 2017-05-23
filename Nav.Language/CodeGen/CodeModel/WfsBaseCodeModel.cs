#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class WfsBaseCodeModel : FileGenerationCodeModel {

        WfsBaseCodeModel(TaskCodeModel taskCodeModel, 
                         string relativeSyntaxFileName, 
                         string filePath, 
                         ImmutableList<string> usingNamespaces, 
                         ParameterCodeModel taskResult, 
                         ImmutableList<ParameterCodeModel> taskBegins, 
                         ImmutableList<ParameterCodeModel> taskParameter,
                         ImmutableList<InitTransitionCodeModel> initTransitions,
                         ImmutableList<ExitTransitionCodeModel> exitTransitions,
                         ImmutableList<TriggerTransitionCodeModel> triggerTransitions,
                         ImmutableList<BeginWrapperCodeModel> beginWrappers) 
            : base(taskCodeModel, relativeSyntaxFileName, filePath) {
            
            UsingNamespaces    = usingNamespaces    ?? throw new ArgumentNullException(nameof(usingNamespaces));
            TaskResult         = taskResult         ?? throw new ArgumentNullException(nameof(taskResult));
            TaskBegins         = taskBegins         ?? throw new ArgumentNullException(nameof(taskBegins));
            TaskParameter      = taskParameter      ?? throw new ArgumentNullException(nameof(taskParameter));
            InitTransitions    = initTransitions    ?? throw new ArgumentNullException(nameof(initTransitions));
            ExitTransitions    = exitTransitions    ?? throw new ArgumentNullException(nameof(exitTransitions));
            TriggerTransitions = triggerTransitions ?? throw new ArgumentNullException(nameof(triggerTransitions));
            BeginWrappers      = beginWrappers      ?? throw new ArgumentNullException(nameof(beginWrappers));
        }
        
        public string WflNamespace         => Task.WflNamespace;
        public string WfsBaseTypeName      => Task.WfsBaseTypeName;
        public string WfsTypeName          => Task.WfsTypeName;
        public string WfsBaseBaseTypeName  => Task.WfsBaseBaseTypeName;

        public ParameterCodeModel TaskResult { get; }
        public ImmutableList<string> UsingNamespaces { get; }       
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<ParameterCodeModel> TaskParameter { get; }
        public ImmutableList<InitTransitionCodeModel> InitTransitions { get; }
        public ImmutableList<ExitTransitionCodeModel> ExitTransitions { get; }
        public ImmutableList<TriggerTransitionCodeModel> TriggerTransitions { get; }
        public ImmutableList<BeginWrapperCodeModel> BeginWrappers { get; }

        public static WfsBaseCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);
            var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsBaseFileName, pathProvider.SyntaxFileName);

            var taskResult           = GetTaskResult(taskDefinition);
            var usingNamespaces      = GetUsingNamespaces(taskDefinition, taskCodeModel);
            var taskBegins           = GetTaskBegins(taskDefinition);
            var taskParameter        = GetTaskParameter(taskDefinition);
            var initTransitions      = GetInitTransitions(taskDefinition, taskCodeModel);
            var exitTransitions      = GetExitTransitions(taskDefinition);
            var triggerTransitions   = GetTriggerTransitions(taskDefinition);

            // BeginWrapper
            var beginWrappers = GetBeginWrappers(taskDefinition);

            return new WfsBaseCodeModel(
                taskCodeModel         : taskCodeModel,
                relativeSyntaxFileName: relativeSyntaxFileName,
                filePath              : pathProvider.WfsBaseFileName,
                usingNamespaces       : usingNamespaces.ToImmutableList(),
                taskResult            : taskResult,
                taskBegins            : taskBegins.ToImmutableList(),
                taskParameter         : taskParameter.ToImmutableList(),
                initTransitions       : initTransitions.ToImmutableList(),
                exitTransitions       : exitTransitions.ToImmutableList(),
                triggerTransitions    : triggerTransitions.ToImmutableList(),
                beginWrappers         : beginWrappers.ToImmutableList());
        }

        private static IEnumerable<ParameterCodeModel> GetTaskBegins(ITaskDefinitionSymbol taskDefinition) {
            var usedTaskDeclarations = GetUsedTaskDeclarations(taskDefinition);
            // TODO Sortierung?
            var taskBegins = ParameterCodeModel.GetTaskBeginsAsParameter(usedTaskDeclarations)
                                               .OrderBy(p => p.ParameterName).ToImmutableList();
            return taskBegins;
        }

        static IEnumerable<ParameterCodeModel> GetTaskParameter(ITaskDefinitionSymbol taskDefinition) {
            var code = GetTaskParameterSyntaxes(taskDefinition);
            // TODO Sortierung?
            var taskParameter = ParameterCodeModel.FromParameterSyntaxes(code);
            return taskParameter;
        }

        static IEnumerable<ParameterSyntax> GetTaskParameterSyntaxes(ITaskDefinitionSymbol taskDefinition) {
            var paramList = taskDefinition.Syntax.CodeParamsDeclaration?.ParameterList;
            if (paramList == null) {
                yield break;
            }

            foreach (var p in paramList) {
                yield return p;
            }
        }

        static IEnumerable<InitTransitionCodeModel> GetInitTransitions(ITaskDefinitionSymbol taskDefinition, TaskCodeModel taskCodeModel) {
            return taskDefinition.NodeDeclarations
                                 .OfType<IInitNodeSymbol>().SelectMany(n => n.Outgoings)
                                 .Select(trans => InitTransitionCodeModel.FromInitTransition(trans, taskCodeModel));
        }
        
        static IEnumerable<ExitTransitionCodeModel> GetExitTransitions(ITaskDefinitionSymbol taskDefinition) {
            // TODO
            return taskDefinition.NodeDeclarations
                                 .OfType<ITaskNodeSymbol>()
                                 .SelectMany(n => n.Outgoings)
                                 .Select(ExitTransitionCodeModel.FromExitTransition);
        }

        static IEnumerable<TriggerTransitionCodeModel> GetTriggerTransitions(ITaskDefinitionSymbol taskDefinition) {
            return taskDefinition.NodeDeclarations
                                 .OfType<IGuiNodeSymbol>()
                                 .SelectMany(n => n.Outgoings)
                                 .SelectMany(TriggerTransitionCodeModel.FromTriggerTransition);            
        }

        static IEnumerable<BeginWrapperCodeModel> GetBeginWrappers(ITaskDefinitionSymbol taskDefinition) {
            return taskDefinition.NodeDeclarations
                                 .OfType<ITaskNodeSymbol>()
                                 .Select(BeginWrapperCodeModel.FromTaskNode);
        }

        static IEnumerable<string> GetUsingNamespaces(ITaskDefinitionSymbol taskDefinition, TaskCodeModel taskCodeModel) {

            var namespaces = new List<string>();

            namespaces.Add(typeof(int).Namespace);
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineWflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            return namespaces.ToSortedNamespaces();
        }

        static ParameterCodeModel GetTaskResult(ITaskDefinitionSymbol taskDefinition) {

            var taskDefinitionSyntax = taskDefinition.Syntax;

            CodeResultDeclarationSyntax codeResultDeclarationSyntax = taskDefinitionSyntax.CodeResultDeclaration;

            var parameterType = codeResultDeclarationSyntax?.Result?.Type?.ToString();
            if (String.IsNullOrEmpty(parameterType)) {
                parameterType = CodeGenFacts.DefaultTaskResultType;
            }

            var parameterName = codeResultDeclarationSyntax?.Result?.Identifier.ToString();
            if (String.IsNullOrEmpty(parameterName)) {
                parameterName = CodeGenFacts.DefaultParamterName;
            }

            var taskResult = new ParameterCodeModel(
                parameterType: parameterType,
                parameterName: parameterName.ToCamelcase());

            return taskResult;
        }
        
        static ImmutableList<ITaskDeclarationSymbol> GetUsedTaskDeclarations(ITaskDefinitionSymbol taskDefinition) {

            var relevantTaskNodes = taskDefinition.NodeDeclarations
                                                  .OfType<ITaskNodeSymbol>()
                                                  // TODO Klären, ob wir nicht evtl. doch alle Knoten, also auch die nicht referenzierten, für den Compile relevant sind!
                                                  .Where(taskNode => taskNode.References.Any())
                                                  .Select(taskNode => taskNode.Declaration)
                                                  .Distinct();

            return relevantTaskNodes.ToImmutableList();
        }
    }
}