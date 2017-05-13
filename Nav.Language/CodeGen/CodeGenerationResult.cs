#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerationResult {

        public CodeGenerationResult(ITaskDefinitionSymbol taskDefinition, string iBeginWfsInterfaceCode, string iWfsInterfaceCode, string wfsBaseCode, string wfsOneShotCode) {
            TaskDefinition         = taskDefinition         ?? throw new ArgumentNullException(nameof(taskDefinition));
            IBeginWfsInterfaceCode = iBeginWfsInterfaceCode ?? throw new ArgumentNullException(nameof(iBeginWfsInterfaceCode));
            IWfsInterfaceCode      = iWfsInterfaceCode      ?? throw new ArgumentNullException(nameof(iWfsInterfaceCode));
            WfsBaseCode            = wfsBaseCode            ?? throw new ArgumentNullException(nameof(wfsBaseCode));
            WfsOneShotCode         = wfsOneShotCode         ?? throw new ArgumentNullException(nameof(wfsOneShotCode));
        }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public string IBeginWfsInterfaceCode { get; }
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public string IWfsInterfaceCode { get; }
        [NotNull]
        public string WfsBaseCode { get; }
        [NotNull]
        public string WfsOneShotCode { get; }        
    }
}