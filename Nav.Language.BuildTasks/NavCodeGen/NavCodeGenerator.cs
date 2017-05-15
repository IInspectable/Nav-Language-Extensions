#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class NavCodeGenerator: Task {

        public bool Force { get; set; }

        public ITaskItem[] Files { get; set; }

        public override bool Execute() {

            if (Files == null) {
                return true;
            }
            
            var options        = new GenerationOptions(force: Force);
            var modelGenerator = new CodeModelGenerator(options);
            var codeGenerator  = new CodeGenerator(options);
            var fileGenerator  = new FileGenerator(options);

            foreach(var file in Files.Select(item => item.GetMetadata("FullPath"))) {

                var syntax = SyntaxTree.FromFile(file);
                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax((CodeGenerationUnitSyntax)syntax.GetRoot());

                LogWarnings(syntax.Diagnostics);
                LogWarnings(codeGenerationUnit.Diagnostics);

                if (LogErrors(syntax.Diagnostics) || LogErrors(codeGenerationUnit.Diagnostics)) {
                    continue;
                }

                var codeModelResults=modelGenerator.Generate(codeGenerationUnit);
                foreach (var codeModelResult in codeModelResults) {
                    var codeGenerationResult = codeGenerator.Generate(codeModelResult);

                    fileGenerator.Generate(codeGenerationResult);
                }
            }
            
            return !Log.HasLoggedErrors;
        }

        bool LogErrors(IEnumerable<Diagnostic> diagnostics) {
            bool errorsLogged = false;
            foreach (var error in diagnostics.Errors()) {
                errorsLogged = true;
                LogError(error);
            }
            return errorsLogged;
        }

        void LogError(Diagnostic diag) {

            var location = diag.Location;
            Log.LogError(
                subcategory    : null, 
                errorCode      : diag.Descriptor.Id, 
                helpKeyword    : null, 
                file           : location.FilePath, 
                lineNumber     : location.StartLine+1, 
                columnNumber   : location.StartCharacter+1, 
                endLineNumber  : location.EndLine + 1, 
                endColumnNumber: location.EndCharacter + 1, 
                message        : diag.Message);
        }

        void LogWarnings(IEnumerable<Diagnostic> diagnostics) {
            foreach (var warning in diagnostics.Warnings()) {
                LogWarning(warning);
            }
        }

        void LogWarning(Diagnostic diag) {

            var location = diag.Location;
            Log.LogWarning(
                subcategory    : null,
                warningCode    : diag.Descriptor.Id,
                helpKeyword    : null,
                file           : location.FilePath,
                lineNumber     : location.StartLine + 1,
                columnNumber   : location.StartCharacter + 1,
                endLineNumber  : location.EndLine + 1,
                endColumnNumber: location.EndCharacter + 1,
                message        : diag.Message);
        }      
    }
}
