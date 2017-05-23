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

        protected static IEnumerable<ParameterCodeModel> GetTaskBegins(IEnumerable<INodeSymbol> nodes) {
            // TODO Ordering
            return ParameterCodeModel.GetTaskBeginsAsParameter(GetDistinctTaskDeclarations(nodes))
                                     .OrderBy(p => p.ParameterName).ToImmutableList();
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