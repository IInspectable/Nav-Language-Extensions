#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    class TransitionCodeModel : CodeModel {

        public TransitionCodeModel(ImmutableList<Call> reachableCalls) {
            if(reachableCalls == null) {
                throw new ArgumentNullException(nameof(reachableCalls));
            }

            var calls = CallCodeModelBuilder.FromCalls(reachableCalls);

            var reachableNodes = reachableCalls.Select(c => c.Node).ToList();

            var taskBegins      = GetTaskBegins(reachableNodes);
            var taskBeginFields = GetTaskBeginFields(reachableNodes);

            Calls           = calls.ToImmutableList();
            TaskBegins      = taskBegins.ToImmutableList();
            TaskBeginFields = taskBeginFields.ToImmutableList();
        }

        public ImmutableList<CallCodeModel> Calls { get; set; }
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<FieldCodeModel> TaskBeginFields { get; }

        // TODO Sortierung
        static IEnumerable<ParameterCodeModel> GetTaskBegins(IEnumerable<INodeSymbol> reachableNodes) {
            return ParameterCodeModel.GetTaskBeginsAsParameter(GetDistinctTaskDeclarations(reachableNodes))
                                     .OrderBy(p => p.ParameterName).ToImmutableList();
        }

        // TODO Sortierung
        static IEnumerable<FieldCodeModel> GetTaskBeginFields(IEnumerable<INodeSymbol> reachableNodes) {
            var taskBegins       = GetTaskBegins(reachableNodes);
            var taskBeginMembers = taskBegins.Select(p => new FieldCodeModel(p.ParameterType, p.ParameterName));
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