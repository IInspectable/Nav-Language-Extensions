#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
// ReSharper disable InconsistentNaming

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

sealed class IWfsCodeModel : FileGenerationCodeModel {

    IWfsCodeModel(TaskCodeInfo taskCodeInfo, 
                  string relativeSyntaxFileName, 
                  string filePath, 
                  ImmutableList<string> usingNamespaces, 
                  string baseInterfaceName, 
                  ImmutableList<TriggerTransitionCodeModel> triggerTransitions) 

        : base(taskCodeInfo, relativeSyntaxFileName, filePath) {
        UsingNamespaces    = usingNamespaces    ?? throw new ArgumentNullException(nameof(usingNamespaces));
        BaseInterfaceName  = baseInterfaceName  ?? throw new ArgumentNullException(nameof(baseInterfaceName));
        TriggerTransitions = triggerTransitions ?? throw new ArgumentNullException(nameof(triggerTransitions));
    }

    public ImmutableList<string>                     UsingNamespaces    { get; }        
    public string                                    Namespace          => Task.IwflNamespace;
    public string                                    InterfaceName      => Task.IWfsTypeName;
    public string                                    BaseInterfaceName  { get; }
    public ImmutableList<TriggerTransitionCodeModel> TriggerTransitions { get; }

    public static IWfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

        if (taskDefinition == null) {
            throw new ArgumentNullException(nameof(taskDefinition));
        }
        if (pathProvider == null) {
            throw new ArgumentNullException(nameof(pathProvider));
        }

        var taskCodeInfo           = TaskCodeInfo.FromTaskDefinition(taskDefinition);
        var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.IWfsFileName, pathProvider.SyntaxFileName);

        var namespaces         = GetUsingNamespaces(taskDefinition, taskCodeInfo);
        var triggerTransitions = CodeModelBuilder.GetTriggerTransitions(taskDefinition, taskCodeInfo);
            
        return new IWfsCodeModel(
            taskCodeInfo          : taskCodeInfo,
            relativeSyntaxFileName: relativeSyntaxFileName,
            filePath              : pathProvider.IWfsFileName, 
            usingNamespaces       : namespaces.ToImmutableList(), 
            baseInterfaceName     : taskDefinition.Syntax.CodeBaseDeclaration?.IwfsBaseType?.ToString() ?? CodeGenFacts.DefaultIwfsBaseType, 
            triggerTransitions    : triggerTransitions.ToImmutableList());
    }

    private static IEnumerable<string> GetUsingNamespaces(ITaskDefinitionSymbol taskDefinition, TaskCodeInfo taskCodeInfo) {

        var namespaces = new List<string>();

        namespaces.Add(taskCodeInfo.IwflNamespace);
        namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
        namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

        return namespaces.ToSortedNamespaces();
    }
}