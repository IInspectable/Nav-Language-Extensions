#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class WfsCodeModel : FileGenerationCodeModel {

        public WfsCodeModel(
            TaskCodeModel taskCodeModel, 
            string relativeSyntaxFileName, 
            string filePath,
            ImmutableList<string> usingNamespaces,
            ImmutableList<InitTransitionCodeModel> initTransitions,
            ImmutableList<ExitTransitionCodeModel> exitTransitions,
            ImmutableList<TriggerTransitionCodeModel> triggerTransitions)
            : base(taskCodeModel, relativeSyntaxFileName, filePath) {

            UsingNamespaces    = usingNamespaces    ?? throw new ArgumentNullException(nameof(usingNamespaces));
            InitTransitions    = initTransitions    ?? throw new ArgumentNullException(nameof(initTransitions));
            ExitTransitions    = exitTransitions    ?? throw new ArgumentNullException(nameof(exitTransitions));
            TriggerTransitions = triggerTransitions ?? throw new ArgumentNullException(nameof(triggerTransitions));
        }

        public string WflNamespace => Task.WflNamespace;
        public string WfsTypeName  => Task.WfsTypeName;

        public ImmutableList<string> UsingNamespaces { get; }
        public ImmutableList<InitTransitionCodeModel> InitTransitions { get; }
        public ImmutableList<ExitTransitionCodeModel> ExitTransitions { get; }
        public ImmutableList<TriggerTransitionCodeModel> TriggerTransitions { get; }

        public static WfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);
            var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsFileName, pathProvider.SyntaxFileName);

            var initTransitions    = CodeModelBuilder.GetInitTransitions(taskDefinition, taskCodeModel);
            var exitTransitions    = CodeModelBuilder.GetExitTransitions(taskDefinition);
            var triggerTransitions = CodeModelBuilder.GetTriggerTransitions(taskDefinition);

            return new WfsCodeModel(
                taskCodeModel         : taskCodeModel,
                relativeSyntaxFileName: relativeSyntaxFileName,
                filePath              : pathProvider.WfsFileName,
                usingNamespaces       : GetUsingNamespaces(taskDefinition, taskCodeModel).ToImmutableList(),
                initTransitions       : initTransitions.ToImmutableList(),
                exitTransitions       : exitTransitions.ToImmutableList(),
                triggerTransitions    : triggerTransitions.ToImmutableList()
               );
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
    }
}