#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWrapperCtor : CodeModel {
       
        public BeginWrapperCtor(string taskNodeName, ParameterCodeModel taskBeginParameter, ImmutableList<ParameterCodeModel> taskParameter) {

            TaskNodeName       = taskNodeName      ?? String.Empty;
            TaskBeginParameter = taskBeginParameter ?? throw new ArgumentNullException(nameof(taskBeginParameter));
            TaskParameter      = taskParameter     ?? throw new ArgumentNullException(nameof(taskParameter));
        }

        // TODO BeginMethodName in Model statt TaskNodeName (=> Pascalcse) mit TaskInitCodeModel abgleichen
        // => TaskDeclarationCodeInfo
        public string TaskNodeName { get; }
        public ParameterCodeModel TaskBeginParameter { get; }
        public ImmutableList<ParameterCodeModel> TaskParameter { get; }
    }
}