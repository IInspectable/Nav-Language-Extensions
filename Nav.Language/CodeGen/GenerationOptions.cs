namespace Pharmatechnik.Nav.Language.CodeGen {

    public class GenerationOptions {

        public GenerationOptions(bool force, bool generateToClasses) {
            Force             = force;
            GenerateTOClasses = generateToClasses;
        }

        public bool Force { get; }
        // ReSharper disable once InconsistentNaming
        public bool GenerateTOClasses { get; }

        public static GenerationOptions Default => new GenerationOptions(force: false, generateToClasses: true);
    }
}
