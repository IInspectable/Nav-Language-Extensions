#region Using Directives

using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension; 

sealed class CodeGenerationUnitAndSnapshot: AndSnapshot {

    internal CodeGenerationUnitAndSnapshot([NotNull] CodeGenerationUnit codeGenerationUnit, [NotNull] ITextSnapshot snapshot): base(snapshot) {
        CodeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
    }

    [NotNull]
    public CodeGenerationUnit CodeGenerationUnit { get; }         
}