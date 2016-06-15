#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTagger: ITagger<IntraTextGoToTag>, IDisposable {

        readonly ITextBuffer _textBuffer;
        readonly IDisposable _parserObs;
        readonly WorkspaceRegistration _workspaceRegistration;

        BuildTagsResult _result;
        Workspace _workspace;

        public IntraTextGoToTagger(ITextBuffer textBuffer) {

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

        public IEnumerable<ITagSpan<IntraTextGoToTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            if (_result == null || spans.Count == 0) {
                yield break;
            }

            foreach (var span in spans) {
                foreach (var tag in _result.Tags) {

                    var transSpan = tag.Span.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive);
                    if (transSpan.IntersectsWith(span)) {
                        yield return new TagSpan<IntraTextGoToTag>(transSpan, tag.Tag);
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

            // TODO hier könnte man noch mit dem aktuellem Ergebnis Vergleichen und ggf. gar nichts machen, wenn gleich...
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
        /// </summary>
        static async Task<BuildTagsResult> BuildTagsAsync(ITextSnapshot snapshot, CancellationToken cancellationToken) {

            return await Task.Run(() => {

                var tags = BuildTags(snapshot).ToList();

                return new BuildTagsResult(tags, snapshot);

            }, cancellationToken).ConfigureAwait(false);
        }

        static IEnumerable<ITagSpan<IntraTextGoToTag>> BuildTags(ITextSnapshot currentSnapshot) {

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
                var navTaskInfo = ReadNavTaskAnnotation(classSymbol);

                // In der Basisklasse nachsehen
                if (navTaskInfo == null) {
                    navTaskInfo = ReadNavTaskAnnotation(classSymbol?.BaseType);
                }

                if (navTaskInfo == null) {
                    continue;
                }

                var start = classDeclaration.Identifier.Span.Start;
                var length = classDeclaration.Identifier.Span.Length;

                var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                yield return new TagSpan<IntraTextGoToTag>(snapshotSpan, new GoToNavTag(navTaskInfo));

                var methodDeclarations = classDeclaration.DescendantNodes()
                                                         .OfType<MethodDeclarationSyntax>();

                foreach (var methodDeclaration in methodDeclarations) {
                    var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

                    var initTag = TryGetNavInitTagSpan(currentSnapshot, methodSymbol, navTaskInfo, methodDeclaration);
                    if (initTag != null) {
                        yield return initTag;
                    }

                    var triggerTag = TryGetNavTriggerTagSpan(currentSnapshot, methodSymbol, navTaskInfo, methodDeclaration);
                    if (triggerTag != null) {
                        yield return triggerTag;
                    }                    
                }

                var invocationExpressions = classDeclaration.DescendantNodes()
                                                            .OfType<InvocationExpressionSyntax>();

                foreach(var invocationExpression in invocationExpressions) {

                    var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression.Expression).Symbol as IMethodSymbol;
                    if(methodSymbol == null) {
                        continue;
                    }

                    var declaringMethodNode = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
                    // TODO hier das richtige Tag auslesen
                    var navInitCallTag = ReadNavTags(declaringMethodNode).FirstOrDefault(tag=>tag.TagName == AnnotationTagNames.NavInitCall);
                    if(navInitCallTag == null) {
                        continue;
                    }
                    
                    var identifier = invocationExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                    if (identifier == null) {
                        continue;
                    }

                    start  = identifier.Span.Start;
                    length = identifier.Identifier.Span.Length;

                    snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                    var beginItfFullyQualifiedName = navInitCallTag.Content;

                    yield return new TagSpan<IntraTextGoToTag>(snapshotSpan, 
                                        new GoToBeginLogic(
                                            currentSnapshot.TextBuffer, 
                                            beginItfFullyQualifiedName, 
                                            methodSymbol.Parameters));
                }
            }
        }
        

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotation(INamedTypeSymbol classSymbol) {

            // Die Klasse kann in mehrere partial classes aufgeteilt sein
            var navTaskInfo = classSymbol?.DeclaringSyntaxReferences
                                          .Select(dsr => dsr.GetSyntax() as ClassDeclarationSyntax)
                                          .Select(ReadNavTaskAnnotation).FirstOrDefault(nti => nti != null);
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotation(ClassDeclarationSyntax classDeclaration) {

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

            var candidate = new NavTaskAnnotation {
                NavFileName = navFileName,
                TaskName    = navTaskName
            };

            return candidate;
        }

        [CanBeNull]
        static ITagSpan<IntraTextGoToTag> TryGetNavTriggerTagSpan(ITextSnapshot currentSnapshot, IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation, MethodDeclarationSyntax methodDeclaration) {

            var triggerInfo = ReadNavTriggerAnnotation(methodSymbol, navTaskAnnotation);
            // In der überschriebenen Methode nachsehen
            if (triggerInfo == null) {
                triggerInfo = ReadNavTriggerAnnotation(methodSymbol?.OverriddenMethod, navTaskAnnotation);
            }

            if (triggerInfo == null) {
                return null;
            }

            int start = methodDeclaration.Identifier.Span.Start;
            int length = methodDeclaration.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, new GoToNavTag(triggerInfo));
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotation(IMethodSymbol method, NavTaskAnnotation taskAnnotation) {

            var navTriggerInfo = method?.DeclaringSyntaxReferences
                                        .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                        .Select(syntax => ReadNavTriggerAnnotation(syntax, taskAnnotation))
                                        .FirstOrDefault(nti => nti != null);

            return navTriggerInfo;
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotation(MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation taskAnnotation) {

            var tags = ReadNavTags(methodDeclaration).ToList();

            var navTriggerTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTrigger);
            var navTriggerName = navTriggerTag?.Content;

            if(String.IsNullOrEmpty(navTriggerName)) {                
                return null;
            }

            return new NavTriggerAnnotation {
                NavFileName = taskAnnotation.NavFileName,
                TaskName    = taskAnnotation.TaskName,
                TriggerName = navTriggerName
            };
        }

        [CanBeNull]
        static ITagSpan<IntraTextGoToTag> TryGetNavInitTagSpan(ITextSnapshot currentSnapshot, IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation, MethodDeclarationSyntax methodDeclaration) {

            var navInitInfo = ReadNavInitAnnotation(methodSymbol, navTaskAnnotation);
            // In der überschriebenen Methode nachsehen
            if (navInitInfo == null) {
                navInitInfo = ReadNavInitAnnotation(methodSymbol?.OverriddenMethod, navTaskAnnotation);
            }

            if (navInitInfo == null) {
                return null;
            }
            // TODO Diesen Teil Konsolodieren
            int start = methodDeclaration.Identifier.Span.Start;
            int length = methodDeclaration.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, new GoToNavTag(navInitInfo));
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotation(IMethodSymbol method, NavTaskAnnotation taskAnnotation) {

            var navInitInfo = method?.DeclaringSyntaxReferences
                                     .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                     .Select(syntax => ReadNavInitAnnotation(syntax, taskAnnotation))
                                     .FirstOrDefault(nti => nti != null);

            return navInitInfo;
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotation(MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation taskAnnotation) {

            var tags = ReadNavTags(methodDeclaration).ToList();

            var navInitTag = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavInit);
            var navInitName = navInitTag?.Content;

            if (String.IsNullOrEmpty(navInitName)) {
                return null;
            }

            return new NavInitAnnotation {
                NavFileName = taskAnnotation.NavFileName,
                TaskName = taskAnnotation.TaskName,
                InitName = navInitName
            };
        }

        [NotNull]
        static IEnumerable<NavTag> ReadNavTags(Microsoft.CodeAnalysis.SyntaxNode node) {

            if(node == null) {
                yield break;
            }

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

            public IList<ITagSpan<IntraTextGoToTag>> Tags { get; }
            public ITextSnapshot Snapshot { get; }

            public BuildTagsResult(IList<ITagSpan<IntraTextGoToTag>> tags, ITextSnapshot snapshot) {
                Tags = tags;
                Snapshot = snapshot;
            }
        }
    }
}