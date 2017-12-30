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

            var calls           = CallCodeModelBuilder.FromCalls(reachableCalls);            
            var taskBegins      = GetTaskBegins(reachableCalls);
            var taskBeginFields = GetTaskBeginFields(reachableCalls);

            ReachableCalls  = calls.ToImmutableList();
            TaskBegins      = taskBegins.ToImmutableList();
            TaskBeginFields = taskBeginFields.ToImmutableList();
        }        

        public ImmutableList<CallCodeModel> ReachableCalls { get; }
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<FieldCodeModel> TaskBeginFields { get; }
        
        static IEnumerable<ParameterCodeModel> GetTaskBegins(ImmutableList<Call> reachableCalls) {
            // TODO Sortierung?
            var taskDeclarations = GetTaskDeclarations(reachableCalls);
            return ParameterCodeModel.GetTaskBeginsAsParameter(taskDeclarations)
                                     .OrderBy(p=>p.ParameterName).ToImmutableList();
        }

        static IEnumerable<FieldCodeModel> GetTaskBeginFields(ImmutableList<Call> reachableCalls) {
            var taskBegins       = GetTaskBegins(reachableCalls);
            var taskBeginMembers = taskBegins.Select(p => new FieldCodeModel(p.ParameterType, p.ParameterName));
                                             
            return taskBeginMembers;
        }
     
        static IEnumerable<ITaskDeclarationSymbol> GetTaskDeclarations(ImmutableList<Call> reachableCalls) {

            return reachableCalls.Select(call=>call.Node)
                                 .OfType<ITaskNodeSymbol>()
                                 .Select(node => node.Declaration)
                                 .Where(taskDeclaration => taskDeclaration != null)
                                 .Distinct();
        }
    }
}