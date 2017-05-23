#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    abstract class CallCodeModel: CodeModel {
        
        protected CallCodeModel(string name, EdgeMode edgeMode) {
            Name     = name ?? String.Empty;
            EdgeMode = edgeMode;

        }

        public EdgeMode EdgeMode { get; }

        public string Name { get; }
        public string PascalCaseName => Name.ToPascalcase();

        public abstract string TemplateName { get; }

    }

    class ExitCallCodeModel : CallCodeModel {

        public ExitCallCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName => "goToExit";

    }

    class EndCallCodeModel : CallCodeModel {

        public EndCallCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName => "goToEnd";

    }

    class TaskCallCodeModel : CallCodeModel {

        public TaskCallCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName {
            get {
                switch (EdgeMode) {
                    case EdgeMode.Modal:
                        return "startModalTask";
                    case EdgeMode.NonModal:
                        return "startNonModalTask";
                    case EdgeMode.Goto:
                        return "startTask";
                    default:
                        return "";
                }
            }
        }
    }

    class GuiCallCodeModel : CallCodeModel {

        public GuiCallCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName {
            get {
                switch (EdgeMode) {
                    case EdgeMode.Modal:
                        return "openModalGUI";
                    case EdgeMode.NonModal:
                        return "openNonModalGUI"; // TODO 
                    case EdgeMode.Goto:
                        return "openGUI";
                    default:
                        return "";
                }
            }
        }
    }
}