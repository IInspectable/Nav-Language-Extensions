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

        TaskDefinition     = taskDefinition    ?? throw new ArgumentNullException(nameof(taskDefinition));
        IBeginWfsCodeModel = beginWfsCodeModel ?? throw new ArgumentNullException(nameof(beginWfsCodeModel));
        IWfsCodeModel      = iwfsCodeModel     ?? throw new ArgumentNullException(nameof(iwfsCodeModel));
        WfsBaseCodeModel   = wfsBaseCodeModel  ?? throw new ArgumentNullException(nameof(wfsBaseCodeModel));
        WfsCodeModel       = wfsCodeModel      ?? throw new ArgumentNullException(nameof(wfsCodeModel));
        TOCodeModels       = (toCodeModels ?? Enumerable.Empty<TOCodeModel>()).ToImmutableList();
    }

    public ITaskDefinitionSymbol      TaskDefinition     { get; }
    public IBeginWfsCodeModel         IBeginWfsCodeModel { get; }
    public IWfsCodeModel              IWfsCodeModel      { get; }
    public WfsBaseCodeModel           WfsBaseCodeModel   { get; }
    public WfsCodeModel               WfsCodeModel       { get; }
    public ImmutableList<TOCodeModel> TOCodeModels       { get; }

}