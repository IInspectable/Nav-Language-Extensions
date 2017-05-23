#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    class TransitionCodeModel : CodeModel {

        public TransitionCodeModel(ImmutableList<CallCodeModel> targetNodes) {
            Calls = targetNodes ?? throw new ArgumentNullException(nameof(targetNodes));
        }

        public ImmutableList<CallCodeModel> Calls { get; set; }

        // TODO Sortierung
        protected static IEnumerable<ParameterCodeModel> GetTaskBegins(IEnumerable<INodeSymbol> reachableNodes) {
            return ParameterCodeModel.GetTaskBeginsAsParameter(GetDistinctTaskDeclarations(reachableNodes))
                                     .OrderBy(p => p.ParameterName).ToImmutableList();
        }

        // TODO Sortierung
        protected static IEnumerable<ParameterCodeModel> GetTaskBeginMembers(IEnumerable<INodeSymbol> reachableNodes) {
            var taskBegins       = GetTaskBegins(reachableNodes);
            var taskBeginMembers = taskBegins.Select(p => new ParameterCodeModel(p.ParameterType, $"_{p.ParameterName.ToCamelcase()}"));
            return taskBeginMembers;
        }

        protected static IEnumerable<ITaskDeclarationSymbol> GetDistinctTaskDeclarations(IEnumerable<INodeSymbol> nodes) {

            var set = new HashSet<ITaskDeclarationSymbol>();

            foreach (var taskNode in nodes.OfType<ITaskNodeSymbol>()) {
                set.Add(taskNode.Declaration);
            }
            return set;
        }
    }
}