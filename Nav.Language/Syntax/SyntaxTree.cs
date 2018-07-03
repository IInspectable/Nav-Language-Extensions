﻿#region Using Directives

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Language.Internal;
using Pharmatechnik.Nav.Language.Generated;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class SyntaxTree {

        internal SyntaxTree(SourceText sourceText,
                            SyntaxNode root,
                            SyntaxTokenList tokens,
                            ImmutableArray<Diagnostic> diagnostics) {

            Root        = root       ?? throw new ArgumentNullException(nameof(root));
            Tokens      = tokens     ?? SyntaxTokenList.Empty;
            SourceText  = sourceText ?? SourceText.Empty;
            Diagnostics = diagnostics;
        }

        [NotNull]
        public SyntaxNode Root { get; }

        [NotNull]
        public SourceText SourceText { get; }

        [NotNull]
        public SyntaxTokenList Tokens { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public static SyntaxTree ParseText(string text, string filePath = null, CancellationToken cancellationToken = default) {

            return ParseText(text: text,
                             treeCreator: parser => parser.codeGenerationUnit(),
                             filePath: filePath,
                             cancellationToken: cancellationToken);
        }

        internal static SyntaxTree ParseText(string text,
                                             Func<NavGrammar, IParseTree> treeCreator,
                                             string filePath,
                                             Encoding encoding = null,
                                             CancellationToken cancellationToken = default) {

            text = text ?? String.Empty;

            var sourceText    = SourceText.From(text, filePath);
            var diagnostics   = ImmutableArray.CreateBuilder<Diagnostic>();
            var stream        = new AntlrInputStream(text);
            var tokenSource   = new NavTokens(stream);
            var cts           = new NavCommonTokenStream(tokenSource);
            var parser        = new NavGrammar(cts);
            var errorListener = new NavErrorListener(filePath, diagnostics);

            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            var tree    = treeCreator(parser);
            var visitor = new NavGrammarVisitor(expectedTokenCount: cts.AllTokens.Count);
            var syntax  = visitor.Visit(tree);
            var tokens  = PostprocessTokens(visitor.Tokens, cts, syntax, diagnostics, filePath, cancellationToken);

            var syntaxTree = new SyntaxTree(sourceText : sourceText,
                                            root       : syntax,
                                            tokens     : tokens,
                                            diagnostics: diagnostics.ToImmutable());

            syntax.FinalConstruct(syntaxTree, null);

            return syntaxTree;
        }

        static SyntaxTokenList PostprocessTokens(
            List<SyntaxToken> tokens,
            NavCommonTokenStream cts,
            SyntaxNode syntax,
            ImmutableArray<Diagnostic>.Builder diagnostics,
            string filePath,
            CancellationToken cancellationToken) {

            var finalTokens = new List<SyntaxToken>(cts.AllTokens.Count);
            tokens.Sort(SyntaxTokenComparer.Default);

            // Wir haben bereits die signifikanten Token (T) im GrammarVisitor erstellt.
            // Wir können nicht alle Tokens hier ermitteln, da der Visitor viel mehr Kontextinformationen
            // hat. Somit wird aus einem "Identifier" z.B. je nach Kontext ein Keyword, oder Symbol (=> Classification).
            // Was uns hier jedoch noch fehlt sind vor allem die Whitespaces (w) und "unbekannten" (u),
            // die der Parser nie zu Gesicht bekommt. Der TokenStream liefert uns alle Token
            // (candidates = c):
            // -T---TTT---T----T-- <= bereits im Visitor erfasste Token
            // ccccccccccccccccccc <= alle Tokens (candidates)
            // wTwwwTTTwwwTwwwwTuu <= die Tokens, wie wir sie hier haben wollen
            var index = 0;
            foreach (var candidate in cts.AllTokens) {

                cancellationToken.ThrowIfCancellationRequested();

                if (index < tokens.Count) {
                    var existing = tokens[index];
                    // Das Token wurde bereits im Visitor erfasst (T)
                    if (existing.Start == candidate.StartIndex) {
                        finalTokens.Add(existing);
                        index++;
                        continue;
                    }
                }

                // Das Token existiert noch nicht, da es der Parser/Visitor offensichtlich nicht "erwischt hat" (t, u)
                SyntaxTokenClassification tokenClassification;
                switch (candidate.Channel) {
                    case NavTokens.TriviaChannel:
                        switch (candidate.Type) {
                            case NavTokens.NewLine:
                                tokenClassification = SyntaxTokenClassification.Whitespace;
                                break;
                            case NavTokens.Whitespace:
                                tokenClassification = SyntaxTokenClassification.Whitespace;
                                break;
                            case NavTokens.SingleLineComment:
                                tokenClassification = SyntaxTokenClassification.Comment;
                                break;
                            case NavTokens.MultiLineComment:
                                tokenClassification = SyntaxTokenClassification.Comment;
                                break;
                            case NavTokens.Unknown:
                                tokenClassification = SyntaxTokenClassification.Skiped;
                                break;
                            default:
                                // Wir haben sonst eigentlich nix im Trivia Channel
                                throw new ArgumentException();
                        }

                        break;
                    case Lexer.DefaultTokenChannel:
                        tokenClassification = SyntaxTokenClassification.Skiped;
                        break;
                    default:
                        throw new ArgumentException();
                }

                // TODO: hier evtl. den "echten" Parent herausfinden...
                SyntaxNode parent = syntax;

                // Fix Für Single Line Comments, da diese leider immer auch das EOL beinhalten
                if (candidate.Type == NavGrammar.SingleLineComment) {
                    foreach (var token in SplitSingleLineCommenTokens(candidate, parent)) {
                        finalTokens.Add(token);
                    }
                } else {

                    finalTokens.Add(SyntaxTokenFactory.CreateToken(candidate, tokenClassification, parent));

                    if (candidate.Type == NavGrammar.Unknown) {
                        diagnostics.Add(
                            new Diagnostic(candidate.GetLocation(filePath),
                                           DiagnosticDescriptors.Syntax.Nav0000UnexpectedCharacter,
                                           candidate.Text));
                    }
                }

            }

            return SyntaxTokenList.AttachSortedTokens(finalTokens);
        }

        static IEnumerable<SyntaxToken> SplitSingleLineCommenTokens(IToken candidate, SyntaxNode parent) {
            int newLineIndex = FindNewLineIndexInSingleLineComment(candidate.Text);
            if (newLineIndex > 0) {
                var tokenExtent   = TextExtent.FromBounds(candidate.StartIndex, candidate.StartIndex + newLineIndex);
                var newLineExtent = TextExtent.FromBounds(candidate.StartIndex                       + newLineIndex, candidate.StopIndex + 1);

                yield return SyntaxTokenFactory.CreateToken(tokenExtent,   SyntaxTokenType.SingleLineComment, SyntaxTokenClassification.Comment,    parent);
                yield return SyntaxTokenFactory.CreateToken(newLineExtent, SyntaxTokenType.NewLine,           SyntaxTokenClassification.Whitespace, parent);
            } else {
                yield return SyntaxTokenFactory.CreateToken(candidate, SyntaxTokenClassification.Comment, parent);
            }
        }

        static int FindNewLineIndexInSingleLineComment(string text) {

            char c1 = text[text.Length - 2];
            char c2 = text[text.Length - 1];

            if (c2 == '\n' || c2 == '\r') {

                if (c1 == '\n') {
                    return text.Length - 2;
                }

                return text.Length - 1;
            }

            return -1;
        }

    }

}