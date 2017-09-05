#region Using Directives

using System;
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface ISyntaxProvider : IDisposable {
        [CanBeNull]
        SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken));
    }
}