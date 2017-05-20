#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class CodeGenerationResult {

        public CodeGenerationResult(
            ITaskDefinitionSymbol taskDefinition, 
            IPathProvider pathProvider, 
            CodeGenerationSpec iBeginWfsCodeSpec, 
            CodeGenerationSpec iWfsCodeSpec, 
            CodeGenerationSpec wfsBaseCodeSpec, 
            CodeGenerationSpec wfsCodeSpec,
            [CanBeNull] IEnumerable<CodeGenerationSpec> toCodeSpecs) {

            TaskDefinition    = taskDefinition    ?? throw new ArgumentNullException(nameof(taskDefinition));
            PathProvider      = pathProvider      ?? throw new ArgumentNullException(nameof(pathProvider));
            IBeginWfsCodeSpec = iBeginWfsCodeSpec ?? throw new ArgumentNullException(nameof(iBeginWfsCodeSpec));
            IWfsCodeSpec      = iWfsCodeSpec      ?? throw new ArgumentNullException(nameof(iWfsCodeSpec));
            WfsBaseCodeSpec   = wfsBaseCodeSpec   ?? throw new ArgumentNullException(nameof(wfsBaseCodeSpec));
            WfsCodeSpec       = wfsCodeSpec       ?? throw new ArgumentNullException(nameof(wfsCodeSpec));
            ToCodeSpecs       = (toCodeSpecs      ?? Enumerable.Empty<CodeGenerationSpec>()).ToImmutableList();
        }

        // ReSharper disable InconsistentNaming
        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        public IPathProvider PathProvider { get; }
        [NotNull]
        public CodeGenerationSpec IBeginWfsCodeSpec { get; }
        [NotNull]        
        public CodeGenerationSpec IWfsCodeSpec { get; }
        [NotNull]
        public CodeGenerationSpec WfsBaseCodeSpec { get; }
        [NotNull]
        public CodeGenerationSpec WfsCodeSpec { get; }
        [NotNull]
        public ImmutableList<CodeGenerationSpec> ToCodeSpecs { get; }       
        // ReSharper restore InconsistentNaming
    }
}