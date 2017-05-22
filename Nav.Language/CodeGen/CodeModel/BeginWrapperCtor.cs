#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class BeginWrapperCtor : CodeModel {
       
        public BeginWrapperCtor(string taskNodeName, ParameterCodeModel taskInitParamter, ImmutableList<ParameterCodeModel> taskParameter) {

            TaskNodeName      = taskNodeName     ?? String.Empty;
            TaskInitParameter = taskInitParamter ?? throw new ArgumentNullException(nameof(taskInitParamter));
            TaskParameter     = taskParameter    ?? throw new ArgumentNullException(nameof(taskParameter));
        }

        public string TaskNodeName { get; }
        public ParameterCodeModel TaskInitParameter { get; }
        public ImmutableList<ParameterCodeModel> TaskParameter { get; }
    }
}