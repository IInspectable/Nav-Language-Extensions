using System.Text;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class GenerationOptions {

        public GenerationOptions(bool force, bool generateToClasses) {
            Force             = force;
            GenerateTOClasses = generateToClasses;
        }

        public bool Force { get; }        
        public bool GenerateTOClasses { get; }
        public Encoding Encoding => Encoding.UTF8; // Ich sehe keinen Grund, ein anderes Encoding als UTF8 zu verwenden.

        public static GenerationOptions Default => new GenerationOptions(force: false, generateToClasses: true);
    }
}
