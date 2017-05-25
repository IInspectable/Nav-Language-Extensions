#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // ReSharper disable once InconsistentNaming
    sealed class IBeginWfsCodeModel : FileGenerationCodeModel {

        IBeginWfsCodeModel(TaskCodeInfo taskCodeInfo, 
                           string relativeSyntaxFileName, 
                           string filePath, 
                           ImmutableList<string> usingNamespaces, 
                           string baseInterfaceName,
                           ImmutableList<InitTransitionCodeModel> initTransitions,
                           ImmutableList<string> codeDeclarations) 
            :base(taskCodeInfo, relativeSyntaxFileName, filePath) {

            UsingNamespaces   = usingNamespaces   ?? throw new ArgumentNullException(nameof(usingNamespaces));
            BaseInterfaceName = baseInterfaceName ?? throw new ArgumentNullException(nameof(baseInterfaceName));
            InitTransitions   = initTransitions   ?? throw new ArgumentNullException(nameof(initTransitions));
            CodeDeclarations  = codeDeclarations  ?? throw new ArgumentNullException(nameof(codeDeclarations));
        }

        public string Namespace => Task.WflNamespace;
        public string BaseInterfaceName { get; }

        public ImmutableList<string> UsingNamespaces { get; }
        public ImmutableList<InitTransitionCodeModel> InitTransitions { get; }
        public ImmutableList<string> CodeDeclarations { get; }
        
        public static IBeginWfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }
            if (pathProvider == null) {
                throw new ArgumentNullException(nameof(pathProvider));
            }

            var taskDefinitionSyntax = taskDefinition.Syntax;
            var taskCodeInfo = TaskCodeInfo.FromTaskDefinition(taskDefinition);

            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeInfo.IwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineWflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            var codeDeclarations = CodeModelBuilder.GetCodeDeclarations(taskDefinition);
            var initTransitions  = CodeModelBuilder.GetInitTransitions(taskDefinition, taskCodeInfo);
            var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.IBeginWfsFileName, pathProvider.SyntaxFileName);
            
            return new IBeginWfsCodeModel(
                taskCodeInfo         : taskCodeInfo,
                relativeSyntaxFileName: relativeSyntaxFileName,
                filePath              : pathProvider.IBeginWfsFileName,
                usingNamespaces       : namespaces.ToSortedNamespaces().ToImmutableList(),
                baseInterfaceName     : taskDefinitionSyntax.CodeBaseDeclaration?.IBeginWfsBaseType?.ToString() ?? CodeGenFacts.DefaultIBeginWfsBaseType,
                initTransitions       : initTransitions.ToImmutableList(),
                codeDeclarations      : codeDeclarations.ToImmutableList());
        }
    }
}