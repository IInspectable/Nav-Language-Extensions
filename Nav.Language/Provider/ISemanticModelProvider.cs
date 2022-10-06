#region Using Directives

using System;
using System.Threading;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language; 

public interface ISemanticModelProvider: IDisposable {

    [CanBeNull]
    CodeGenerationUnit GetSemanticModel(string filePath, CancellationToken cancellationToken = default);
    [NotNull]
    CodeGenerationUnit GetSemanticModel(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken = default);

}