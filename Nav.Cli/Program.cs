#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Analyzer;
using Pharmatechnik.Nav.Language.Generator;

#endregion

namespace Pharmatechnik.Nav.Language {
    
    static class Program  {

        static int Main(string[] args) {


            var cmdArgs = new List<string>(args);

            var rspArg = cmdArgs.FirstOrDefault(s => s.StartsWith("@"));
            if (rspArg != null) {

                cmdArgs.Remove(rspArg);

                var fileName = rspArg.Substring(1);

                var rsp = File.ReadAllText(fileName);
                
                cmdArgs.Add($"-s");

                // BUG: Funktiuoniert nicht mit "quoted strings"
                foreach (var v in rsp.Split(' ')) {
                    cmdArgs.Add(v);
                }
            }
            
            var cl = CommandLine.Parse(cmdArgs.ToArray());
            
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