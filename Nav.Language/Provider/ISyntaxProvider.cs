#region Using Directives

using System;
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface ISyntaxProvider : IDisposable {
        [CanBeNull]
        CodeGenerationUnitSyntax GetSyntax(string filePath, CancellationToken cancellationToken = default);
    }
}