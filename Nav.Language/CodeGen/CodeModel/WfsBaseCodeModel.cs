#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

sealed class WfsBaseCodeModel : FileGenerationCodeModel {

    WfsBaseCodeModel(TaskCodeInfo taskCodeInfo,
                     string relativeSyntaxFileName,
                     string filePath,
                     ImmutableList<string> usingNamespaces,
                     ParameterCodeModel taskResult,
                     ImmutableList<ParameterCodeModel> taskBegins,
                     ImmutableList<ParameterCodeModel> taskParameter,
                     ImmutableList<InitTransitionCodeModel> initTransitions,
                     ImmutableList<ExitTransitionCodeModel> exitTransitions,
                     ImmutableList<TriggerTransitionCodeModel> triggerTransitions,
                     ImmutableList<BeginWrapperCodeModel> beginWrappers,
                     ImmutableList<BeginWrapperCodeModel> allBeginWrappers)
        : base(taskCodeInfo, relativeSyntaxFileName, filePath) {

        UsingNamespaces    = usingNamespaces    ?? throw new ArgumentNullException(nameof(usingNamespaces));
        TaskResult         = taskResult         ?? throw new ArgumentNullException(nameof(taskResult));
        TaskBegins         = taskBegins         ?? throw new ArgumentNullException(nameof(taskBegins));
        TaskParameter      = taskParameter      ?? throw new ArgumentNullException(nameof(taskParameter));
        InitTransitions    = initTransitions    ?? throw new ArgumentNullException(nameof(initTransitions));
        ExitTransitions    = exitTransitions    ?? throw new ArgumentNullException(nameof(exitTransitions));
        TriggerTransitions = triggerTransitions ?? throw new ArgumentNullException(nameof(triggerTransitions));
        BeginWrappers      = beginWrappers      ?? throw new ArgumentNullException(nameof(beginWrappers));
        AllBeginWrappers   = allBeginWrappers   ?? throw new ArgumentNullException(nameof(allBeginWrappers));

        ViewParameters = TriggerTransitions.DistinctBy(ts => ts.ViewParameter.ParameterType).Select(ts => ts.ViewParameter).ToImmutableList();
    }
        
    public string WflNamespace        => Task.WflNamespace;
    public string WfsBaseTypeName     => Task.WfsBaseTypeName;
    public string WfsTypeName         => Task.WfsTypeName;
    public string WfsBaseBaseTypeName => Task.WfsBaseBaseTypeName;

    public ParameterCodeModel                        TaskResult         { get; }
    public ImmutableList<string>                     UsingNamespaces    { get; }       
    public ImmutableList<ParameterCodeModel>         TaskBegins         { get; }
    public ImmutableList<ParameterCodeModel>         TaskParameter      { get; }
    public ImmutableList<InitTransitionCodeModel>    InitTransitions    { get; }
    public ImmutableList<ExitTransitionCodeModel>    ExitTransitions    { get; }
    public ImmutableList<TriggerTransitionCodeModel> TriggerTransitions { get; }
    public ImmutableList<BeginWrapperCodeModel>      BeginWrappers      { get; }
    public ImmutableList<BeginWrapperCodeModel>      AllBeginWrappers   { get; }

    public ImmutableList<ParameterCodeModel> ViewParameters { get; }

    public static WfsBaseCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider, GenerationOptions options) {

        if (taskDefinition == null) {
            throw new ArgumentNullException(nameof(taskDefinition));
        }

        var taskCodeInfo           = TaskCodeInfo.FromTaskDefinition(taskDefinition);
        var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsBaseFileName, pathProvider.SyntaxFileName);

        var taskResult         = ParameterCodeModel.TaskResult(taskDefinition);
        var usingNamespaces    = GetUsingNamespaces(taskCodeInfo, taskDefinition);
        var taskBegins         = CodeModelBuilder.GetTaskBeginParameter(taskDefinition);
        var taskParameter      = CodeModelBuilder.GetTaskParameter(taskDefinition);
        var initTransitions    = CodeModelBuilder.GetInitTransitions(taskCodeInfo, taskDefinition);
        var exitTransitions    = CodeModelBuilder.GetExitTransitions(taskCodeInfo, taskDefinition);
        var triggerTransitions = CodeModelBuilder.GetTriggerTransitions(taskCodeInfo, taskDefinition);
        var beginWrappers      = CodeModelBuilder.GetBeginWrappers(taskCodeInfo, taskDefinition);
        var allBeginWrappers   = CodeModelBuilder.GetAllBeginWrappers(taskCodeInfo, taskDefinition);

        return new WfsBaseCodeModel(
            taskCodeInfo          : taskCodeInfo,
            relativeSyntaxFileName: relativeSyntaxFileName,
            filePath              : pathProvider.WfsBaseFileName,
            usingNamespaces       : usingNamespaces.ToImmutableList(),
            taskResult            : taskResult,
            taskBegins            : taskBegins.ToImmutableList(),
            taskParameter         : taskParameter.ToImmutableList(),
            initTransitions       : initTransitions.ToImmutableList(),
            exitTransitions       : exitTransitions.ToImmutableList(),
            triggerTransitions    : triggerTransitions.ToImmutableList(),
            beginWrappers         : beginWrappers.ToImmutableList(),
            allBeginWrappers      : allBeginWrappers.ToImmutableList());

    }

    static IEnumerable<string> GetUsingNamespaces(TaskCodeInfo containingTask, ITaskDefinitionSymbol taskDefinition) {

        IEnumerable<string> namespaces = [
            typeof(int).Namespace,
            containingTask.IwflNamespace,
            CodeGenFacts.NavigationEngineIwflNamespace,
            CodeGenFacts.NavigationEngineWflNamespace,
            ..taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces()
        ];

        return namespaces.ToSortedNamespaces();
    }

}