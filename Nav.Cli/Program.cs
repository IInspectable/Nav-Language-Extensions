#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pharmatechnik.Nav.Language.BuildTasks;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Generator;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language {
    
    class Program  {

        static void Main(string[] args) {

            //Console.WriteLine($"{Environment.CommandLine}");

            var cl = CommandLine.Parse(args);
            
         //   Console.WriteLine($"Error:{Environment.CommandLine}");
            if (cl == null) {
                PressAnyKeyToContinue();
                return;
            }

            if (cl.Analyze) {
                var p = new SyntaxAnalyzerProgram();
                p.Run(cl);
            } else {
                var p = new GeneratorProgram();
                p.Run(cl);
            }
                      
        }

        protected static void PressAnyKeyToContinue() {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }     
    }

    class GeneratorProgram: Program {

        public void Run(CommandLine cl) {

            

            var syntaxProviderFactory = cl.UseSyntaxCache ? SyntaxProviderFactory.Cached : SyntaxProviderFactory.Default;

            var options  = new GenerationOptions(force: cl.Force, generateToClasses: cl.GenerateToClasses);
            var logger   = new ConsoleLogger(cl.Verbose);


            logger.LogInfo($"{ThisAssembly.ProductName} {ThisAssembly.ProductVersion}");

            var pipeline = new NavCodeGeneratorPipeline(options, logger, syntaxProviderFactory);

            IEnumerable<FileSpec> fileSpecs=null;

            if (cl.Directory != null) {
                var navFiles = Directory.EnumerateFiles(cl.Directory, "*.nav", SearchOption.AllDirectories);
                fileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(cl.Directory, file), fileName: file));
            }
            if (cl.Sources != null) {         
                fileSpecs = cl.Sources.Select(file => FileSpec.FromRelativePath(Environment.CurrentDirectory, file));
            }

            if (fileSpecs == null) {
               

                // TODO Log Not Files Specified?
                return;
            }
            //Console.WriteLine($"Error: pipeline.Run {fileSpecs.First().FilePath} !");
            pipeline.Run(fileSpecs);
        }
    }

    sealed class SyntaxAnalyzerProgram : Program {
        public void Run(CommandLine cl) {

            var syntaxProviderFactory = cl.UseSyntaxCache ? SyntaxProviderFactory.Cached : SyntaxProviderFactory.Default;

            var logger   = new ConsoleLogger(cl.Verbose);
            var pipeline = new SyntaxAnalyzerPipeline(logger, syntaxProviderFactory);

            var navFiles  = Directory.EnumerateFiles(cl.Directory, "*.nav", SearchOption.AllDirectories);
            var fileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(cl.Directory, file), fileName: file));
            var analyzer = new CodeNotImplementedAnalyzer(cl.Pattern);
            pipeline.Run(fileSpecs, analyzer);

            Console.WriteLine($"Number of CodeNotImplemented: {analyzer.Result}");
        }
    }
}