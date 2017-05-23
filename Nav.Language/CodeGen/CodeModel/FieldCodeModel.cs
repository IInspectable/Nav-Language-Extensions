namespace Pharmatechnik.Nav.Language.CodeGen {
    public class FieldCodeModel : ParameterCodeModel {

        public FieldCodeModel(string parameterType, string name): base(parameterType, name) {
        }

        public override string ParameterName => $"{CodeGenFacts.FieldPrefix}{base.ParameterName}";
    }
}