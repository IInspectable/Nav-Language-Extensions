#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    abstract class TransitionCodeModel: CodeModel {

        protected TransitionCodeModel(ImmutableList<Call> reachableCalls) {

            if (reachableCalls == null) {
                throw new ArgumentNullException(nameof(reachableCalls));
            }

            var implementedCalls = reachableCalls.Where(c => !c.Node.CodeNotImplemented()).ToList();
            var injectedCalls    = implementedCalls.Where(c => !c.Node.CodeDoNotInject()).ToList();

            var reachableCallsModels = CallCodeModelBuilder.FromCalls(reachableCalls).ToList().OrderBy(c => c.SortOrder);
            var taskBeginModels      = GetTaskBegins(injectedCalls);
            var taskBeginFieldModels = GetTaskBeginFields(injectedCalls);

            ReachableCalls  = reachableCallsModels.ToImmutableList();
            TaskBegins      = taskBeginModels.ToImmutableList();
            TaskBeginFields = taskBeginFieldModels.ToImmutableList();
        }

        public ImmutableList<CallCodeModel>      ReachableCalls  { get; }
        public ImmutableList<ParameterCodeModel> TaskBegins      { get; }
        public ImmutableList<FieldCodeModel>     TaskBeginFields { get; }

        static IEnumerable<ParameterCodeModel> GetTaskBegins(IEnumerable<Call> reachableCalls) {
            
            var taskDeclarations = GetTaskDeclarations(reachableCalls);
            return ParameterCodeModel.GetTaskBeginsAsParameter(taskDeclarations)
                                     .OrderBy(p => p.ParameterName)
                                     .ToImmutableList();
        }

        static IEnumerable<FieldCodeModel> GetTaskBeginFields(IEnumerable<Call> reachableCalls) {
            
            var taskBegins       = GetTaskBegins(reachableCalls);
            var taskBeginMembers = taskBegins.Select(p => new FieldCodeModel(p.ParameterType, p.ParameterName));

            return taskBeginMembers;
        }

        static IEnumerable<ITaskDeclarationSymbol> GetTaskDeclarations(IEnumerable<Call> reachableCalls) {

            return reachableCalls.Select(call => call.Node)
                                 .OfType<ITaskNodeSymbol>()
                                 .Select(node => node.Declaration)
                                 .Where(taskDeclaration => taskDeclaration != null)
                                 .Distinct();
        }

    }

}