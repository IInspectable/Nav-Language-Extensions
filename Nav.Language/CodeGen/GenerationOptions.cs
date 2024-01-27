using System.Text;

namespace Pharmatechnik.Nav.Language.CodeGen;

public record GenerationOptions {

    public static GenerationOptions Default => new() {
        Force               = false,
        GenerateToClasses   = true,
        GenerateWflClasses  = true,
        GenerateIwflClasses = true,
    };

    public bool Force               { get; init; }
    public bool Strict              { get; init; }
    public bool GenerateToClasses   { get; init; }
    public bool GenerateWflClasses  { get; init; }
    public bool GenerateIwflClasses { get; init; }

    public string ProjectRootDirectory { get; init; }
    public string IwflRootDirectory    { get; init; }
    public string WflRootDirectory     { get; init; }

    public Encoding Encoding => new UTF8Encoding(encoderShouldEmitUTF8Identifier: true); // Ich sehe keinen Grund, ein anderes Encoding als UTF8 zu verwenden.

}