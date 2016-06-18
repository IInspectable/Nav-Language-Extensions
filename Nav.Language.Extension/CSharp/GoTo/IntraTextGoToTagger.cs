#region Using Directives

using System;
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
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

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
        static Task<BuildTagsResult> BuildTagsAsync(ITextSnapshot snapshot, CancellationToken cancellationToken) {

            return Task.Run(() => {

                var tags = BuildTags(snapshot).ToList();

                return new BuildTagsResult(tags, snapshot);

            }, cancellationToken);
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

            // TODO Diese ganze Gaudi hätte ich gerne von den "Tags" entkoppelt, und in den AnnotationReader gelegt
            foreach (var classDeclaration in classDeclarations) {

                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                var navTaskInfo = AnnotationReader.ReadNavTaskAnnotation(classSymbol);

                if (navTaskInfo == null) {
                    continue;
                }

                var start = classDeclaration.Identifier.Span.Start;
                var length = classDeclaration.Identifier.Span.Length;

                var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

                yield return new TagSpan<IntraTextGoToTag>(snapshotSpan, new GoToNavTaskAnnotationTag(navTaskInfo));

                var methodDeclarations = classDeclaration.DescendantNodes()
                                                         .OfType<MethodDeclarationSyntax>();

                foreach (var methodDeclaration in methodDeclarations) {
                    var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

                    var initTag = TryGetNavInitTagSpan(currentSnapshot, methodSymbol, navTaskInfo, methodDeclaration);
                    if (initTag != null) {
                        yield return initTag;
                    }

                    var exitTag = TryGetNavExitTagSpan(currentSnapshot, methodSymbol, navTaskInfo, methodDeclaration);
                    if (exitTag != null) {
                        yield return exitTag;
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
                    var navInitCallTag = AnnotationReader.ReadNavTags(declaringMethodNode).FirstOrDefault(tag=>tag.TagName == AnnotationTagNames.NavInitCall);
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

                    var provider = new BeginLogicLocationInfoProvider(
                                            currentSnapshot.TextBuffer,
                                            beginItfFullyQualifiedName,
                                            methodSymbol.Parameters);

                    yield return new TagSpan<IntraTextGoToTag>(snapshotSpan, 
                                        new IntraTextGoToTag(
                                            provider    : provider, 
                                            imageMoniker: GoToImageMonikers.GoToBeginLogicCallDeclaration, 
                                            // TODO Tooltip Text zentralisieren
                                            toolTip     : "Go To Begin Logic"));
                }
            }
        }


        [CanBeNull]
        static ITagSpan<IntraTextGoToTag> TryGetNavTriggerTagSpan(ITextSnapshot currentSnapshot, IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation, MethodDeclarationSyntax methodDeclaration) {

            var triggerAnnotation = AnnotationReader.ReadNavTriggerAnnotation(methodSymbol, navTaskAnnotation);

            if (triggerAnnotation == null) {
                return null;
            }

            return CreateTagSpan(currentSnapshot, methodDeclaration, triggerAnnotation);
        }
        
        [CanBeNull]
        static ITagSpan<IntraTextGoToTag> TryGetNavInitTagSpan(ITextSnapshot currentSnapshot, IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation, MethodDeclarationSyntax methodDeclaration) {

            var initAnnotation = AnnotationReader.ReadNavInitAnnotation(methodSymbol, navTaskAnnotation);

            if (initAnnotation == null) {
                return null;
            }
            return CreateTagSpan(currentSnapshot, methodDeclaration, initAnnotation);
        }

        [CanBeNull]
        static ITagSpan<IntraTextGoToTag> TryGetNavExitTagSpan(ITextSnapshot currentSnapshot, IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation, MethodDeclarationSyntax methodDeclaration) {

            var navExitAnnotation= AnnotationReader.ReadNavExitAnnotation(methodSymbol, navTaskAnnotation);
            if(navExitAnnotation == null) {
                return null;
            }
            return CreateTagSpan(currentSnapshot, methodDeclaration, navExitAnnotation);
        }
        
        static ITagSpan<IntraTextGoToTag> CreateTagSpan(ITextSnapshot currentSnapshot, MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation navTaskAnnotation) {

            int start  = methodDeclaration.Identifier.Span.Start;
            int length = methodDeclaration.Identifier.Span.Length;

            var snapshotSpan = new SnapshotSpan(currentSnapshot, start, length);

            return new TagSpan<IntraTextGoToTag>(snapshotSpan, new GoToNavTaskAnnotationTag(navTaskAnnotation));
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