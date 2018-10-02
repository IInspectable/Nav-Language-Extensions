﻿#region Using Directives

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Language.Text;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {
    
    class NavCompletionSource: AsyncCompletionSource {

        public NavCompletionSource(QuickinfoBuilderService quickinfoBuilderService): base(quickinfoBuilderService) {

        }
        
        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            bool IsTriggerChar() {

                return char.IsLetter(typedChar)     ||
                       typedChar == '\0'            ||
                       typedChar == SyntaxFacts.Colon;
            }

            applicableToSpan = default;

            if (!IsTriggerChar()) {
                return false;
            }

            var codeGenerationUnit = GetCodeGenerationUnit(triggerLocation);

            return ShouldProvideCompletions(triggerLocation, codeGenerationUnit, out applicableToSpan);
        }

        public override async Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            
            var codeGenerationUnit = GetCodeGenerationUnit(triggerLocation);

            if (!ShouldProvideCompletions(triggerLocation, codeGenerationUnit, out _)) {
                return CreateEmptyCompletionContext();
            }

            await Task.Yield();
           
            var triggerLine       = triggerLocation.GetContainingLine();
            var startOfIdentifier = triggerLine.GetStartOfIdentifier(triggerLocation);

            var previousNonWhitespacePoint = triggerLine.GetPreviousNonWhitespace(startOfIdentifier);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();
            var previousWordSpan           = triggerLine.GetSpanOfPreviousIdentifier(startOfIdentifier);

            var prevousIdentfier = previousWordSpan?.GetText() ?? "";

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();
            
            // Task Nodes
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

                    var exitNodeEnd   = startOfIdentifier - 1;
                    var exitNodeStart = exitNodeEnd;

                    var nodeSpan = triggerLine.GetSpanOfPreviousIdentifier(exitNodeStart);
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

            // Nav Keywords ohne Edges
            foreach (var keyword in SyntaxFacts.NavKeywords
                                               .Where(k => !SyntaxFacts.IsHiddenKeyword(k) && !SyntaxFacts.IsEdgeKeyword(k))
                                               .OrderBy(k => k)) {

                completionItems.Add(CreateKeywordCompletion(keyword));
            }

            return CreateCompletionContext(completionItems);
        }

        bool ShouldProvideCompletions(SnapshotPoint triggerLocation, CodeGenerationUnit codeGenerationUnit, out SnapshotSpan applicableToSpan) {

            applicableToSpan = default;

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

            // Kein Auto Completion in Code Blöcken
            // TODO Nicht vollständig, da nur aktuelle Zeile betrachtet wird
            var isInCodeBlock = lineText.IsInTextBlock(linePosition, SyntaxFacts.OpenBracket, SyntaxFacts.CloseBracket);
            if (isInCodeBlock) {
                return false;
            }

            var start = line.GetStartOfIdentifier(triggerLocation);
            if (start == triggerLocation) {
                return false;
            }
            
            applicableToSpan = new SnapshotSpan(start, triggerLocation);

            return true;
        }

        

    }

}