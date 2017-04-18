#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Pharmatechnik.Nav.Language;

#endregion

namespace TestClient
{
    sealed class Program : IDisposable {
        readonly StreamWriter _logger;

        Program() {
            _logger = new StreamWriter("NavSyntaxErrors.txt");
        }

        static void Main(string[] args) {

            if (args.Length == 0) {
                WriteConsole(ConsoleColor.DarkRed,  "no directory specified");
                PressAnyKeyToContinue();
                return;
            }

            string directory = args[0];

            var sw = Stopwatch.StartNew();
            using (var p = new Program()) {

                p.Run(directory);
            }

            Console.WriteLine(sw.Elapsed);
            PressAnyKeyToContinue();
        }

        static void PressAnyKeyToContinue() {
            Console.WriteLine("Press any key to continue");
            //Console.ReadKey();
        }

        public void Dispose() {
            _logger.Dispose();
        }
        
        void Run(string directory) {

            int maxTokens = 0;
            int sumTokens = 0;
            int files     = 0;

            var navFiles = Directory.EnumerateFiles(directory, "*.nav", SearchOption.AllDirectories).SelectMany(f=> Enumerable.Repeat(f, 1000));
            var codeGenerationUnits= navFiles
                                          //   .AsParallel().WithMergeOptions(ParallelMergeOptions.NotBuffered) // Sofortige Ausgabe der Ergebnisse
                                          .Select(BuildCodeGenerationUnit);

            foreach(var codeGenerationUnit in codeGenerationUnits.Where(cu => cu!=null)) {

                files++;

                var syntaxTree = codeGenerationUnit.Syntax.SyntaxTree;
                var file       = syntaxTree.FileInfo;

                bool writeEndSeparator = false;

                if(syntaxTree.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error) || 
                   codeGenerationUnit.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error)) {
                    WriteInfo("");
                    WriteInfo("File:");
                    WriteInfo("=====");
                    WriteInfo("{0}", file); 
                    writeEndSeparator = true;
                }
               
                if (syntaxTree.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error)) {
                    WriteInfo("");
                    WriteInfo("Syntaxfehler:");
                    WriteInfo("=============");
                    WriteErrorDiagnostics(syntaxTree.Diagnostics);
                }

                if (codeGenerationUnit.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error)) {
                    WriteInfo("");
                    WriteInfo("Semantikfehler:");
                    WriteInfo("===============");
                    WriteErrorDiagnostics(codeGenerationUnit.Diagnostics);
                }

                if (writeEndSeparator) {
                    WriteInfo("=============================================================");
                }

                maxTokens =  Math.Max(maxTokens, syntaxTree.Tokens.Count);
                sumTokens += syntaxTree.Tokens.Count;
            }

            Console.WriteLine("maxTokens: {0}, sumTokens: {1}, Files {2}", maxTokens, sumTokens, files);
        }
      
        CodeGenerationUnit BuildCodeGenerationUnit(string filename) {
            try {

                var syntaxTree      = SyntaxTree.FromFile(filename);
                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntaxTree.GetRoot() as CodeGenerationUnitSyntax);

                return codeGenerationUnit;
            }
            catch (Exception e) {
                WriteError("Unerwarteter Fehler beim Parsen von '{0}'", filename);
                WriteError(e.ToString());
            }
            return null;
        }

        void WriteErrorDiagnostics(IReadOnlyList<Diagnostic> diagnostics) {
            foreach (var diagnostic in diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)) {
                WriteError("{0}", diagnostic);
            }
        }

        void WriteInfo(string format, params object[] args) {
           // Console.WriteLine(format, args);
            WriteLog(format, args);
        }

        // ReSharper disable once UnusedMember.Local
        void WriteWarning(string format, params object[] args) {
           // WriteConsole(ConsoleColor.Yellow, format, args);
            WriteLog(format, args);
        }

        void WriteError(string format, params object[] args) {
           // WriteConsole(ConsoleColor.DarkRed, format, args);
            WriteLog(format, args);
        }

        void WriteLog(string format, object[] args) {
            //_logger.WriteLine(format, args);
        }
        
        static void WriteConsole(ConsoleColor color, string format, params object[] args) {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
          //  Console.WriteLine(format, args);
            Console.ForegroundColor = oldColor;
        }
    }
}
