#region Using Directives

using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {
    public class NavCodeGenerator: Task, IGeneratorLogger {

        public bool Force { get; set; }

        public ITaskItem[] Files { get; set; }

        public override bool Execute() {

            if (Files == null) {
                return true;
            }

            var options  = new GenerationOptions(force: Force);
            var pipeline = new NavCodeGeneratorPipeline(options, logger: this);
            var files    = Files.Select(FileSpec.FromTaskItem);

            return pipeline.Run(files);            
        }

        void IGeneratorLogger.LogError(string message) {
            Log.LogError(message);
        }

        void IGeneratorLogger.LogError(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            Log.LogError(
                subcategory    : null, 
                errorCode      : diag.Descriptor.Id, 
                helpKeyword    : null, 
                file           : GetFile(diag, fileSpec),
                lineNumber     : diag.Location.StartLine + 1, 
                columnNumber   : diag.Location.StartCharacter + 1, 
                endLineNumber  : 0, // Ist das von Interesse?
                endColumnNumber: 0, // Ist das von Interesse?
                message        : diag.Message);
        }
        
        void IGeneratorLogger.LogWarning(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            Log.LogWarning(
                subcategory    : null,
                warningCode    : diag.Descriptor.Id,
                helpKeyword    : null,
                file           : GetFile(diag, fileSpec),
                lineNumber     : diag.Location.StartLine + 1,
                columnNumber   : diag.Location.StartCharacter + 1,
                endLineNumber  : 0, // Ist das von Interesse?
                endColumnNumber: 0, // Ist das von Interesse?
                message        : diag.Message);
        }

        static string GetFile(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            if (diag?.Location.FilePath?.ToLower() == fileSpec?.FilePath.ToLower()) {
                return fileSpec?.Identity ?? diag?.Location.FilePath;
            }
            return diag?.Location.FilePath;
        }
    }
}