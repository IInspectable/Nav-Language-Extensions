#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    abstract class TransitionCodeModel : CodeModel {

        protected TransitionCodeModel(ImmutableList<Call> reachableCalls) {

            if(reachableCalls == null) {
                throw new ArgumentNullException(nameof(reachableCalls));
            }

            var calls = CallCodeModelBuilder.FromCalls(reachableCalls);

            var reachableNodes = reachableCalls.Select(c => c.Node).ToList();

            var taskBegins      = GetTaskBegins(reachableNodes);
            var taskBeginFields = GetTaskBeginFields(reachableNodes);

            ReachableCalls  = calls.ToImmutableList();
            TaskBegins      = taskBegins.ToImmutableList();
            TaskBeginFields = taskBeginFields.ToImmutableList();
        }        
        public ImmutableList<CallCodeModel> ReachableCalls { get; set; }
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<FieldCodeModel> TaskBeginFields { get; }
        
        static IEnumerable<ParameterCodeModel> GetTaskBegins(IEnumerable<INodeSymbol> reachableNodes) {
            // TODO Sortierung?
            return ParameterCodeModel.GetTaskBeginsAsParameter(GetDistinctTaskDeclarations(reachableNodes))
                                     .OrderBy(p=>p.ParameterName).ToImmutableList();
        }

        static IEnumerable<FieldCodeModel> GetTaskBeginFields(IEnumerable<INodeSymbol> reachableNodes) {
            var taskBegins       = GetTaskBegins(reachableNodes);
            var taskBeginMembers = taskBegins.Select(p => new FieldCodeModel(p.ParameterType, p.ParameterName));
                                             
            return taskBeginMembers;
        }
     
        static IEnumerable<ITaskDeclarationSymbol> GetDistinctTaskDeclarations(IEnumerable<INodeSymbol> nodes) {

            return nodes.OfType<ITaskNodeSymbol>()
                        .Select(node => node.Declaration)
                        .Where(taskDeclaration => taskDeclaration != null)
                        .Distinct();
        }
    }
}