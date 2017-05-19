#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class WfsBaseCodeModel : CodeModel {

        WfsBaseCodeModel(
            TaskCodeModel taskCodeModel,
            ImmutableList<string> usingNamespaces,
            string syntaxFileName,
            string baseClassName, 
            ParameterCodeModel taskResult,
            ImmutableList<ParameterCodeModel> taskBegins) {
            Task            = taskCodeModel   ?? throw new ArgumentNullException(nameof(taskCodeModel));
            UsingNamespaces = usingNamespaces ?? throw new ArgumentNullException(nameof(usingNamespaces));
            SyntaxFileName  = syntaxFileName  ?? String.Empty;
            BaseClassName   = baseClassName   ?? throw new ArgumentNullException(nameof(baseClassName));
            TaskResult      = taskResult      ?? throw new ArgumentNullException(nameof(taskResult));
            TaskBegins      = taskBegins      ?? throw new ArgumentNullException(nameof(taskBegins));
        }

        [NotNull]
        public TaskCodeModel Task { get; }
        [NotNull]
        public string SyntaxFileName { get; }
        [NotNull]
        public ImmutableList<string> UsingNamespaces { get; }

        [NotNull]
        public string Namespace => Task.WflNamespace;

        [NotNull]
        public string ClassName => Task.WfsBaseTypeName;

        [NotNull]
        public string BaseClassName { get; }
        [NotNull]
        public ParameterCodeModel TaskResult { get; }
        [NotNull]
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }

        public static WfsBaseCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, PathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskDefinitionSyntax = taskDefinition.Syntax;
            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);

            // TaskBegins
            var usedTaskDeclarations = GetUsedTaskDeclarations(taskDefinition);
            var taskBegins = ToParameter(usedTaskDeclarations);

            var syntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsBaseFileName, pathProvider.SyntaxFileName);

            return new WfsBaseCodeModel(
                taskCodeModel    : taskCodeModel,
                usingNamespaces  : GetUsingNamespaces(taskDefinition, taskCodeModel),
                syntaxFileName   : syntaxFileName,
                baseClassName    : GetBaseClassName(taskDefinitionSyntax),
                taskResult       : GetTaskResult(taskDefinitionSyntax),
                taskBegins       : taskBegins);
        }

        private static ImmutableList<string> GetUsingNamespaces(ITaskDefinitionSymbol taskDefinition, TaskCodeModel taskCodeModel) {

            var namespaces = new List<string>();

            namespaces.Add(typeof(int).Namespace);
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineWflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            return namespaces.ToSortedNamespaces();
        }

        private static ParameterCodeModel GetTaskResult(TaskDefinitionSyntax taskDefinitionSyntax) {

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

        static string GetBaseClassName(TaskDefinitionSyntax taskDefinitionSyntax) {         
            var baseClassName = taskDefinitionSyntax.CodeBaseDeclaration?.WfsBaseType?.ToString() ?? CodeGenFacts.DefaultWfsBaseClass;
            return baseClassName;
        }

        static ImmutableList<ParameterCodeModel> ToParameter(ImmutableList<ITaskDeclarationSymbol> taskDeclarations) {
            return taskDeclarations.Select(ToParameter).OrderBy(p => p.ParameterType.Length+ p.ParameterName.Length).ToImmutableList();            
        }

        static ParameterCodeModel ToParameter(ITaskDeclarationSymbol taskDeclaration) {

            // If a task is not implemented, there is no IBegin interface for it! - so the generated
            // code will not compile. Therefore, we generate IWFService instead. When the task IS
            // implemented one day, regeneration (necessary for constructor parameters anyway!)
            // will insert the correct types.
            var parameterType = CodeGenFacts.DefaultIwfsBaseType;

            if (!taskDeclaration.CodeNotImplemented) {
                var codeNamespace = String.IsNullOrEmpty(taskDeclaration.CodeNamespace) ? CodeGenFacts.UnknownNamespace : taskDeclaration.CodeNamespace;
                parameterType = $"{codeNamespace}.{CodeGenFacts.WflNamespaceSuffix}.{CodeGenFacts.BeginInterfacePrefix}{taskDeclaration.Name}{CodeGenFacts.WfsClassSuffix}";
            }
           
            var parameterName = taskDeclaration.Name.ToCamelcase();

            return new ParameterCodeModel(parameterType: parameterType, parameterName: parameterName);
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