#nullable enable

#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen;

sealed class CallCodeModelBuilder: SymbolVisitor<CallCodeModel> {

    public CallCodeModelBuilder(EdgeMode edgeMode, CallCodeModel? continuationCall) {
        EdgeMode         = edgeMode;
        ContinuationCall = continuationCall;
    }

    public EdgeMode       EdgeMode         { get; }
    public CallCodeModel? ContinuationCall { get; }

    public static IEnumerable<CallCodeModel> FromCalls(IEnumerable<Call> calls) {
        return calls.Select(GetCallCodeModel);
    }

    static CallCodeModel GetCallCodeModel(Call call) {

        var continuationCall = TryGetContinuationCall(call);
        var builder          = new CallCodeModelBuilder(call.EdgeMode.EdgeMode, continuationCall);

        return builder.Visit(call.Node);

        static CallCodeModel? TryGetContinuationCall(Call call) {
            if (call.ContinuationCall is { } conti) {
                var builderConti = new CallCodeModelBuilder(conti.EdgeMode.EdgeMode, continuationCall: null);
                return builderConti.Visit(conti.Node);
            }

            return null;
        }
    }

    public override CallCodeModel VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
        return new ExitCallCodeModel(exitNodeSymbol.Name, EdgeMode);
    }

    public override CallCodeModel VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
        return new EndCallCodeModel(endNodeSymbol.Name, EdgeMode);
    }

    public override CallCodeModel VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
        return new TaskCallCodeModel(taskNodeSymbol.Name,
                                     EdgeMode,
                                     ParameterCodeModel.TaskResult(taskNodeSymbol.Declaration),
                                     taskNodeSymbol.Declaration?.CodeNotImplemented ?? false);
    }

    public override CallCodeModel VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
        return new GuiCallCodeModel(dialogNodeSymbol.Name, EdgeMode, ContinuationCall);
    }

    public override CallCodeModel VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
        return new GuiCallCodeModel(viewNodeSymbol.Name, EdgeMode, ContinuationCall);
    }

}