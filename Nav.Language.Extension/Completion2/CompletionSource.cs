﻿#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Pharmatechnik.Nav.Utilities.IO;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Text;

using TextExtent = Pharmatechnik.Nav.Language.Text.TextExtent;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    static class TextSnaphotLineExtensions {

        public static SnapshotPoint GetStartOfIdentifier(this ITextSnapshotLine line, SnapshotPoint start) {
            while (start > line.Start && SyntaxFacts.IsIdentifierCharacter((start - 1).GetChar())) {
                start -= 1;
            }

            return start;
        }

        public static SnapshotPoint? GetPreviousNonWhitespace(this ITextSnapshotLine line, SnapshotPoint start) {

            if (start == line.Start) {
                return null;
            }

            do {
                start -= 1;
            } while (start > line.Start && char.IsWhiteSpace(start.GetChar()));

            return start;
        }

        public static SnapshotSpan? GetSpanOfPreviousIdentifier(this ITextSnapshotLine line, SnapshotPoint start) {

            var wordEnd = line.GetPreviousNonWhitespace(start);
            if (wordEnd == null) {
                return null;
            }

            var wordStart = line.GetStartOfIdentifier(wordEnd.Value);

            return new SnapshotSpan(wordStart, wordEnd.Value + 1);
        }

    }

    class CompletionSource: SemanticModelServiceDependent, ICompletionSource {

        private readonly NavFileCompletionCache _navFileCompletionCache;

        bool _disposed;

        public CompletionSource(ITextBuffer buffer, NavFileCompletionCache navFileCompletionCache): base(buffer) {
            _navFileCompletionCache = navFileCompletionCache;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_disposed) {
                throw new ObjectDisposedException(nameof(CompletionSource));
            }

            var generationUnitAndSnapshot = SemanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;
            var snapshot                  = generationUnitAndSnapshot.Snapshot;

            var snapshotPoint = session.GetTriggerPoint(snapshot);
            if (snapshotPoint == null) {
                return;
            }

            var triggerPoint = (SnapshotPoint) snapshotPoint;
            var triggerToken = codeGenerationUnit.Syntax.FindToken(triggerPoint);

            bool isInComment = triggerToken.Type == SyntaxTokenType.SingleLineComment ||
                               triggerToken.Type == SyntaxTokenType.MultiLineComment;

            // Keine Autocompletion in Kommentaren!
            if (isInComment) {
                return;
            }

            var line         = triggerPoint.GetContainingLine();
            var linePosition = triggerPoint - line.Start;
            var lineText     = line.GetText();
            var start        = line.GetStartOfIdentifier(triggerPoint);

            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();
            var previousWordSpan           = line.GetSpanOfPreviousIdentifier(start);

            var  prevousIdentfier = previousWordSpan?.GetText() ?? "";
            bool showEdgeModes    = previousNonWhitespace == '-';

            if (showEdgeModes) {
                start -= 1;
            }

            var    applicableToSpan = new SnapshotSpan(start, triggerPoint);
            var    applicableTo     = snapshot.CreateTrackingSpan(applicableToSpan, SpanTrackingMode.EdgeInclusive);
            string moniker          = null;
            var    completions      = new List<Completion4>();

            // Es gibt derzeit eigentlich nur die taskrefs wo innerhalb von "" etwas vorgeschlagen werden kann
            if (lineText.IsInQuotation(linePosition)) {

                // TODO Pürüfen, ob links von uns ein taskref steht...
                var quotExtent = lineText.QuotatedExtent(linePosition);

                applicableToSpan = new SnapshotSpan(
                    line.Start + quotExtent.Start,
                    triggerPoint);

                applicableTo = snapshot.CreateTrackingSpan(applicableToSpan, SpanTrackingMode.EdgeInclusive);

                var fi = codeGenerationUnit.Syntax.SyntaxTree.SourceText.FileInfo;
                if (fi?.DirectoryName != null) {
                    var files = Directory.EnumerateFiles(path: fi.DirectoryName,
                                                         searchPattern: $"*{NavLanguageContentDefinitions.FileExtension}",
                                                         searchOption: SearchOption.TopDirectoryOnly);
                    foreach (var file in files) {
                        completions.Add(CreateFileNameCompletion(fi.Directory, new FileInfo(file)));
                    }

                    foreach (var file in _navFileCompletionCache.GetNavFiles()) {
                        completions.Add(CreateFileNameCompletion(fi.Directory, file));
                    }
                }
                

                if (completions.Any()) {
                    moniker = "file";
                    CreateCompletionSet(moniker, completionSets, completions, applicableTo);

                }

                // Wenn in Quotation, dann sind wir fertig...
                return;

            }

            // Edge Modes
            if (showEdgeModes) {

                completions.Add(CreateKeywordCompletion(SyntaxFacts.GoToEdgeKeyword));
                completions.Add(CreateKeywordCompletion(SyntaxFacts.ModalEdgeKeyword));

                moniker = "keyword";

                CreateCompletionSet(moniker, completionSets, completions, applicableTo);

                return;
            }

            // Code Keyword
            if (previousNonWhitespace.ToString() == SyntaxFacts.OpenBracket) {

                foreach (var keyword in SyntaxFacts.CodeKeywords) {
                    completions.Add(CreateKeywordCompletion(keyword));
                }

                moniker = "keyword";
                CreateCompletionSet(moniker, completionSets, completions, applicableTo);

                return;
            }

            if (prevousIdentfier == SyntaxFacts.TaskKeyword) {
                var taskDecls = codeGenerationUnit.TaskDeclarations;
                foreach (var decl in taskDecls) {

                    completions.Add(CreateSymbolCompletion(decl, "decl"));

                    moniker = "keyword";
                }

                if (completions.Any()) {
                    CreateCompletionSet(moniker, completionSets, completions, applicableTo);
                    return;
                }

            }

            var extent = TextExtent.FromBounds(triggerPoint, triggerPoint);

            var taskDefinition = codeGenerationUnit.TaskDefinitions
                                                   .FirstOrDefault(td => td.Syntax.Extent.IntersectsWith(extent));

            if (taskDefinition == null) {
                taskDefinition = codeGenerationUnit.TaskDefinitions
                                                   .LastOrDefault(td => extent.Start > td.Syntax.Start);
            }

            if (taskDefinition != null) {

                // Exit Connection Points
                if (previousNonWhitespace.ToString() == SyntaxFacts.Colon) {

                    var exitNodeEnd   = start - 1;
                    var exitNodeStart = exitNodeEnd;

                    var nodeSpan = line.GetSpanOfPreviousIdentifier(exitNodeStart);
                    var nodeName = nodeSpan?.GetText();

                    if (!String.IsNullOrEmpty(nodeName)) {

                        var exitNodeCandidate = taskDefinition.TryFindNode(nodeName) as ITaskNodeSymbol;

                        if (exitNodeCandidate?.Declaration != null) {
                            // Erst die noch nicht verbundenen...
                            foreach (var cp in exitNodeCandidate.GetUnconnectedExits()) {

                                completions.Add(CreateSymbolCompletion(cp, cp.Name));

                                moniker = "exitNode";
                            }

                            // Dann die bereits verbundenen
                            foreach (var cp in exitNodeCandidate.GetConnectedExits()) {

                                completions.Add(CreateSymbolCompletion(cp, cp.Name));

                                moniker = "exitNode";
                            }
                        }

                        if (completions.Any()) {
                            CreateCompletionSet(moniker, completionSets, completions, applicableTo);
                            return;
                        }
                    }
                }

                // Erst alle Knoten ohne Referenzen...
                foreach (var node in taskDefinition.NodeDeclarations
                                                   .Where(n => n.References.Count == 0)
                                                   .OrderBy(n => n.Name)) {
                    var description = node.Syntax.ToString();

                    completions.Add(CreateSymbolCompletion(node, description));

                    moniker = "keyword";
                }

                // ...dann alle übrigen
                foreach (var node in taskDefinition.NodeDeclarations
                                                   .Where(n => n.References.Count != 0)
                                                   .OrderBy(n => n.Name)) {

                    var description = node.Syntax.ToString();

                    completions.Add(CreateSymbolCompletion(node, description));

                    moniker = "keyword";
                }

            }

            if (!completions.Any()) {
                moniker = "keyword";
            }

            // Keywords
            foreach (var keyword in SyntaxFacts.NavKeywords.OrderBy(n => n)) {

                completions.Add(CreateKeywordCompletion(keyword));
            }

            CreateCompletionSet(moniker, completionSets, completions, applicableTo);

        }

        public override void Dispose() {
            base.Dispose();
            _disposed = true;
        }

        private static void CreateCompletionSet(string moniker, IList<CompletionSet> completionSets, List<Completion4> list, ITrackingSpan applicableTo) {
            if (list.Any()) {
                if (moniker == "keyword") {
                    // IntellisenseFilter[] filters = new[] {
                    //     new IntellisenseFilter(KnownMonikers.Property,   "Standard rules (Alt + S)",      "s", "Standard"),
                    //     new IntellisenseFilter(KnownMonikers.CSFileNode, "C# analysis rules (Alt + C)",   "c", "CSharp"),
                    //     new IntellisenseFilter(KnownMonikers.DotNET,     ".NET analysis rules (Alt + D)", "d", "DotNe"),
                    // };

                    completionSets.Add(new FilteredCompletionSet(moniker, applicableTo, list, Enumerable.Empty<Completion4>(), null));
                } else {
                    completionSets.Add(new FilteredCompletionSet(moniker, applicableTo, list, Enumerable.Empty<Completion4>(), null));
                }
            }
        }

        Completion4 CreateSymbolCompletion(ISymbol symbol, string description) {

            var imageMoniker = ImageMonikers.FromSymbol(symbol);

            var completion = new Completion4(displayText: symbol.Name,
                                             insertionText: symbol.Name,
                                             description: description,
                                             iconMoniker: imageMoniker,
                                             iconAutomationText: "Symbol",
                                             attributeIcons: null,
                                             suffix: null);

            completion.Properties.AddProperty(CompletionElementProvider.SymbolPropertyName, symbol);

            return completion;
        }

        Completion4 CreateFileNameCompletion(DirectoryInfo directory, FileInfo file) {

            var directoryName = directory.FullName + Path.DirectorySeparatorChar;
            var fullPath      = file.FullName;
            var relativePath  = PathHelper.GetRelativePath(directoryName, file.FullName);
            var displayPath   = CompactPath(relativePath, 50);

            var completion = new Completion4(displayText: displayPath,
                                             insertionText: relativePath,
                                             description: fullPath,
                                             iconMoniker: ImageMonikers.Include,
                                             iconAutomationText: null,
                                             attributeIcons: null);

            return completion;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        static string CompactPath(string longPathName, int wantedLength) {
            // NOTE: You need to create the builder with the required capacity before calling function.
            // See http://msdn.microsoft.com/en-us/library/aa446536.aspx
            StringBuilder sb = new StringBuilder(wantedLength + 1);
            PathCompactPathEx(sb, longPathName, wantedLength + 1, 0);
            return sb.ToString();
        }

        Completion4 CreateKeywordCompletion(string keyword) {

            var completion = new Completion4(displayText: keyword,
                                             insertionText: keyword,
                                             description: $"{keyword} Keyword",
                                             iconMoniker: KnownMonikers.IntellisenseKeyword,
                                             iconAutomationText: null,
                                             attributeIcons: null);

            completion.Properties.AddProperty(CompletionElementProvider.KeywordPropertyName, keyword);

            return completion;
        }

        //static class NavIntellisenseFilter {

        //    public static IntellisenseFilter Keyword => new IntellisenseFilter(KnownMonikers.IntellisenseKeyword, "Keyword (Alt + K)", "k", automationText: "Keyword");
        //    public static IntellisenseFilter Nodes   => new IntellisenseFilter(ImageMonikers.TaskNode,            "Tasks (Alt + T)",   "k", automationText: "Symbol");

        //}

    }

}