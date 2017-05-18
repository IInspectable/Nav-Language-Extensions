#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationResult {

        public CodeGenerationResult(ITaskDefinitionSymbol taskDefinition, PathProvider pathProvider, string iBeginWfsCode, string iWfsCode, string wfsBaseCode, string wfsCode) {
            TaskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            PathProvider   = pathProvider   ?? throw new ArgumentNullException(nameof(pathProvider));
            IBeginWfsCode  = iBeginWfsCode  ?? throw new ArgumentNullException(nameof(iBeginWfsCode));
            IWfsCode       = iWfsCode       ?? throw new ArgumentNullException(nameof(iWfsCode));
            WfsBaseCode    = wfsBaseCode    ?? throw new ArgumentNullException(nameof(wfsBaseCode));
            WfsCode        = wfsCode        ?? throw new ArgumentNullException(nameof(wfsCode));
        }

        // ReSharper disable InconsistentNaming
        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        public PathProvider PathProvider { get; }
        [NotNull]
        public string IBeginWfsCode { get; }
        [NotNull]
        
        public string IWfsCode { get; }
        [NotNull]
        public string WfsBaseCode { get; }
        [NotNull]
        public string WfsCode { get; }
        // ReSharper restore InconsistentNaming
    }
}