﻿#region Using Directives

using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation; 

public abstract class NavInvocationAnnotation: NavTaskAnnotation {

    protected NavInvocationAnnotation(NavTaskAnnotation taskAnnotation,
                                      SimpleNameSyntax identifier): base(taskAnnotation) {

        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }

    [NotNull]
    public SimpleNameSyntax Identifier { get; }
}