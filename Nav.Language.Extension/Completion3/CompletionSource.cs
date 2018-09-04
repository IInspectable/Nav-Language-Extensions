#region Using Directives

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

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    class CompletionSource: AsyncCompletionSource {

        public CompletionSource(QuickinfoBuilderService quickinfoBuilderService): base(quickinfoBuilderService) {

        }

        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            char edgeTrigger = '-';

            bool IsTriggerChar() {

                return char.IsLetter(typedChar)             ||
                       typedChar == '\0'                    ||
                       typedChar == edgeTrigger             ||
                       typedChar == SyntaxFacts.OpenBracket ||
                       typedChar == SyntaxFacts.Colon       ||
                       typedChar == '"';
            }

            applicableToSpan = default;

            if (!IsTriggerChar()) {
                return false;
            }

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;

            var triggerToken = codeGenerationUnit.Syntax.FindToken(triggerLocation);

            bool isInComment = triggerToken.Type == SyntaxTokenType.SingleLineComment ||
                               triggerToken.Type == SyntaxTokenType.MultiLineComment;

            // Keine Autocompletion in Kommentaren!
            if (isInComment) {
                return false;
            }

            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();
            var start        = line.GetStartOfIdentifier(triggerLocation);

            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();

            bool showEdgeModes = previousNonWhitespace == edgeTrigger;

            if (showEdgeModes) {
                start -= 1;
            }

            applicableToSpan = new SnapshotSpan(start, triggerLocation);

            if (lineText.IsInQuotation(linePosition)) {

                var quotedExtent       = lineText.QuotedExtent(linePosition);
                var previousIdentifier = lineText.GetPreviousIdentifier(quotedExtent.Start - 1);

                if (previousIdentifier == SyntaxFacts.TaskrefKeyword) {
                    applicableToSpan = new SnapshotSpan(
                        line.Start + quotedExtent.Start,
                        line.Start + quotedExtent.End);
                }
            }

            return false;
        }

        public override Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.CodeGenerationUnitAndSnapshot;
            if (generationUnitAndSnapshot == null) {
                return CreateEmptyCompletionContext();
            }

            var codeGenerationUnit = generationUnitAndSnapshot.CodeGenerationUnit;
            var snapshot           = generationUnitAndSnapshot.Snapshot;
            var triggerToken       = codeGenerationUnit.Syntax.FindToken(triggerLocation);

            bool isInComment = triggerToken.Type == SyntaxTokenType.SingleLineComment ||
                               triggerToken.Type == SyntaxTokenType.MultiLineComment;

            // Keine Autocompletion in Kommentaren!
            if (isInComment) {
                return CreateEmptyCompletionContext();
            }

            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();
            var start        = line.GetStartOfIdentifier(triggerLocation);

            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();
            var previousWordSpan           = line.GetSpanOfPreviousIdentifier(start);

            var  prevousIdentfier = previousWordSpan?.GetText() ?? "";
            bool showEdgeModes    = previousNonWhitespace == '-';

            if (showEdgeModes) {
                start -= 1;
            }

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();

            var isInCodeBlock = lineText.IsInTextBlock(linePosition, SyntaxFacts.OpenBracket, SyntaxFacts.CloseBracket);

            // Es gibt derzeit eigentlich nur die taskrefs wo innerhalb von "" etwas vorgeschlagen werden kann
            if (lineText.IsInQuotation(linePosition)) {

                var quotatedExtent     = lineText.QuotedExtent(linePosition);
                var previousIdentifier = lineText.GetPreviousIdentifier(quotatedExtent.Start - 1);

                if (previousIdentifier == SyntaxFacts.TaskrefKeyword) {

                    var typedSpan = new SnapshotSpan(
                        line.Start + quotatedExtent.Start,
                        triggerLocation);

                    var unused = snapshot.CreateTrackingSpan(typedSpan, SpanTrackingMode.EdgeInclusive);

                    var fi = codeGenerationUnit.Syntax.SyntaxTree.SourceText.FileInfo;
                    if (fi?.DirectoryName != null) {
                        var files = Directory.EnumerateFiles(path: fi.DirectoryName,
                                                             searchPattern: $"*{NavLanguageContentDefinitions.FileExtension}",
                                                             searchOption: SearchOption.TopDirectoryOnly);
                        foreach (var file in files) {
                            completionItems.Add(CreateFileNameCompletion(fi.Directory, new FileInfo(file)));
                        }

                        //foreach (var file in _navFileCompletionCache.GetNavFiles()) {
                        //    itemsBuilder.Add(CreateFileNameCompletion(fi.Directory, file));
                        //}
                    }

                }

                // Wenn in Quotation, dann sind wir fertig...
                return CreateCompletionContext(completionItems);

            }

            // Edge Modes
            if (showEdgeModes) {

                completionItems.Add(CreateKeywordCompletion(SyntaxFacts.GoToEdgeKeyword));
                completionItems.Add(CreateKeywordCompletion(SyntaxFacts.ModalEdgeKeyword));

                if (completionItems.Any()) {
                    return CreateCompletionContext(completionItems);
                }
            }

            // Code Keyword
            if (previousNonWhitespace == SyntaxFacts.OpenBracket) {

            }

            if (prevousIdentfier == SyntaxFacts.TaskKeyword) {
                var taskDecls = codeGenerationUnit.TaskDeclarations;
                foreach (var decl in taskDecls) {

                    completionItems.Add(CreateSymbolCompletion(decl, "decl"));

                }

                if (completionItems.Any()) {
                    return CreateCompletionContext(completionItems);
                }

            }

            var extent = TextExtent.FromBounds(triggerLocation, triggerLocation);

            var taskDefinition = codeGenerationUnit.TaskDefinitions
                                                   .FirstOrDefault(td => td.Syntax.Extent.IntersectsWith(extent));

            if (taskDefinition == null) {
                taskDefinition = codeGenerationUnit.TaskDefinitions
                                                   .LastOrDefault(td => extent.Start > td.Syntax.Start);
            }

            if (taskDefinition != null && !isInCodeBlock) {

                // Exit Connection Points
                if (previousNonWhitespace == SyntaxFacts.Colon) {

                    var exitNodeEnd   = start - 1;
                    var exitNodeStart = exitNodeEnd;

                    var nodeSpan = line.GetSpanOfPreviousIdentifier(exitNodeStart);
                    var nodeName = nodeSpan?.GetText();

                    if (!String.IsNullOrEmpty(nodeName)) {

                        var exitNodeCandidate = taskDefinition.TryFindNode(nodeName) as ITaskNodeSymbol;

                        if (exitNodeCandidate?.Declaration != null) {
                            // Erst die noch nicht verbundenen...
                            foreach (var cp in exitNodeCandidate.GetUnconnectedExits()) {

                                completionItems.Add(CreateSymbolCompletion(cp, cp.Name));

                            }

                            // Dann die bereits verbundenen
                            foreach (var cp in exitNodeCandidate.GetConnectedExits()) {

                                completionItems.Add(CreateSymbolCompletion(cp, cp.Name));
                            }
                        }

                        if (completionItems.Any()) {
                            return CreateCompletionContext(completionItems);
                        }
                    }
                }

                // Erst alle Knoten ohne Referenzen...
                foreach (var node in taskDefinition.NodeDeclarations
                                                   .Where(n => n.References.Count == 0)
                                                   .OrderBy(n => n.Name)) {
                    var description = node.Syntax.ToString();

                    completionItems.Add(CreateSymbolCompletion(node, description));
                }

                // ...dann alle übrigen
                foreach (var node in taskDefinition.NodeDeclarations
                                                   .Where(n => n.References.Count != 0)
                                                   .OrderBy(n => n.Name)) {

                    var description = node.Syntax.ToString();

                    completionItems.Add(CreateSymbolCompletion(node, description));
                }

            }

            // Keywords
            if (!isInCodeBlock) {
                foreach (var keyword in SyntaxFacts.NavKeywords.OrderBy(n => n)) {
                    completionItems.Add(CreateKeywordCompletion(keyword));
                }
            }

            return CreateCompletionContext(completionItems);
        }

    }

}