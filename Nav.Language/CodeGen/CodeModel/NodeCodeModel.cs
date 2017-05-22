#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    abstract class NodeCodeModel: CodeModel {
        
        protected NodeCodeModel(string name, EdgeMode edgeMode) {
            Name     = name ?? String.Empty;
            EdgeMode = edgeMode;

        }

        public EdgeMode EdgeMode { get; }

        public string Name { get; }
        public string PascalCaseName => Name.ToPascalcase();

        public abstract string TemplateName { get; }

    }

    class ExitNodeCodeModel : NodeCodeModel {

        public ExitNodeCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName => "goToExit";

    }

    class EndNodeCodeModel : NodeCodeModel {

        public EndNodeCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
        }

        public override string TemplateName => "goToEnd";

    }

    class TaskNodeCodeModel : NodeCodeModel {

        public TaskNodeCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
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

    class GuiNodeCodeModel : NodeCodeModel {

        public GuiNodeCodeModel(string name, EdgeMode edgeMode) : base(name, edgeMode) {
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