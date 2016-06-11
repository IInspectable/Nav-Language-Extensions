#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    class GoToNavTagger: ITagger<GoToNavTag>, IDisposable {

        readonly ITextBuffer _textBuffer;

        public GoToNavTagger(ITextBuffer textBuffer) {
            _textBuffer = textBuffer;
            _textBuffer.Changed += OnTextBufferChanged;
        }

        public void Dispose() {
            _textBuffer.Changed -= OnTextBufferChanged;
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) {
            if (e.Changes.Count == 0)
                return;

            ITextSnapshot snapshot = e.After;

            int start = e.Changes[0].NewPosition;
            int end = e.Changes[e.Changes.Count - 1].NewEnd;

            SnapshotSpan totalAffectedSpan = new SnapshotSpan(
                snapshot.GetLineFromPosition(start).Start,
                snapshot.GetLineFromPosition(end).End);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }

        public IEnumerable<ITagSpan<GoToNavTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            return GetTags();
        }

        IEnumerable<ITagSpan<GoToNavTag>> GetTags() {

            var currentSnapshot = _textBuffer.CurrentSnapshot;

            var document = currentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null) {
                yield break;
            }

            var semanticModel = document.GetSemanticModelAsync().Result;
            var rootNode = semanticModel.SyntaxTree.GetRoot();
           
            var classDeclarations = rootNode.DescendantNodesAndSelf()
                                            .OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations) {

                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                var navTaskInfo = GetNavTaskInfo(classSymbol);

                // In der Basisklasse nachsehen
                if (navTaskInfo == null) {
                    navTaskInfo = GetNavTaskInfo(classSymbol?.BaseType);
                }

                if (navTaskInfo == null) {
                    continue;
                }

                var start = classDeclaration.Identifier.Span.Start;
                var length = classDeclaration.Identifier.Span.Length;

                var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                yield return new TagSpan<GoToNavTag>(snapshotSpan, new GoToNavTag(navTaskInfo));

                var methodDeclarations = classDeclaration.DescendantNodes()
                                                         .OfType<MethodDeclarationSyntax>();

                foreach (var methodDeclaration in methodDeclarations) {

                    var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
                    var triggerInfo  = GetNavTriggerInfo(methodSymbol, navTaskInfo);

                    // In den überschriebenen Methoden nachsehen
                    if (triggerInfo == null) {
                        triggerInfo = GetNavTriggerInfo(methodSymbol?.OverriddenMethod, navTaskInfo);         
                    }

                    if(triggerInfo == null) {
                        continue;
                    }

                    start = methodDeclaration.Identifier.Span.Start;
                    length = methodDeclaration.Identifier.Span.Length;

                    snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                    yield return new TagSpan<GoToNavTag>(snapshotSpan, new GoToNavTag(triggerInfo));
                }                
            }
        }
        
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        [CanBeNull]
        NavTaskInfo GetNavTaskInfo(INamedTypeSymbol classSymbol) {

            // Die Klasse kann in mehrere partial classes aufgeteilt sein
            var navTaskInfo = classSymbol?.DeclaringSyntaxReferences
                                          .Select(dsr => dsr.GetSyntax() as ClassDeclarationSyntax)
                                          .Select(GetNavTaskInfo).FirstOrDefault(nti => nti != null);
            return navTaskInfo;
        }

        [CanBeNull]
        NavTaskInfo GetNavTaskInfo(ClassDeclarationSyntax classDeclaration) {

            var tags = ReadNavTags(classDeclaration).ToList();

            var navFileTag  = tags.FirstOrDefault(t => t.TagName == "NavFile");
            var navFileName = navFileTag?.Content;

            var navTaskTag  = tags.FirstOrDefault(t => t.TagName == "NavTask");
            var navTaskName = navTaskTag?.Content;

            if (String.IsNullOrWhiteSpace(navFileName) || String.IsNullOrWhiteSpace(navTaskName)) {
                return null;
            }

            // Relative Pfadangabe sind Standard.
            if (!Path.IsPathRooted(navFileName)) {
                var declaringNodePath = classDeclaration.GetLocation().GetLineSpan().Path;

                if (String.IsNullOrWhiteSpace(declaringNodePath)) {
                    return null;
                }

                var declaringDir = Path.GetDirectoryName(declaringNodePath);
                if (declaringDir == null) {
                    return null;
                }

                navFileName = Path.GetFullPath(Path.Combine(declaringDir, navFileName));
            }

            var candidate = new NavTaskInfo {
                NavFileName = navFileName,
                TaskName    = navTaskName
            };

            return candidate;
        }

        [CanBeNull]
        NavTriggerInfo GetNavTriggerInfo(IMethodSymbol method, NavTaskInfo taskInfo) {

            var navTriggerInfo = method?.DeclaringSyntaxReferences
                                       .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                       .Select(syntax => GetNavTriggerInfo(syntax, taskInfo))
                                       .FirstOrDefault(nti => nti != null);

            return navTriggerInfo;
        }

        [CanBeNull]
        NavTriggerInfo GetNavTriggerInfo(MethodDeclarationSyntax methodDeclaration, NavTaskInfo taskInfo) {

            var tags = ReadNavTags(methodDeclaration).ToList();

            var navTriggerTag  = tags.FirstOrDefault(t => t.TagName == "NavTrigger");
            var navTriggerName = navTriggerTag?.Content;

            if(String.IsNullOrEmpty(navTriggerName)) {                
                return null;
            }

            return new NavTriggerInfo {
                NavFileName = taskInfo.NavFileName,
                TaskName    = taskInfo.TaskName,
                TriggerName = navTriggerName
            };
        }
        
        [NotNull]
        IEnumerable<NavTag> ReadNavTags(Microsoft.CodeAnalysis.SyntaxNode node) {

            var trivias = node.GetLeadingTrivia()
                              .Where(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            foreach (var trivia in trivias) {

                if (!trivia.HasStructure) {
                    continue;
                }

                var xmlElementSyntaxes = trivia.GetStructure()
                        .ChildNodes()
                        .OfType<XmlElementSyntax>()
                        .ToList();

                // Wir suchen alle Tags, deren Namen mit Nav beginnt
                foreach (var xmlElementSyntax in xmlElementSyntaxes) {
                    var startTagName = xmlElementSyntax.StartTag?.Name?.ToString();
                    if (startTagName?.StartsWith("Nav") == true) {
                        yield return new NavTag {
                            TagName = startTagName,
                            Content = xmlElementSyntax.Content.ToString()
                        };
                    }
                }
            }
        }

        sealed class NavTag {
            public string TagName { get; set; }
            public string Content { get; set; }
        }
    }
}