﻿#region Using Directives

using System;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface ISyntaxProvider : IDisposable {

        SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken));
    }
}