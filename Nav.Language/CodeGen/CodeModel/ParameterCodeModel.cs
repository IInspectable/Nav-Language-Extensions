#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class ParameterCodeModel : CodeModel {
        
        public ParameterCodeModel(string parameterType, string parameterName) {
            ParameterType = parameterType ?? String.Empty;
            ParameterName = parameterName ?? String.Empty;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
    }
}
