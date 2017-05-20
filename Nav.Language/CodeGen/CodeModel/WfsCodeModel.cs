#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class WfsCodeModel : FileGenerationCodeModel {

        public WfsCodeModel(
               TaskCodeModel taskCodeModel, 
               string relativeSyntaxFileName, 
               string filePath,
               ImmutableList<string> usingNamespaces)
            : base(taskCodeModel, relativeSyntaxFileName, filePath) {

            UsingNamespaces = usingNamespaces ?? throw new ArgumentNullException(nameof(usingNamespaces));
        }

        public string WflNamespace => Task.WflNamespace;
        public string WfsTypeName  => Task.WfsTypeName;

        public ImmutableList<string> UsingNamespaces { get; }
        
        public static WfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, IPathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);
            var relativeSyntaxFileName = pathProvider.GetRelativePath(pathProvider.WfsFileName, pathProvider.SyntaxFileName);
            
            return new WfsCodeModel(
                taskCodeModel         : taskCodeModel,
                relativeSyntaxFileName: relativeSyntaxFileName,
                filePath              : pathProvider.WfsFileName,
                usingNamespaces       : GetUsingNamespaces(taskDefinition, taskCodeModel)
               );
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
    }
}