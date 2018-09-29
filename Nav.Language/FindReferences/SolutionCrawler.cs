#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    // TODO Sollte evtl. in die Solution oder NavWorkspace, weil man dadurch die Provider 
    // von außen steuern könnte
    class SolutionCrawler {

        public static async Task StartAsync(
            NavSolution solution,
            CodeGenerationUnit startingUnit,
            Func<CodeGenerationUnit, Task> asyncAction,
            CancellationToken cancellationToken) {

            // TODO Review and refactoring, Cancellation
            var syntaxProvider        = new CachedSyntaxProvider();
            var semanticModelProvider = new SemanticModelProvider(syntaxProvider);

            // TODO File/Path comparer...
            var seenFiles = new HashSet<string>();

            // Falls uns eine CGU für den Einstieg gegeben wurde, beginne wir die Suche hier,
            // bevor alle anderen CGUs der Solution durchkaufen werden.            
            if (startingUnit != null) {

                var navFile = startingUnit.Syntax.SyntaxTree.SourceText.FileInfo;
                var navDir  = navFile?.Directory;

                // 1. In dem File anfangen, in dem sich auch die Definition befindet, deren Referenzen gesucht werden
                await asyncAction(startingUnit);

                if (navFile != null) {
                    // Wenn das Definitionsfile einen Dateinamen hat, dann zu den bereits gesehenen hinzufügen.
                    seenFiles.Add(navFile.FullName);
                }

                // 2. Wir suchen in dem Verzeichnis, in dem sich auch das Nav File der Definition befindet. Die Wahscheinlichkeit ist recht groß,
                //    dass hier bereits erste Treffer ermittelt werden.
                if (navDir != null) {

                    foreach (var fileName in Directory.EnumerateFiles(navDir.FullName, "*.nav")) {

                        if (cancellationToken.IsCancellationRequested) {
                            break;
                        }

                        await ProcessFile(fileName);
                    }

                }
            }

            // 3. Zu guter letzt durchsuchen wir alle übrigen Files der "Solution", was mittlerweil ~1400 Dateien sind, und
            //    entsprechend lange dauert.
            foreach (var file in solution.SolutionFiles) {

                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                await ProcessFile(file.FullName);
            }

            async Task ProcessFile(string fileName) {

                if (!seenFiles.Add(fileName)) {
                    return;
                }

                var codeGen = semanticModelProvider.GetSemanticModel(fileName, cancellationToken);
                if (codeGen == null) {
                    return;
                }

                await asyncAction(codeGen);
            }
        }

    }

}