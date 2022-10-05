#region Using Directives

using System;
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

            var logger = new ConsoleLogger(
                fullPaths : cl.FullPaths, 
                noWarnings: cl.NoWarnings,
                verbose   : cl.Verbose);

            try {

                var fileSpecs = CollectFiles(cl);
                var pipeline  = CreatePipeline(cl, logger);

                return pipeline.Run(fileSpecs) ? 0 : 1;

            } catch (Exception ex) {

                logger.LogError(ex.ToString());

                return -1;
            }
        }

        static NavCodeGeneratorPipeline CreatePipeline(CommandLine cl, ConsoleLogger logger) {

            var syntaxProviderFactory = cl.UseSyntaxCache ? SyntaxProviderFactory.Cached : SyntaxProviderFactory.Default;

            var options = new GenerationOptions {
                Force                = cl.Force,
                GenerateTOClasses    = cl.GenerateToClasses,
                ProjectRootDirectory = cl.ProjectRootDirectory,
                IwflRootDirectory    = cl.IwflRootDirectory,

            };
            var pipeline = NavCodeGeneratorPipeline.Create(options: options, logger: logger, syntaxProviderFactory: syntaxProviderFactory);

            return pipeline;
        }

        static IEnumerable<FileSpec> CollectFiles(CommandLine cl) {

            var dirFileSpecs = Enumerable.Empty<FileSpec>();
            if (cl.Directory != null) {
                var navFiles = Directory.EnumerateFiles(cl.Directory, "*.nav", SearchOption.AllDirectories);
                dirFileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(cl.Directory, file), fileName: file));
            }

            var srcFileSpecs = Enumerable.Empty<FileSpec>();
            if (cl.Sources != null) {
                srcFileSpecs = cl.Sources.Select(FileSpec.FromFile);
            }

            return dirFileSpecs.Concat(srcFileSpecs);
        }

    }

}