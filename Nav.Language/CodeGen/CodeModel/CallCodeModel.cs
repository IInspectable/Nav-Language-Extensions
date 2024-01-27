#nullable enable

#region Using Directives

using System;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen;

abstract class CallCodeModel: CodeModel {

    protected CallCodeModel(string? name, EdgeMode edgeMode, CallCodeModel? continuation = null) {
        Name         = name ?? String.Empty;
        EdgeMode     = edgeMode;
        Continuation = continuation;
    }

    public EdgeMode EdgeMode       { get; }
    public string   Name           { get; }
    public string   PascalcaseName => Name.ToPascalcase();
    public string   CamelcaseName  => Name.ToCamelcase();

    public CallCodeModel? Continuation { get; }

    public abstract string TemplateName { get; }
    public abstract int    SortOrder    { get; }

}

sealed class CancelCallCodeModel: CallCodeModel {

    public CancelCallCodeModel(): base(String.Empty, EdgeMode.Goto) {
    }

    public override string TemplateName => "cancel";
    public override int    SortOrder    => Int32.MaxValue;

}

sealed class TaskCallCodeModel: CallCodeModel {

    public TaskCallCodeModel(string name, EdgeMode edgeMode, ParameterCodeModel taskResult, bool notImplemented): base(name, edgeMode) {
        TaskResult     = taskResult ?? throw new ArgumentNullException(nameof(taskResult));
        NotImplemented = notImplemented;
    }

    public ParameterCodeModel TaskResult     { get; }
    public bool               NotImplemented { get; }

    public override string TemplateName {
        get {
            return EdgeMode switch {
                EdgeMode.Modal    => "openModalTask",
                EdgeMode.NonModal => "startNonModalTask",
                EdgeMode.Goto     => "gotoTask",
                _                 => ""
            };
        }
    }

    public override int SortOrder => 1;

}

sealed class GuiCallCodeModel: CallCodeModel {

    public GuiCallCodeModel(string name, EdgeMode edgeMode, CallCodeModel? continuation): base(name, edgeMode, continuation) {
    }

    public override string TemplateName {
        get {
            if (Continuation != null) {
                return EdgeMode switch {
                    EdgeMode.Modal    => "openModalGUIConcat",
                    EdgeMode.NonModal => "startNonModalGUIConcat",
                    EdgeMode.Goto     => "gotoGUIConcat",
                    _                 => ""
                };
            }

            return EdgeMode switch {
                EdgeMode.Modal    => "openModalGUI",
                EdgeMode.NonModal => "startNonModalGUI",
                EdgeMode.Goto     => "gotoGUI",
                _                 => ""
            };
        }
    }

    public override int SortOrder => 2;

}

sealed class ExitCallCodeModel: CallCodeModel {

    public ExitCallCodeModel(string name, EdgeMode edgeMode): base(name, edgeMode) {
    }

    public override string TemplateName => "goToExit";
    public override int    SortOrder    => 3;

}

sealed class EndCallCodeModel: CallCodeModel {

    public EndCallCodeModel(string name, EdgeMode edgeMode): base(name, edgeMode) {
    }

    public override string TemplateName => "goToEnd";
    public override int    SortOrder    => 4;

}