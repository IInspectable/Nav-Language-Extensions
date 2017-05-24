#region Using Directives

using System;
using System.IO;
using System.Linq;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.BuildTasks;

using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Nav.Client {
    
    class Program  {

        static void Main(string[] args) {

            var cl = CommandLine.Parse(args);

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
            var pipeline = new NavCodeGeneratorPipeline(options, logger, syntaxProviderFactory);

            var navFiles  = Directory.EnumerateFiles(cl.Directory, "*.nav", SearchOption.AllDirectories);
            var fileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(cl.Directory, file), fileName: file));

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