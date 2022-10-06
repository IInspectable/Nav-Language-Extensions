#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

public sealed class CodeGenerationSpec {

    public CodeGenerationSpec(string content, string filePath) {
        Content  = content  ?? String.Empty;
        FilePath = filePath ?? String.Empty;
    }

    public string Content  { get; }
    public string FilePath { get; }        
}