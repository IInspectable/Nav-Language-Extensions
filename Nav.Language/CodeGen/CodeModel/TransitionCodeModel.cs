﻿#nullable enable

#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen;

// TODO init required
sealed class ContinuationCodeModel: CodeModel {

    public CallCodeModel         Call         { get; init; }
    public BeginWrapperCodeModel BeginWrapper { get; init; }

}

// TODO in eigene files
sealed class CallContextCodeModel: CodeModel {

    public CallContextCodeModel(TaskCodeInfo containingTask, TransitionCodeModel parent, string viewName, ImmutableList<ContinuationCodeModel> continuations) {
        ContainingTask = containingTask ?? throw new ArgumentNullException(nameof(containingTask));
        Parent         = parent;
        ViewName  = viewName;
        Continuations  = continuations ?? throw new ArgumentNullException(nameof(continuations));

    }

    public string                               ClassName      => Parent.GetCallContextClassName();
    public TaskCodeInfo                         ContainingTask { get; }
    public TransitionCodeModel                  Parent         { get; }
    public string                               ViewName       { get; }
    public ImmutableList<ContinuationCodeModel> Continuations  { get; }

}

abstract class TransitionCodeModel: CodeModel {

    protected TransitionCodeModel(TaskCodeInfo containingTask, IEnumerable<Call> reachableCalls) {

        if (reachableCalls == null) {
            throw new ArgumentNullException(nameof(reachableCalls));
        }

        var distinctReachableCalls = reachableCalls.Distinct(CallComparer.FoldExits).ToImmutableList();
        var implementedCalls       = distinctReachableCalls.Where(c => !c.Node.CodeNotImplemented()).ToList();
        var injectedCalls          = implementedCalls.Where(c => !c.Node.CodeDoNotInject()).ToList();

        var reachableCallsModels = CallCodeModelBuilder.FromCalls(distinctReachableCalls)
                                                        // Cancel ist immer implizit erreichbar
                                                       .Concat(new[] { new CancelCallCodeModel() })
                                                       .OrderBy(c => c.SortOrder)
                                                       .ToImmutableArray();

        var taskBeginModels      = GetTaskBegins(injectedCalls);
        var taskBeginFieldModels = GetTaskBeginFields(injectedCalls);

        ReachableCalls  = reachableCallsModels.ToImmutableList();
        TaskBegins      = taskBeginModels.ToImmutableList();
        TaskBeginFields = taskBeginFieldModels.ToImmutableList();

        // TODO Continuation logik wo anders hin? Besser auf IEdges basieren lassen?
        var continuationEdges = distinctReachableCalls.Select(c => new {
                                                           Call         = c,
                                                           Continuation = c.ContinuationCall
                                                       })
                                                      .Where(cc => cc?.Continuation != null)
                                                      .ToImmutableArray();
        if (continuationEdges.Any()) {

            var continuationCalls = continuationEdges.Select(cc => cc.Continuation).ToImmutableArray();
            var guiCall           = continuationEdges.FirstOrDefault()?.Call; // Muss pper Definition ein GUI Node sein. // TODO Nav Error, wenn mehr als eine GUI im Spiel ist

            var viewName=guiCall?.Node.Name??String.Empty;

            var continuationCallCodeModels = CallCodeModelBuilder.FromCalls(continuationCalls);
            var continuationBeginsCodeModels = continuationCalls.Select(c => c?.Node)
                                                                .OfType<ITaskNodeSymbol>()
                                                                .Select(tns => BeginWrapperCodeModel.FromTaskNode(containingTask, tns))
                                                                .ToImmutableList();

         

            var continuations = continuationCallCodeModels.Zip(continuationBeginsCodeModels, (call, bein) => new ContinuationCodeModel {
                                                               Call = call, BeginWrapper = bein
                                                           })
                                                          .ToImmutableList();

            CallContext = new CallContextCodeModel(containingTask, this, viewName,continuations);

        } else {
            CallContext = null;
        }

    }

    public ImmutableList<CallCodeModel>      ReachableCalls  { get; }
    public ImmutableList<ParameterCodeModel> TaskBegins      { get; }
    public ImmutableList<FieldCodeModel>     TaskBeginFields { get; }
    public CallContextCodeModel?             CallContext     { get; }

    public bool EmitCallContext => CallContext != null;

    public abstract string GetCallContextClassName();

    public ParameterCodeModel? CallContextParameter => EmitCallContext ? new(parameterType: GetCallContextClassName(), parameterName: CodeGenFacts.CallContextParamtername) : null;
    
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