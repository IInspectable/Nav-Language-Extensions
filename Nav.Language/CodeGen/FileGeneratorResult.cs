#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

public class FileGeneratorResult {

    public FileGeneratorResult(ITaskDefinitionSymbol taskDefinition, FileGeneratorAction action, string fileName) {
        TaskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
        FileName       = fileName       ?? throw new ArgumentNullException(nameof(fileName));
        Action         = action;
    }

    public ITaskDefinitionSymbol TaskDefinition { get; }
    public string                FileName       { get; }
    public FileGeneratorAction   Action         { get; }
}