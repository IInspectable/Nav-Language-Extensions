#region Using Directives

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.Common;
using Pharmatechnik.Nav.Language.FindReferences;

using SourceText = Microsoft.CodeAnalysis.Text.SourceText;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindReferences {

    public partial class WfsReferenceFinder {

        static readonly ImmutableArray<ClassInfo> NavlessClasses = new[] {
                new ClassInfo(projectName: "XTplus.Kasse",             className: "Pharmatechnik.Apotheke.XTplus.Kasse.WFL.KasseWFS"),
                new ClassInfo(projectName: "XTplus.Kontaktverwaltung", className: "Pharmatechnik.Apotheke.XTplus.Kontaktverwaltung.StandardKontakteSuche.WFL.StandardKontakteSucheWFS")
            }
           .ToImmutableArray();

        public static async Task FindReferencesAsync(Solution solution, FindReferencesArgs args) {

            if (args.OriginatingSymbol is ITaskDefinitionSymbol taskDefinition) {

                var nodeDefinition                 = DefinitionItem.CreateTaskDefinitionItem(taskDefinition);
                var initConnectionPointDefinition  = DefinitionItem.CreateInitConnectionPointDefinition(taskDefinition, false);
                var exitConnectionPointDefinitions = DefinitionItem.CreateExitConnectionPointDefinitions(taskDefinition, false);
                var taskDeclarationCodeInfo        = TaskDeclarationCodeInfo.FromTaskDeclaration(taskDefinition.AsTaskDeclaration);

                foreach (var project in solution.Projects) {

                    foreach (var classInfo in NavlessClasses.Where(ci => ci.ProjectName == project.Name)) {

                        var compilation = await project.GetCompilationAsync(args.Context.CancellationToken).ConfigureAwait(false);

                        var wfsClass = compilation.GetTypeByMetadataName(classInfo.ClassName);
                        if (wfsClass == null) {
                            continue;
                        }

                        var fields = GetReferencingFields(wfsClass, taskDeclarationCodeInfo);

                        await FindTaskReferences(args.Context, solution, compilation, taskDefinition, fields, nodeDefinition).ConfigureAwait(false);
                        await FindInitReferences(args.Context, solution, compilation, taskDefinition, fields, initConnectionPointDefinition).ConfigureAwait(false);
                        await FindExitReferences(args.Context, solution, compilation, taskDefinition, fields, exitConnectionPointDefinitions).ConfigureAwait(false);

                    }

                }
            }

        }

        static async Task FindTaskReferences(IFindReferencesContext context,
                                             Solution solution,
                                             Compilation compilation,
                                             ITaskDefinitionSymbol taskDefinition,
                                             ImmutableArray<IFieldSymbol> fields,
                                             DefinitionItem nodeDefinition) {

            foreach (var referenceItem in (await FindReferences().ConfigureAwait(false)).OrderByLocation()) {
                await context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);
            }

            async Task<ImmutableArray<ReferenceItem>> FindReferences() {

                var referenceItems = ImmutableArray.CreateBuilder<ReferenceItem>();

                foreach (var field in fields) {

                    foreach (var syntaxReference in field.DeclaringSyntaxReferences) {

                        var variableDeclarator = await syntaxReference.GetSyntaxAsync(context.CancellationToken).ConfigureAwait(false) as VariableDeclaratorSyntax;
                        
                        if (!TryFindTypeSyntax(variableDeclarator, out var typeSyntax)) {
                            continue;
                        }

                        if (!TryGetLocation(typeSyntax.GetLocation(), out var location)) {
                            continue;
                        }

                        var referenceItem = await CreateReferenceItemAsync(definition       : nodeDefinition,
                                                                           referenceLocation: location,
                                                                           solution         : solution,
                                                                           syntaxTree       : typeSyntax.SyntaxTree, 
                                                                           cancellationToken: context.CancellationToken).ConfigureAwait(false);

                        referenceItems.Add(referenceItem);

                    }

                }

                return referenceItems.ToImmutableArray();

            }

            bool TryFindTypeSyntax(VariableDeclaratorSyntax variableDeclaratorSyntax, out TypeSyntax typeSyntax) {

                typeSyntax = default;

                var node = variableDeclaratorSyntax?.Parent;

                while (node != null) {
                    if (node is FieldDeclarationSyntax fds && fds.Declaration.Type != null) {
                        typeSyntax = fds.Declaration.Type;
                        return true;
                    }

                    node = node.Parent;
                }

                return false;
            }
        }
       
        static async Task FindInitReferences(IFindReferencesContext context,
                                             Solution solution,
                                             Compilation compilation,
                                             ITaskDefinitionSymbol taskDefinition,
                                             ImmutableArray<IFieldSymbol> fields,
                                             DefinitionItem initDefinitionItem) {

            foreach (var referenceItem in (await FindReferences().ConfigureAwait(false)).OrderByLocation()) {
                await context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);
            }

            async Task<ImmutableArray<ReferenceItem>> FindReferences() {

                var referenceItems = ImmutableArray.CreateBuilder<ReferenceItem>();

                foreach (var field in fields) {

                    var references = await SymbolFinder.FindReferencesAsync(field, solution, context.CancellationToken)
                                                       .ConfigureAwait(false);

                    foreach (var reference in references) {

                        foreach (var referenceLocation in reference.Locations) {

                            if (!referenceLocation.Document.TryGetSyntaxTree(out var syntaxTree)) {
                                continue;
                            }

                            var syntaxNode = syntaxTree.GetRoot().FindNode(referenceLocation.Location.SourceSpan);

                            // Sicherstellen, dass die Referenz ein Methodenaufruf darstellt
                            // node.Parent => MemberAccessExpressionSyntax
                            // node.Parent.Parent => InvocationExpressionSyntax
                            if (!(syntaxNode?.Parent?.Parent is InvocationExpressionSyntax)) {
                                continue;
                            }

                            if (!TryGetLocation(syntaxNode.GetLocation(), out var location)) {
                                continue;
                            }

                            var referenceItem = await CreateReferenceItemAsync(definition       : initDefinitionItem,
                                                                               referenceLocation: location,
                                                                               solution         : solution,
                                                                               syntaxTree       : syntaxTree,
                                                                               cancellationToken: context.CancellationToken).ConfigureAwait(false);

                            referenceItems.Add(referenceItem);

                        }
                    }

                }

                return referenceItems.ToImmutableArray();
            }

        }

        private static Task FindExitReferences(IFindReferencesContext context,
                                               Solution solution,
                                               Compilation compilation,
                                               ITaskDefinitionSymbol taskDefinition,
                                               ImmutableArray<IFieldSymbol> fields,
                                               ImmutableDictionary<Location, DefinitionItem> exitDefinitionItems) {
            return Task.CompletedTask;
        }

        static ImmutableArray<IFieldSymbol> GetReferencingFields(INamedTypeSymbol wfsClass, TaskDeclarationCodeInfo taskDeclarationCodeInfo) {

            var fullyQualifiedFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

            return wfsClass.GetMembers()
                           .OfType<IFieldSymbol>()
                           .Where(f => f.Type.ToDisplayString(fullyQualifiedFormat) == taskDeclarationCodeInfo.FullyQualifiedBeginInterfaceName)
                           .ToImmutableArray();

        }
        
                static async Task<ReferenceItem> CreateReferenceItemAsync(DefinitionItem definition,
                                                                  Location referenceLocation,
                                                                  Solution solution,
                                                                  Microsoft.CodeAnalysis.SyntaxTree syntaxTree,
                                                                  CancellationToken cancellationToken) {

            var        document   = solution.GetDocument(syntaxTree);
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            var textSpan = GetPreviewSpan(sourceText, referenceLocation.Start);

            var textParts           = await ToClassifiedTextAsync(document, textSpan, cancellationToken).ConfigureAwait(false);
            var textHighlightExtent = new TextExtent(referenceLocation.Start - textSpan.Start, referenceLocation.Length);

            var toolTipSpan  = GetPreviewSpan(sourceText, referenceLocation.Start, 3);
            var toolTipParts = await ToClassifiedTextAsync(document, toolTipSpan, cancellationToken).ConfigureAwait(false);

            var toolTipHighlightExtent = new TextExtent(referenceLocation.Start - toolTipSpan.Start, referenceLocation.Length);

            var referenceItem = new ReferenceItem(
                definition            : definition,
                location              : referenceLocation,
                textParts             : textParts,
                textHighlightExtent   : textHighlightExtent,
                toolTipParts          : toolTipParts,
                toolTipHighlightExtent: toolTipHighlightExtent);

            return referenceItem;
        }

        static TextSpan GetPreviewSpan(SourceText sourceText, int position, int previewLines = 0) {

            var lineNumber = sourceText.Lines.GetLineFromPosition(position).LineNumber;

            int startLineNumber = Math.Max(0, lineNumber          - previewLines);
            int endLine         = Math.Min(sourceText.Lines.Count - 1, lineNumber + previewLines);

            return TextSpan.FromBounds(sourceText.Lines[startLineNumber].Start,
                                       sourceText.Lines[endLine].End);

        }

        static async Task<ImmutableArray<ClassifiedText>> ToClassifiedTextAsync(Document document, TextSpan span, CancellationToken cancellationToken) {
            
            var builder         = ImmutableArray.CreateBuilder<ClassifiedText>();
            var sourceText      = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            var firstLine = sourceText.Lines.GetLineFromPosition(span.Start).LineNumber;
            var lastLine=sourceText.Lines.GetLineFromPosition(span.End).LineNumber;

            int currentLine = firstLine;
            while (currentLine <= lastLine) {

                var lineSpan = currentLine == lastLine ? sourceText.Lines[currentLine].Span : sourceText.Lines[currentLine].SpanIncludingLineBreak;

                currentLine++;

                var classifiedSpans = await Classifier.GetClassifiedSpansAsync(document, lineSpan, cancellationToken).ConfigureAwait(false);

                int currentEnd = lineSpan.Start;
                foreach (var classifiedSpan in classifiedSpans) {

                    // TODO auf lineSpan eindampfen. Es schein, als würde der Classifier den Span verbreitern (z.B. bei über mehrere Zeilen gehenden Verbatim Strings)
                    var textSpan = classifiedSpan.TextSpan;

                    if (textSpan.Start > currentEnd) {
                        ProcessUnclassifiedSpan(TextSpan.FromBounds(currentEnd, textSpan.Start));
                    }

                    var text = sourceText.ToString(textSpan);
                    var tc   = GetClassification(classifiedSpan.ClassificationType);

                    builder.Add(new ClassifiedText(text, tc));

                    currentEnd = textSpan.End;
                }

                if (currentEnd < lineSpan.End) {
                    ProcessUnclassifiedSpan(TextSpan.FromBounds(currentEnd, lineSpan.End));
                }

            }

            return builder.ToImmutableArray();

            void ProcessUnclassifiedSpan(TextSpan unhandledSpan) {

                var ws = sourceText.ToString(unhandledSpan);
                if (String.IsNullOrWhiteSpace(ws)) {
                    builder.Add(ClassifiedTexts.Whitespace(ws));
                } else {
                    builder.Add(ClassifiedTexts.Text(ws));
                }
            }
        }


        // Nicht vollständig. Haupsache die Vorschau sieht hübsch aus
        static TextClassification GetClassification(string c) {

            if (c == ClassificationTypeNames.ClassName     ||
                c == ClassificationTypeNames.InterfaceName ||
                c == ClassificationTypeNames.EnumName      ||
                c == ClassificationTypeNames.StructName) {
                return TextClassification.TaskName;
            }

            if (c == ClassificationTypeNames.Keyword) {
                return TextClassification.Keyword;
            }

            if (c == ClassificationTypeNames.StringLiteral ||
                c == ClassificationTypeNames.VerbatimStringLiteral) {
                return TextClassification.StringLiteral;
            }

            if (c == ClassificationTypeNames.Punctuation) {
                return TextClassification.Punctuation;
            }

            if (c == ClassificationTypeNames.WhiteSpace) {
                return TextClassification.Whitespace;
            }

            if (c == ClassificationTypeNames.Comment) {
                return TextClassification.Comment;
            }

            return TextClassification.Text;
        }
        
        private static bool TryGetLocation(Microsoft.CodeAnalysis.Location loc, out Location location) {
            
            location = default;

            if (loc == null) {
                return false;
            }

            var filePath = loc.SourceTree?.FilePath;
            if (filePath == null || !loc.IsInSource) {
                return false;
            }

            var lineSpan = loc.GetLineSpan();
            if (!lineSpan.IsValid) {
                return false;
            }

            var textExtent = loc.SourceSpan.ToTextExtent();
            var lineRange  = lineSpan.ToLineRange();

            location = new Location(textExtent, lineRange, filePath);
            return true;
        }

    }

}