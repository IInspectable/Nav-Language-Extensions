﻿using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class MemberDeclarationSyntax : SyntaxNode {
        internal MemberDeclarationSyntax(TextExtent extent) : base(extent) {
        }
    }
}