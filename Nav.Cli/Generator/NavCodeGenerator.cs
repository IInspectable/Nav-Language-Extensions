#region Using Directives

using System.IO;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Utilities.IO;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Generator {

    class NavCodeGenerator {

        public int Run(CommandLine cl) {
         
            var syntaxProviderFactory = cl.UseSyntaxCache ? SyntaxProviderFactory.Cached : SyntaxProviderFactory.Default;

            var options  = new GenerationOptions(force: cl.Force, generateToClasses: cl.GenerateToClasses);
            var logger   = new ConsoleLogger(cl.Verbose);            
            var pipeline = new NavCodeGeneratorPipeline(options, logger, syntaxProviderFactory);

            var fileSpecs = CollectFiles(cl, logger);

            return pipeline.Run(fileSpecs) ? 0 : 1;
        }

        static IEnumerable<FileSpec> CollectFiles(CommandLine cl, ILogger logger) {

            var dirFileSpecs = Enumerable.Empty<FileSpec>();
            if (cl.Directory != null) {
                var navFiles = Directory.EnumerateFiles(cl.Directory, "*.nav", SearchOption.AllDirectories);
                dirFileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(cl.Directory, file), fileName: file));
            }

            var srcFileSpecs = Enumerable.Empty<FileSpec>();
            if (cl.Sources != null) {
                srcFileSpecs = cl.Sources.Select(FileSpec.FromFile);
            }

            var fileSpecs = Enumerable.Concat(dirFileSpecs, srcFileSpecs);
            return fileSpecs;
        }
    }
}