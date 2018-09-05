using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Completion2;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    class PathCompletionSource: AsyncCompletionSource {

        private readonly NavFileCompletionCache _navFileCompletionCache;

        public PathCompletionSource(QuickinfoBuilderService quickinfoBuilderService, NavFileCompletionCache navFileCompletionCache)
            : base(quickinfoBuilderService) {
            _navFileCompletionCache = navFileCompletionCache;

        }

        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            applicableToSpan = default;

            bool IsTriggerChar() {

                return char.IsLetter(typedChar)                 ||
                       typedChar == '\0'                        ||
                       typedChar == '"'                         ||
                       typedChar == Path.DirectorySeparatorChar ||
                       typedChar == Path.AltDirectorySeparatorChar
                    ;
            }

            if (!IsTriggerChar()) {
                return false;
            }

            return ShouldProvideCompletions(triggerLocation, out applicableToSpan, out _);

        }

        public override Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            if (!ShouldProvideCompletions(triggerLocation, out _, out var replacementSpan)) {
                return CreateEmptyCompletionContext();
            }

            var semanticModelService      = SemanticModelService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);
            var generationUnitAndSnapshot = semanticModelService.CodeGenerationUnitAndSnapshot;
            if (generationUnitAndSnapshot == null) {
                return CreateEmptyCompletionContext();
            }

            var codeGenerationUnit = generationUnitAndSnapshot.CodeGenerationUnit;

            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();

            var quotedExtent       = lineText.QuotedExtent(linePosition);
            var previousIdentifier = lineText.GetPreviousIdentifier(quotedExtent.Start - 1);

            // Es gibt derzeit eigentlich nur die taskrefs wo innerhalb von "" etwas vorgeschlagen werden kann
            if (previousIdentifier == SyntaxFacts.TaskrefKeyword) {

                var navDirectory = codeGenerationUnit.Syntax.SyntaxTree.SourceText.FileInfo?.Directory;

                if (navDirectory != null) {

                    // typed ist alles links vom Caret bis zum "
                    var typed = lineText.Substring(quotedExtent.Start, length: linePosition - quotedExtent.Start);
                    var parts = SplitPath(typed);

                    var searchDirectory = navDirectory;
                    // Wenn der Benutzer gerade anfängt einen Dateinamen anzugeben, er aber noch keinen Pfad geschrieben hat, dann zeigen wir
                    // ALLE nav-Files, die von der Solution aus zu erreichen sind.
                    if (String.IsNullOrWhiteSpace(parts.DirPart)) {
                        foreach (var file in _navFileCompletionCache.GetNavFiles()) {
                            completionItems.Add(CreateFileInfoCompletion(navDirectory, file, replacementSpan: replacementSpan));
                        }
                    }

                    // Es wurden nicht alle Nav-Files vorgeschlagen:
                    // - entweder der Benutzer hat schon einen Pfad angegeben, z.B. "..\
                    // - oder es gibt keine Solution
                    // - oder es sined keine Nav-File svon der Solution aus erreichbar
                    if (!completionItems.Any()) {

                        // Der Benutzer hat schon angefangen, eine Pfad zu schreiben, als z.B. "..\
                        if (!String.IsNullOrWhiteSpace(parts.DirPart)) {

                            // Wenn der Pfad absolut ist (z.B. "c:\), nehmen wir direkt dieses Verzeichnis als Suchverzeichnis
                            if (SafeIsPathRooted(parts.DirPart)) {
                                searchDirectory = new DirectoryInfo(parts.DirPart);
                                // Andernfalls stellen wir das Verzeichnis des aktuellen Nav-Files voran.
                            } else if (TryCombinePath(navDirectory.FullName, parts.DirPart, out var fullPath)) {
                                searchDirectory = new DirectoryInfo(fullPath);
                            }
                        }

                        // Sofern es das Verzeichnis ein übergeordnetes Verzeichnis hat, '..' als Auswahl anbieten.
                        if (searchDirectory.Parent != null) {
                            completionItems.Add(CreateDirectoryInfoCompletion(navDirectory, searchDirectory.Parent, "..",
                                                                              replacementSpan: replacementSpan));
                        }

                        // jetzt alle Verzeichnisse anzeigen
                        foreach (var dir in searchDirectory.EnumerateDirectories()) {
                            completionItems.Add(CreateDirectoryInfoCompletion(navDirectory, dir, replacementSpan: replacementSpan));
                        }

                        // .. und am Ende die Nav-Files
                        foreach (var file in searchDirectory.EnumerateFiles(searchPattern: $"*{NavLanguageContentDefinitions.FileExtension}",
                                                                            searchOption: SearchOption.TopDirectoryOnly)) {
                            completionItems.Add(CreateFileInfoCompletion(navDirectory, file, replacementSpan: replacementSpan));
                        }
                    }
                }
            }

            return CreateCompletionContext(completionItems);

        }

        static bool TryCombinePath(string path1, string path2, out string combinedPath) {
            combinedPath = default;
            try {
                combinedPath = Path.Combine(path1, path2);
                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        static bool SafeIsPathRooted(string path) {
            try {
                return Path.IsPathRooted(path);
            } catch (ArgumentException) {
                return false;
            }
        }

        (string DirPart, string FilePart) SplitPath(string path) {
            var dirPart  = "";
            var filePath = "";

            if (!string.IsNullOrEmpty(path)) {

                var index = path.Length;
                while (index > 0) {

                    char ch = path[--index];
                    if (ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar) {

                        dirPart = path.Substring(0, length: index + 1);

                        filePath = path.Substring(index + 1, length: path.Length - index - 1);

                        break;
                    }
                }
            }

            return (DirPart: dirPart, FilePart: filePath);
        }

        bool ShouldProvideCompletions(SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, out ITrackingSpan replacementSpan) {

            applicableToSpan = default;
            replacementSpan  = default;

            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();

            if (lineText.IsInQuotation(linePosition)) {

                var quotedExtent       = lineText.QuotedExtent(linePosition);
                var previousIdentifier = lineText.GetPreviousIdentifier(quotedExtent.Start - 1);

                if (previousIdentifier == SyntaxFacts.TaskrefKeyword) {

                    var start = line.GetStartOfIdentifier(triggerLocation);

                    applicableToSpan = new SnapshotSpan(start, triggerLocation);

                    replacementSpan = applicableToSpan.Snapshot.CreateTrackingSpan(
                        new SnapshotSpan(
                            line.Start + quotedExtent.Start,
                            line.Start + quotedExtent.End),
                        SpanTrackingMode.EdgeInclusive);

                    return true;
                }

            }

            return false;

        }

    }

}