﻿#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language; 

public class NavSolution {

    public NavSolution([CanBeNull] DirectoryInfo solutionRoot,
                       ImmutableArray<FileInfo> solutionFiles,
                       ISyntaxProvider syntaxProvider = null,
                       ISemanticModelProvider semanticModelProvider = null) {

        SolutionDirectory = solutionRoot;
        SolutionFiles     = solutionFiles;

        SyntaxProvider        = syntaxProvider        ?? new CachedSyntaxProvider();
        SemanticModelProvider = semanticModelProvider ?? new SemanticModelProvider(SyntaxProvider);
    }

    public ISyntaxProvider        SyntaxProvider        { get; }
    public ISemanticModelProvider SemanticModelProvider { get; }

    [CanBeNull]
    public DirectoryInfo SolutionDirectory { get; }

    public ImmutableArray<FileInfo> SolutionFiles { get; }

    public static NavSolution Empty = new NavSolution(null, ImmutableArray<FileInfo>.Empty);

    public static string SearchFilter => $"*.nav";

    public static Task<NavSolution> FromDirectoryAsync(DirectoryInfo directory, CancellationToken cancellationToken) {

        if (String.IsNullOrEmpty(directory?.FullName)) {
            return Task.FromResult(Empty);
        }

        var itemBuilder = ImmutableArray.CreateBuilder<FileInfo>();

        foreach (var file in Directory.EnumerateFiles(directory.FullName,
                                                      SearchFilter,
                                                      SearchOption.AllDirectories)) {

            if (cancellationToken.IsCancellationRequested) {
                return Task.FromResult(Empty);
            }

            var fileInfo = new FileInfo(file);
            itemBuilder.Add(fileInfo);

        }

        var solution = new NavSolution(directory, itemBuilder.ToImmutableArray());

        return Task.FromResult(solution);
    }

    public async Task ProcessCodeGenerationUnitsAsync([NotNull] Func<CodeGenerationUnit, Task> asyncAction,
                                                      [CanBeNull] CodeGenerationUnit startingUnit,
                                                      CancellationToken cancellationToken) {

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

                    cancellationToken.ThrowIfCancellationRequested();

                    await ProcessFile(fileName);
                }

            }
        }

        // 3. Zu guter Letzt durchsuchen wir alle übrigen Files der "Solution", was mittlerweile ~1400 Dateien sind, und
        //    entsprechend lange dauert.
        foreach (var file in SolutionFiles.AsParallel().WithCancellation(cancellationToken)) {

            cancellationToken.ThrowIfCancellationRequested();

            await ProcessFile(file.FullName);
        }

        async Task ProcessFile(string fileName) {

            if (!seenFiles.Add(fileName)) {
                return;
            }

            var codeGen = SemanticModelProvider.GetSemanticModel(fileName, cancellationToken);
            if (codeGen == null) {
                return;
            }

            await asyncAction(codeGen);
        }
    }

}