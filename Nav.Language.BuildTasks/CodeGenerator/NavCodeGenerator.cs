#region Using Directives

using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class NavCodeGenerator: Task {

        public bool Force { get; set; }
        public bool GenerateToClasses { get; set; }

        public ITaskItem[] Files { get; set; }

        public override bool Execute() {

            if (Files == null) {
                return true;
            }

            var options  = new GenerationOptions(force: Force, generateToClasses: GenerateToClasses);
            var logger   = new TaskLogger(this);
            var pipeline = new NavCodeGeneratorPipeline(options, logger);
            var files    = Files.Select(FileSpec.FromTaskItem);

            return pipeline.Run(files);            
        }        
    }
}