using System.Text;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class GenerationOptions {

        GenerationOptions(bool force, bool generateToClasses) {
            Force             = force;
            GenerateTOClasses = generateToClasses;
        }

        public static GenerationOptions Default => Create();

        public static GenerationOptions Create(bool force = false, bool generateToClasses = true) {
            return new GenerationOptions(force: force, generateToClasses: generateToClasses);
        }

        public bool     Force             { get; }
        public bool     GenerateTOClasses { get; }
        public Encoding Encoding          => Encoding.UTF8; // Ich sehe keinen Grund, ein anderes Encoding als UTF8 zu verwenden.

    }

}