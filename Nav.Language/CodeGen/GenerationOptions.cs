namespace Pharmatechnik.Nav.Language.CodeGen {

    public class GenerationOptions {

        public GenerationOptions(bool force) {
            Force = force;
        }

        public bool Force { get; }

        public static GenerationOptions Default => new GenerationOptions(force: false);
    }
}
