#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // TODO separates Model für OnShot Datei (Zwecks Dateinamen)
    public sealed class WfsBaseCodeModel : FileGenerationCodeModel {

        WfsBaseCodeModel(TaskCodeModel taskCodeModel, 
                         string relativeSyntaxFileName, 
                         string filePath, 
                         ImmutableList<string> usingNamespaces, 
                         ParameterCodeModel taskResult, 
                         ImmutableList<ParameterCodeModel> taskBegins, 
                         ImmutableList<ParameterCodeModel> taskParameter,
                         ImmutableList<TaskInitCodeModel> taskInits,
                         ImmutableList<BeginWrapperCodeModel> beginWrappers) 
            : base(taskCodeModel, relativeSyntaxFileName, filePath) {
            
            UsingNamespaces = usingNamespaces ?? throw new ArgumentNullException(nameof(usingNamespaces));
            TaskResult      = taskResult      ?? throw new ArgumentNullException(nameof(taskResult));
            TaskBegins      = taskBegins      ?? throw new ArgumentNullException(nameof(taskBegins));
            TaskParameter   = taskParameter   ?? throw new ArgumentNullException(nameof(taskParameter));
            TaskInits       = taskInits       ?? throw new ArgumentNullException(nameof(taskInits));
            BeginWrappers   = beginWrappers   ?? throw new ArgumentNullException(nameof(beginWrappers));
        }
        
        public string WflNamespace         => Task.WflNamespace;
        public string WfsBaseTypeName      => Task.WfsBaseTypeName;
        public string WfsTypeName          => Task.WfsTypeName;
        public string WfsBaseBaseTypeName  => Task.WfsBaseBaseTypeName;

        public ParameterCodeModel TaskResult { get; }
        public ImmutableList<string> UsingNamespaces { get; }       
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<ParameterCodeModel> TaskParameter { get; }
        public ImmutableList<TaskInitCodeModel> TaskInits { get; }
        public ImmutableList<BeginWrapperCodeModel> BeginWrappers { get; }

        public static WfsBaseCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskDefinitionSyntax = taskDefinition.Syntax;
            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);
            var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsBaseFileName, pathProvider.SyntaxFileName);

            // TaskBegins
            var usedTaskDeclarations = GetUsedTaskDeclarations(taskDefinition);
            var taskBegins = ToParameter(usedTaskDeclarations);
            
            // Task Parameter
            var code = GetTaskParameter(taskDefinition);
            var taskParameter = ToParameter(code);

            // Inits
            var taskInits = new List<TaskInitCodeModel>();
            foreach (var initNode in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
                var taskInit = TaskInitCodeModel.FromInitNode(initNode, taskCodeModel);
                taskInits.Add(taskInit);
            }

            // BeginWrapper
            var beginWrappers = GetBeginWrappers(taskDefinition);

            return new WfsBaseCodeModel(
                taskCodeModel         : taskCodeModel,
                relativeSyntaxFileName: relativeSyntaxFileName,
                filePath              : pathProvider.WfsBaseFileName,
                usingNamespaces       : GetUsingNamespaces(taskDefinition, taskCodeModel),
                taskResult            : GetTaskResult(taskDefinitionSyntax),
                taskBegins            : taskBegins,
                taskParameter         : taskParameter,
                taskInits             : taskInits.ToImmutableList(),
                beginWrappers         : beginWrappers.ToImmutableList());
        }

        private static IEnumerable<BeginWrapperCodeModel> GetBeginWrappers(ITaskDefinitionSymbol taskDefinition) {

            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {
                yield return BeginWrapperCodeModel.FromTaskNode(taskNode);
            }
        }

        static ImmutableList<string> GetUsingNamespaces(ITaskDefinitionSymbol taskDefinition, TaskCodeModel taskCodeModel) {

            var namespaces = new List<string>();

            namespaces.Add(typeof(int).Namespace);
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineWflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            return namespaces.ToSortedNamespaces();
        }

        static ParameterCodeModel GetTaskResult(TaskDefinitionSyntax taskDefinitionSyntax) {

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

        static IEnumerable<ParameterSyntax> GetTaskParameter(ITaskDefinitionSymbol taskDefinition) {
            var paramList = taskDefinition.Syntax.CodeParamsDeclaration?.ParameterList;
            if (paramList == null) {
                yield break;
            }

            foreach (var p in paramList) {
                yield return p;
            }
        }

        static ImmutableList<ParameterCodeModel> ToParameter(IEnumerable<ParameterSyntax> parameters) {
            // TODO Vermutlich darf nur nach Name sortiert werden
            return ParameterCodeModel.FromParameterSyntax(parameters);
        }

        static ImmutableList<ParameterCodeModel> ToParameter(ImmutableList<ITaskDeclarationSymbol> taskDeclarations) {
            // TODO Vermutlich darf nur nach Name sortiert werden
            return taskDeclarations.Select(ToParameter).OrderBy(p => p.ParameterType.Length+ p.ParameterName.Length).ToImmutableList();            
        }

        static ParameterCodeModel ToParameter(ITaskDeclarationSymbol taskDeclaration) {
            return ParameterCodeModel.FromTaskDeclaration(taskDeclaration);           
        }
        
        static ImmutableList<ITaskDeclarationSymbol> GetUsedTaskDeclarations(ITaskDefinitionSymbol taskDefinition) {

            var relevantTaskNodes = taskDefinition.NodeDeclarations
                                                  .OfType<ITaskNodeSymbol>()
                                                  // TODO Klären, ob wir nicht evtl. doch alle Knoten, also auch die nicht referenzierten, für den Compile relevant sind!
                                                  .Where(taskNode => taskNode.References.Any())
                                                  .Select(taskNode => taskNode.Declaration)
                                                  .DistinctBy(declaration => declaration?.Name ?? String.Empty);

            return relevantTaskNodes.ToImmutableList();
        }
    }
}