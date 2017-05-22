#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWrapperCodeModel: CodeModel {
        
        public BeginWrapperCodeModel(string taskNodeName, ImmutableList<BeginWrapperCtor> ctors, CallCodeModel call) {
            
            TaskNodeName = taskNodeName?? String.Empty;
            Ctors        = ctors       ?? throw new ArgumentNullException(nameof(ctors));
            Call        = call ?? throw new ArgumentNullException(nameof(ctors));
        }

        public string TaskNodeName { get; }
        public ImmutableList<BeginWrapperCtor> Ctors { get;}
        public CallCodeModel Call { get; }

        public static BeginWrapperCodeModel FromTaskNode(ITaskNodeSymbol taskNode) {

            if (taskNode.Declaration == null) {
                throw new InvalidOperationException();
            }

            var ctors = new List<BeginWrapperCtor>();
            foreach (var initConnectionPoint in taskNode.Declaration.Inits().OfType<IInitConnectionPointSymbol>()) {

                var taskParameter       = GetTaskParameter(initConnectionPoint);
                var parameterCodeModels = ParameterCodeModel.FromParameterSyntax(taskParameter);
               
                var ctor = new BeginWrapperCtor(
                    taskNodeName    : taskNode.Name.ToPascalcase(), 
                    taskInitParamter: ParameterCodeModel.FromTaskDeclaration(taskNode.Declaration), 
                    taskParameter   : parameterCodeModels.ToImmutableList());
                ctors.Add(ctor);
            }

            var calls = CallCodeModel.FromNode(taskNode);

            return new BeginWrapperCodeModel(taskNode.Name.ToPascalcase(), ctors.ToImmutableList(), calls);
        }

        static IEnumerable<ParameterSyntax> GetTaskParameter(IInitConnectionPointSymbol initConnectionPoint) {
            var paramList = initConnectionPoint.Syntax.CodeParamsDeclaration?.ParameterList;
            if (paramList == null) {
                yield break;
            }

            foreach (var p in paramList) {
                yield return p;
            }
        }
    }
}