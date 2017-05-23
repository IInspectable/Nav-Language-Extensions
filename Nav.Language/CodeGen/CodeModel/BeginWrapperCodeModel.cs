#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWrapperCodeModel: CodeModel {
        
        public BeginWrapperCodeModel(string taskNodeName, ImmutableList<BeginWrapperCtor> ctors) {
            
            TaskNodeName   = taskNodeName?? String.Empty;
            Ctors          = ctors       ?? throw new ArgumentNullException(nameof(ctors));
        }

        public string TaskNodeName { get; }
        public ImmutableList<BeginWrapperCtor> Ctors { get;}

        public static BeginWrapperCodeModel FromTaskNode(ITaskNodeSymbol taskNode) {

            if (taskNode.Declaration == null) {
                throw new InvalidOperationException();
            }

            var ctors = new List<BeginWrapperCtor>();
            foreach (var initConnectionPoint in taskNode.Declaration.Inits().OfType<IInitConnectionPointSymbol>()) {

                var parameterSyntaxes = GetTaskParameterSyntaxes(initConnectionPoint);
                var taskParameter     = ParameterCodeModel.FromParameterSyntaxes(parameterSyntaxes);
               
                var ctor = new BeginWrapperCtor(
                    taskNodeName      : taskNode.Name.ToPascalcase(), 
                    taskBeginParameter: ParameterCodeModel.GetTaskBeginAsParameter(taskNode.Declaration), 
                    taskParameter     : taskParameter.ToImmutableList());
                ctors.Add(ctor);
            }
           
            return new BeginWrapperCodeModel(taskNode.Name.ToPascalcase(), ctors.ToImmutableList());
        }

        static IEnumerable<ParameterSyntax> GetTaskParameterSyntaxes(IInitConnectionPointSymbol initConnectionPoint) {
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