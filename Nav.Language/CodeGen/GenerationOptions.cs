using System.Text;

namespace Pharmatechnik.Nav.Language.CodeGen; 

public record GenerationOptions {

    public static GenerationOptions Default => new() {
        Force             = false,
        GenerateTOClasses = true
    };

    public bool   Force                { get; init; }
    public bool   GenerateTOClasses    { get; init; }
    public string ProjectRootDirectory { get; init; }
    public string IwflRootDirectory    { get; init; }

    public Encoding Encoding => Encoding.UTF8; // Ich sehe keinen Grund, ein anderes Encoding als UTF8 zu verwenden.

}