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
            PathProvider pathProvider, 
            CodeGenerationSpec iBeginWfsCode, 
            CodeGenerationSpec iWfsCode, 
            CodeGenerationSpec wfsBaseCode, 
            CodeGenerationSpec wfsCode,
            [CanBeNull] IEnumerable<CodeGenerationSpec> toCodeSpecs) {

            TaskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            PathProvider   = pathProvider   ?? throw new ArgumentNullException(nameof(pathProvider));
            IBeginWfsCode  = iBeginWfsCode  ?? throw new ArgumentNullException(nameof(iBeginWfsCode));
            IWfsCode       = iWfsCode       ?? throw new ArgumentNullException(nameof(iWfsCode));
            WfsBaseCode    = wfsBaseCode    ?? throw new ArgumentNullException(nameof(wfsBaseCode));
            WfsCode        = wfsCode        ?? throw new ArgumentNullException(nameof(wfsCode));
            ToCodeSpecs    = (toCodeSpecs ?? Enumerable.Empty<CodeGenerationSpec>()).ToImmutableList();
        }

        // ReSharper disable InconsistentNaming
        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        public PathProvider PathProvider { get; }
        [NotNull]
        public CodeGenerationSpec IBeginWfsCode { get; }
        [NotNull]        
        public CodeGenerationSpec IWfsCode { get; }
        [NotNull]
        public CodeGenerationSpec WfsBaseCode { get; }
        [NotNull]
        public CodeGenerationSpec WfsCode { get; }
        [NotNull]
        public ImmutableList<CodeGenerationSpec> ToCodeSpecs { get; }
        // ReSharper restore InconsistentNaming
    }
}