#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

class TaskBeginCodeModel : CodeModel {

    public TaskBeginCodeModel(string taskNodeName,
                              ParameterCodeModel taskBeginParameter,
                              ParameterCodeModel taskBeginFieldParameter,
                              ImmutableList<ParameterCodeModel> taskParameter,
                              bool notImplemented = false) {

        TaskNodeName            = taskNodeName            ?? String.Empty;
        TaskBeginParameter      = taskBeginParameter      ?? throw new ArgumentNullException(nameof(taskBeginParameter));
        TaskBeginFieldParameter = taskBeginFieldParameter ?? throw new ArgumentNullException(nameof(taskBeginFieldParameter));
        TaskParameter           = taskParameter           ?? throw new ArgumentNullException(nameof(taskParameter));
        NotImplemented          = notImplemented;
        TaskBeginField          = new FieldCodeModel(taskBeginFieldParameter.ParameterType, taskBeginFieldParameter.ParameterName);
    }

    public string TaskNodeName           { get; }
    public string TaskNodeNamePascalcase => TaskNodeName.ToPascalcase();

    /// <summary>
    /// Parameter, der das IBegin...WFS interface des Tasks darstellt.
    /// </summary>
    public ParameterCodeModel TaskBeginParameter { get; }

    public ParameterCodeModel TaskBeginFieldParameter { get; }
    public FieldCodeModel     TaskBeginField          { get; }

    /// <summary>
    /// Die Parameter, die zum Aufrufen des Tasks nötig sind.
    /// </summary>
    public ImmutableList<ParameterCodeModel> TaskParameter { get; }
    /// <summary>
    /// Gibt an, ob der Task nicht implementiert ist.
    /// </summary>
    public bool NotImplemented { get; }

}