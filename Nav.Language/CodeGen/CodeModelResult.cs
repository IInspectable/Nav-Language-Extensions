#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen;

sealed class CodeModelResult {

    public CodeModelResult(
        ITaskDefinitionSymbol taskDefinition,
        IBeginWfsCodeModel beginWfsCodeModel,
        IWfsCodeModel iwfsCodeModel,
        WfsBaseCodeModel wfsBaseCodeModel,
        WfsCodeModel wfsCodeModel,
        [CanBeNull] IEnumerable<TOCodeModel> toCodeModels) {

        TaskDefinition     = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
        IBeginWfsCodeModel = beginWfsCodeModel;
        IWfsCodeModel      = iwfsCodeModel;
        WfsBaseCodeModel   = wfsBaseCodeModel;
        WfsCodeModel       = wfsCodeModel;
        TOCodeModels       = (toCodeModels ?? Enumerable.Empty<TOCodeModel>()).ToImmutableList();
    }

    public ITaskDefinitionSymbol TaskDefinition { get; }

    [CanBeNull]
    public IBeginWfsCodeModel IBeginWfsCodeModel { get; }

    [CanBeNull]
    public IWfsCodeModel IWfsCodeModel { get; }

    [CanBeNull]
    public WfsBaseCodeModel WfsBaseCodeModel { get; }

    [CanBeNull]
    public WfsCodeModel WfsCodeModel { get; }

    public ImmutableList<TOCodeModel> TOCodeModels { get; }

}