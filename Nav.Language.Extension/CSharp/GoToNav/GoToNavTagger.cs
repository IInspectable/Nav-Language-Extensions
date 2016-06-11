#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    class GoToNavTagger: ITagger<GoToNavTag>, IDisposable {

        readonly ITextBuffer _textBuffer;
        readonly IDisposable _parserObs;
        readonly WorkspaceRegistration _workspaceRegistration;

        BuildTagsResult _result;
        Workspace _workspace;

        public GoToNavTagger(ITextBuffer textBuffer) {

            _textBuffer = textBuffer;

            _workspaceRegistration = Workspace.GetWorkspaceRegistration(textBuffer.AsTextContainer());
            _workspaceRegistration.WorkspaceChanged += OnWorkspaceRegistrationChanged;

            if(_workspaceRegistration.Workspace != null) {
                ConnectToWorkspace(_workspaceRegistration.Workspace);
            }

            _parserObs = Observable.FromEventPattern<EventArgs>(
                                               handler => RebuildTriggered += handler,
                                               handler => RebuildTriggered -= handler)
                                   .Select(_ => _textBuffer.CurrentSnapshot)
                                   .Throttle(ServiceProperties.GoToNavTaggerThrottleTime)
                                   .Select(args => Observable.DeferAsync(async token =>
                                   {
                                       var parseResult = await BuildTagsAsync(args, token).ConfigureAwait(false);

                                       return Observable.Return(parseResult);
                                   }))
                                   .Switch()
                                   .ObserveOn(SynchronizationContext.Current)
                                   .Subscribe(TrySetResult);

            Invalidate();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<GoToNavTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            if (_result == null || spans.Count == 0) {
                yield break;
            }

            foreach (var span in spans) {
                foreach (var tag in _result.Tags) {

                    var transSpan = tag.Span.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive);
                    if (transSpan.IntersectsWith(span)) {
                        yield return new TagSpan<GoToNavTag>(transSpan, tag.Tag);
                    }
                }
            }
        }

        void OnWorkspaceRegistrationChanged(object sender, EventArgs e) {

            DisconnectFromWorkspace();

            var newWorkspace = _workspaceRegistration.Workspace;

            if (newWorkspace != null) {
                ConnectToWorkspace(newWorkspace);
            }
        }

        void ConnectToWorkspace(Workspace workspace) {

            _result    = null;

            _workspace = workspace;
            _workspace.WorkspaceChanged += OnWorkspaceChanged;
            _workspace.DocumentOpened   += OnDocumentOpened;

            Invalidate();
        }

        void DisconnectFromWorkspace() {

            _result = null;

            if (_workspace != null) {
                _workspace.WorkspaceChanged -= OnWorkspaceChanged;
                _workspace.DocumentOpened   -= OnDocumentOpened;

                _workspace = null;                
            }
        }

        void OnDocumentOpened(object sender, DocumentEventArgs args) {
            InvalidateIfThisDocument(args.Document.Id);
        }

        void OnWorkspaceChanged(object sender, WorkspaceChangeEventArgs args) {
            // We're getting an event for a workspace we already disconnected from
            if (args.NewSolution.Workspace != _workspace) {
                // we are async so we are getting events from previous workspace we were associated with
                // just ignore them
                return;
            }

            if(args.Kind == WorkspaceChangeKind.DocumentChanged) {
                InvalidateIfThisDocument(args.DocumentId);
            }               
        }

        void InvalidateIfThisDocument(DocumentId documentId) {
            if (_workspace != null) {
                var openDocumentId = _workspace.GetDocumentIdInCurrentContext(_textBuffer.AsTextContainer());
                if (openDocumentId == documentId) {
                    Invalidate();
                }
            }
        }
        
        void TrySetResult(BuildTagsResult result) {

            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (_textBuffer.CurrentSnapshot != result.Snapshot) {   
                return;
            }

            // hier könnte man noch mit dem aktuellem Ergebnis Vergleichen und ggf. gar nichts machen, wenn gleich...
            _result = result;

            var snapshotSpan = result.Snapshot.GetFullSpan();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));
        }

        // Dieses Event feuern wir um den Observer zu "füttern".
        event EventHandler<EventArgs> RebuildTriggered;

        public void Invalidate() {
            RebuildTriggered?.Invoke(this, EventArgs.Empty);
        }
        
        public void Dispose() {
            _workspaceRegistration.WorkspaceChanged -= OnWorkspaceRegistrationChanged;
            DisconnectFromWorkspace();
            _parserObs.Dispose();
        }

        /// <summary>
        /// Achtung: Diese Methode wird bereits in einem Background Thread aufgerufen. Also vorischt bzgl. thread safety!
        /// Deshalb werden die BuildResultArgs bereits vorab im GUI Thread erstellt.
        /// </summary>
        static async Task<BuildTagsResult> BuildTagsAsync(ITextSnapshot snapshot, CancellationToken cancellationToken) {

            return await Task.Run(() => {

                var tags = BuildTags(snapshot).ToList();

                return new BuildTagsResult(tags, snapshot);

            }, cancellationToken).ConfigureAwait(false);
        }

        static IEnumerable<ITagSpan<GoToNavTag>> BuildTags(ITextSnapshot currentSnapshot) {

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

                    // In der überschriebenen Methode nachsehen
                    if (triggerInfo == null) {
                        triggerInfo = GetNavTriggerInfo(methodSymbol?.OverriddenMethod, navTaskInfo);         
                    }

                    if(triggerInfo == null) {
                        continue;
                    }

                    start  = methodDeclaration.Identifier.Span.Start;
                    length = methodDeclaration.Identifier.Span.Length;

                    snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                    yield return new TagSpan<GoToNavTag>(snapshotSpan, new GoToNavTag(triggerInfo));
                }                
            }
        }
        
        [CanBeNull]
        static NavTaskInfo GetNavTaskInfo(INamedTypeSymbol classSymbol) {

            // Die Klasse kann in mehrere partial classes aufgeteilt sein
            var navTaskInfo = classSymbol?.DeclaringSyntaxReferences
                                          .Select(dsr => dsr.GetSyntax() as ClassDeclarationSyntax)
                                          .Select(GetNavTaskInfo).FirstOrDefault(nti => nti != null);
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskInfo GetNavTaskInfo(ClassDeclarationSyntax classDeclaration) {

            var tags = ReadNavTags(classDeclaration).ToList();

            var navFileTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavFile);
            var navFileName = navFileTag?.Content;

            var navTaskTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTask);
            var navTaskName = navTaskTag?.Content;

            // Dateiname und Taskname müssen immer paarweise vorhanden sein
            if (String.IsNullOrWhiteSpace(navFileName) || String.IsNullOrWhiteSpace(navTaskName)) {
                return null;
            }

            // Relative Pfadangaben in absolute auflösen
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
        static NavTriggerInfo GetNavTriggerInfo(IMethodSymbol method, NavTaskInfo taskInfo) {

            var navTriggerInfo = method?.DeclaringSyntaxReferences
                                        .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                        .Select(syntax => GetNavTriggerInfo(syntax, taskInfo))
                                        .FirstOrDefault(nti => nti != null);

            return navTriggerInfo;
        }

        [CanBeNull]
        static NavTriggerInfo GetNavTriggerInfo(MethodDeclarationSyntax methodDeclaration, NavTaskInfo taskInfo) {

            var tags = ReadNavTags(methodDeclaration).ToList();

            var navTriggerTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTrigger);
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
        static IEnumerable<NavTag> ReadNavTags(Microsoft.CodeAnalysis.SyntaxNode node) {

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

                // Wir suchen alle Tags, deren Namen mit Nav beginnen
                foreach (var xmlElementSyntax in xmlElementSyntaxes) {
                    var startTagName = xmlElementSyntax.StartTag?.Name?.ToString();
                    if (startTagName?.StartsWith(AnnotationTagNames.TagPrefix) == true) {
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

        sealed class BuildTagsResult {

            public IList<ITagSpan<GoToNavTag>> Tags { get; }
            public ITextSnapshot Snapshot { get; }

            public BuildTagsResult(IList<ITagSpan<GoToNavTag>> tags, ITextSnapshot snapshot) {
                Tags = tags;
                Snapshot = snapshot;
            }
        }
    }
}