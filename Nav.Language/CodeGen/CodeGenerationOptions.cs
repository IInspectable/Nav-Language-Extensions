namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationOptions {

        public CodeGenerationOptions(bool force) {
            Force = force;
        }

        public bool Force { get; }

        public static CodeGenerationOptions Default => new CodeGenerationOptions(force: false);
    }
}
