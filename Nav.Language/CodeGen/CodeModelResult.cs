#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class CodeModelResult {

        public CodeModelResult(
            ITaskDefinitionSymbol taskDefinition, 
            IPathProvider pathProvider, 
            IBeginWfsCodeModel beginWfsCodeModel,
            IWfsCodeModel iwfsCodeModel, 
            WfsBaseCodeModel wfsBaseCodeModel,
            WfsCodeModel wfsCodeModel,
            [CanBeNull]IEnumerable<TOCodeModel> toCodeModels) {

            TaskDefinition     = taskDefinition    ?? throw new ArgumentNullException(nameof(taskDefinition));
            PathProvider       = pathProvider      ?? throw new ArgumentNullException(nameof(pathProvider));
            IBeginWfsCodeModel = beginWfsCodeModel ?? throw new ArgumentNullException(nameof(beginWfsCodeModel));
            IWfsCodeModel      = iwfsCodeModel     ?? throw new ArgumentNullException(nameof(iwfsCodeModel));
            WfsBaseCodeModel   = wfsBaseCodeModel  ?? throw new ArgumentNullException(nameof(wfsBaseCodeModel));
            WfsCodeModel       = wfsCodeModel      ?? throw new ArgumentNullException(nameof(wfsCodeModel));
            TOCodeModels       = (toCodeModels     ?? Enumerable.Empty<TOCodeModel>()).ToImmutableList();
        }

        // ReSharper disable InconsistentNaming
        public ITaskDefinitionSymbol TaskDefinition { get; }
        public IPathProvider PathProvider { get; }
        public IBeginWfsCodeModel IBeginWfsCodeModel { get; }
        public IWfsCodeModel IWfsCodeModel { get; }
        public WfsBaseCodeModel WfsBaseCodeModel { get; }
        public WfsCodeModel WfsCodeModel { get; }
        public ImmutableList<TOCodeModel> TOCodeModels { get; }
        // ReSharper restore InconsistentNaming
    }
}