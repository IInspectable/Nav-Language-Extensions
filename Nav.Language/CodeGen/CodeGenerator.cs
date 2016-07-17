#region Using Directives

using System.Collections.Generic;

using Antlr4.StringTemplate;

using Pharmatechnik.Nav.Language.Properties;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerator {

        readonly CodeGenerationOptions _options;

        public CodeGenerator(CodeGenerationOptions options) {
            _options = options;
        }

        // TODO Error / Warning Logging
        public IEnumerable<CodeGenerationResult> Generate(CodeGenerationUnit codeGenerationUnit) {
            // TODO Diagnostic check

            foreach(var taskDefinition in codeGenerationUnit.TaskDefinitions) {
                var result = Generate(taskDefinition);
                yield return result;
            }
        }

        CodeGenerationResult Generate(ITaskDefinitionSymbol taskDefinition) {

            // TODO Diagnostic check

            var result = new CodeGenerationResult(taskDefinition);

            if (_options.GenerateIBeginWfsInterface) {

                var model = new BeginWfsCodeModel(taskDefinition);
                var group = new TemplateGroupString(Resources.IBeginWFS);
                var st    = group.GetInstanceOf("IBeginWFS");

                st.Add("model", model);
                // TODO Add Context?

                result.BeginWfsInterface = st.Render();
            }

            return result;
        }
    }
}