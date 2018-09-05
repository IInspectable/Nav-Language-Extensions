#region Using Directives

using System;
using System.Collections.Immutable;
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

    class NavCompletionSource: AsyncCompletionSource {

        public NavCompletionSource(QuickinfoBuilderService quickinfoBuilderService): base(quickinfoBuilderService) {

        }

        char edgeTrigger = '-';

        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            bool IsTriggerChar() {

                return char.IsLetter(typedChar) ||
                       typedChar == '\0'        ||
                       typedChar == edgeTrigger ||
                       typedChar == SyntaxFacts.Colon;
            }

            applicableToSpan = default;

            if (!IsTriggerChar()) {
                return false;
            }

            return ShouldProvideCompletions(triggerLocation, out applicableToSpan);
        }

        public override Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.CodeGenerationUnitAndSnapshot;
            if (generationUnitAndSnapshot == null) {
                return CreateEmptyCompletionContext();
            }

            var codeGenerationUnit = generationUnitAndSnapshot.CodeGenerationUnit;
            var triggerToken       = codeGenerationUnit.Syntax.FindToken(triggerLocation);

            bool isInComment = triggerToken.Type == SyntaxTokenType.SingleLineComment ||
                               triggerToken.Type == SyntaxTokenType.MultiLineComment;

            // Keine Auto Completion in Kommentaren!
            if (isInComment) {
                return CreateEmptyCompletionContext();
            }

            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();

            // Kein Auto Completion in ""
            if (lineText.IsInQuotation(linePosition)) {
                return CreateEmptyCompletionContext();
            }

            // Kein Auto Completion in in Code Blöcken
            var isInCodeBlock = lineText.IsInTextBlock(linePosition, SyntaxFacts.OpenBracket, SyntaxFacts.CloseBracket);
            if (isInCodeBlock) {
                return CreateEmptyCompletionContext();
            }

            var start = line.GetStartOfIdentifier(triggerLocation);

            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();
            var previousWordSpan           = line.GetSpanOfPreviousIdentifier(start);

            var prevousIdentfier = previousWordSpan?.GetText() ?? "";

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();

            // Edge Modes
            bool showEdgeModes = previousNonWhitespace == '-';
            if (showEdgeModes) {

                completionItems.Add(CreateKeywordCompletion(SyntaxFacts.GoToEdgeKeyword));
                completionItems.Add(CreateKeywordCompletion(SyntaxFacts.ModalEdgeKeyword));

                if (completionItems.Any()) {
                    return CreateCompletionContext(completionItems);
                }
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

            if (taskDefinition != null) {

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
            foreach (var keyword in SyntaxFacts.NavKeywords.OrderBy(n => n)) {
                completionItems.Add(CreateKeywordCompletion(keyword));
            }

            return CreateCompletionContext(completionItems);
        }

        bool ShouldProvideCompletions(SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan) {

            applicableToSpan = default;

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

            // Kein Auto Completion in ""
            var line         = triggerLocation.GetContainingLine();
            var linePosition = triggerLocation - line.Start;
            var lineText     = line.GetText();

            if (lineText.IsInQuotation(linePosition)) {
                return false;
            }

            // Kein Auto Completion in in Code Blöcken
            var isInCodeBlock = lineText.IsInTextBlock(linePosition, SyntaxFacts.OpenBracket, SyntaxFacts.CloseBracket);
            if (isInCodeBlock) {
                return false;
            }

            var start = line.GetStartOfIdentifier(triggerLocation);

            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();

            bool showEdgeModes = previousNonWhitespace == edgeTrigger;

            if (showEdgeModes) {
                start -= 1;
            }

            applicableToSpan = new SnapshotSpan(start, triggerLocation);

            return true;
        }

    }

}