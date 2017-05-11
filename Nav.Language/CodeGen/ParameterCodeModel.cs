using System;

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class ParameterCodeModel {
        
        public ParameterCodeModel(string parameterType, string parameterName) {
            ParameterType = parameterType ?? String.Empty;
            ParameterName = parameterName ?? String.Empty;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
    }
}
