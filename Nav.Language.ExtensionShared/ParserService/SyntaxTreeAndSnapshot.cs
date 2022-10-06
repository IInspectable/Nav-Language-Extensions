#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension; 

sealed class SyntaxTreeAndSnapshot : AndSnapshot {

    internal SyntaxTreeAndSnapshot([NotNull] SyntaxTree syntaxTree, ITextSnapshot snapshot) : base(snapshot) {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

}