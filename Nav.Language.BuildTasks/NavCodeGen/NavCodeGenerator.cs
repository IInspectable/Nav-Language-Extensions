#region Using Directives

using System.Linq;
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
            var files    = Files.Select(item => item.GetMetadata("FullPath"));

            return pipeline.Run(files);            
        }

        void IGeneratorLogger.LogError(string message) {
            Log.LogError(message);
        }

        void IGeneratorLogger.LogError(Diagnostic diag) {

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
        
        void IGeneratorLogger.LogWarning(Diagnostic diag) {

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