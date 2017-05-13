#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class ParameterCodeModel : CodeModel {
        
        public ParameterCodeModel(string parameterType, string parameterName) {
            ParameterType = parameterType ?? String.Empty;
            ParameterName = parameterName ?? String.Empty;
        }

        [NotNull]
        public string ParameterType { get; }
        [NotNull]
        public string ParameterName { get; }
    }
}
