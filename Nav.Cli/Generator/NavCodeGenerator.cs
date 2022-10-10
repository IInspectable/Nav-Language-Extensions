#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Utilities.IO;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Logging;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Generator;

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
            Strict               = cl.Strict,
            GenerateToClasses    = (cl.GenerationOptions & CodeGenerationOptions.ToClasses)   != 0,
            GenerateWflClasses   = (cl.GenerationOptions & CodeGenerationOptions.WflClasses)  != 0,
            GenerateIwflClasses  = (cl.GenerationOptions & CodeGenerationOptions.IwflClasses) != 0,
            ProjectRootDirectory = cl.ProjectRootDirectory,
            IwflRootDirectory    = cl.IwflRootDirectory,
            WflRootDirectory     = cl.WflRootDirectory,

        };

        ValidateOptions();

        var pipeline = NavCodeGeneratorPipeline.Create(options: options, logger: logger, syntaxProviderFactory: syntaxProviderFactory);

        return pipeline;

        void ValidateOptions() {

            if (!options.ProjectRootDirectory.IsNullOrEmpty() &&
                !Directory.Exists(options.ProjectRootDirectory)) {
                throw new ArgumentException($"Das Project Wurzelverzeichnis '{options.ProjectRootDirectory}' exisitiert nicht.");
            }

            if (!options.WflRootDirectory.IsNullOrEmpty() &&
                options.ProjectRootDirectory.IsNullOrEmpty()
               ) {
                throw new ArgumentException($"es wurde ein alternatives WFL Wurzelverzeichnis '{options.IwflRootDirectory}' angegeben, aber kein Project Wurzelverzeichnis.");
            }

            if (!options.IwflRootDirectory.IsNullOrEmpty() &&
                options.ProjectRootDirectory.IsNullOrEmpty()
               ) {
                throw new ArgumentException($"es wurde ein alternatives IWFL Wurzelverzeichnis '{options.IwflRootDirectory}' angegeben, aber kein Project Wurzelverzeichnis.");
            }
        }
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