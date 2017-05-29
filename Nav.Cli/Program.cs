using Pharmatechnik.Nav.Language.Analyzer;
using Pharmatechnik.Nav.Language.Generator;

namespace Pharmatechnik.Nav.Language {
    
    static class Program  {

        static int Main(string[] args) {

            // TODO Response Files
            var cl = CommandLine.Parse(args);
            
            if (cl == null) {
                return -1;
            }
            
            if (cl.Analyze) {
                var p = new SyntaxAnalyzerProgram();
                return p.Run(cl);
            } else {
                var p = new NavCodeGenerator();
                return p.Run(cl);
            }                      
        }
    }
}