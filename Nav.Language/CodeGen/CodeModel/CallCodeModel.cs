#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class CallCodeModel : CodeModel {

        public CallCodeModel(ImmutableList<NodeCodeModel> targetNodes, ParameterCodeModel result) {
            TargetNodes = targetNodes  ?? throw new ArgumentNullException(nameof(targetNodes));
            TaskResult = result ?? throw new ArgumentNullException(nameof(result));
        }

        public ImmutableList<NodeCodeModel> TargetNodes { get; set; }
        public ParameterCodeModel TaskResult { get; }

        public static CallCodeModel FromNode(INodeSymbol node) {

            var nodes = new List<NodeCodeModel>();
            foreach(var edge in node.GetOutgoingEdges()) {
                var nodeCodeModel = NodeCodeModelBuilder.GetNodeCodeModel(edge.Target?.Declaration, edge.EdgeMode);
                nodes.Add(nodeCodeModel);
            }

            // TODO Result
            var taskResult = new ParameterCodeModel("bool", "todo");
            
            return new CallCodeModel(nodes.ToImmutableList(), taskResult);
        }
    }
}