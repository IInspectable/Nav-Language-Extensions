#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class CodeModelResult {

        public CodeModelResult(ITaskDefinitionSymbol taskDefinition, PathProvider pathProvider, IBeginWfsCodeModel beginWfsCodeModel, IWfsCodeModel wfsCodeModel, WfsBaseCodeModel wfsBaseCodeModel) {
            TaskDefinition     = taskDefinition    ?? throw new ArgumentNullException(nameof(taskDefinition));
            PathProvider       = pathProvider      ?? throw new ArgumentNullException(nameof(pathProvider));
            IBeginWfsCodeModel = beginWfsCodeModel ?? throw new ArgumentNullException(nameof(beginWfsCodeModel));
            IWfsCodeModel      = wfsCodeModel      ?? throw new ArgumentNullException(nameof(wfsCodeModel));
            WfsBaseCodeModel   = wfsBaseCodeModel  ?? throw new ArgumentNullException(nameof(wfsCodeModel));
        }

        // ReSharper disable InconsistentNaming
        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        public PathProvider PathProvider { get; }
        [NotNull]
        public IBeginWfsCodeModel IBeginWfsCodeModel { get; }
        [NotNull]
        public IWfsCodeModel IWfsCodeModel { get; }
        [NotNull]
        public WfsBaseCodeModel WfsBaseCodeModel { get; }
        // ReSharper restore InconsistentNaming
    }
}