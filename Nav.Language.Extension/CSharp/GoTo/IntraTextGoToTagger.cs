#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.Extension.Notification;

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
                                   .Select(_ => new BuildTagsArgs {
                                                    DocumentId = GetDocumentId(),
                                                    Snapshot   = _textBuffer.CurrentSnapshot })
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
            // TODO Fehlt uns irgendein Event? Es scheint manchmal vorzukommen, dass die Tags nach dem Starten von VS nicht verfügbar sind...
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
                var openDocumentId = GetDocumentId();
                if (openDocumentId == documentId) {
                    Invalidate();
                }
            }
        }

        DocumentId GetDocumentId() {
            var openDocumentId = _workspace.GetDocumentIdInCurrentContext(_textBuffer.AsTextContainer());
            return openDocumentId;
        }

        /// <summary>
        /// TrySetResult wird im GUI-Context aufgerufen
        /// </summary>
        void TrySetResult(BuildTagsResult result) {

            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (_textBuffer.CurrentSnapshot != result.BuildArgs.Snapshot) {   
                return;
            }

            var prevResult = _result;

            // Das neue Ergebnis wird schon alleine wegen der aktuelleren SnapshotSpans gespeichert
            _result = result;

            if (ShouldRaiseTagsChanged(prevResult, _result)) {

                var snapshotSpan = result.BuildArgs.Snapshot.GetFullSpan();
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));

                foreach(var navTaskAnnotation in result.Annotations
                                                       .Where(a => a.GetType() == typeof(NavTaskAnnotation))) {

                    var args = new NavTaskAnnotationChangedArgs {
                        DocumentId     = result.BuildArgs.DocumentId,
                        TaskAnnotation = navTaskAnnotation
                    };
                    NotificationService.RaiseChanged(args);
                }
            }
        }

        static bool ShouldRaiseTagsChanged(BuildTagsResult prevResult, BuildTagsResult newResult) {

            // Der triviale, aber Standardfall für alle "nicht-WFS" files.
            if (prevResult?.Tags.Count == 0 && newResult.Tags.Count ==0) {
                return false;
            }

            // Hier könnte man theoretisch noch weiter in die Tags schauen, ob sich diese de facto geändert haben,
            // um so unnötig UI-Updates zu vermeiden. Wie groß ist der Benefit, wie groß das "Risiko" echte Updates
            // zu verlieren?

            return true;
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
        static Task<BuildTagsResult> BuildTagsAsync(BuildTagsArgs args, CancellationToken cancellationToken) {

            return Task.Run(() => {

                var result = BuildTags(args);

                return result;

            }, cancellationToken);
        }

        static BuildTagsResult BuildTags(BuildTagsArgs buildArgs) {

            var document = buildArgs.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if(document == null) {
                // TODO Wann kommt das vor? Müssen wir darauf mit einer nachgeschobenen Berechnung reagieren?
                return new BuildTagsResult(buildArgs);
            }
            
            var annotations = AnnotationReader.ReadNavTaskAnnotations(document)
                                              .ToList();
            var tagSpanBuilder = new IntraTextGoToTagSpanBuilder(buildArgs.Snapshot);

            var tags = annotations.Select(annotation => tagSpanBuilder.Visit(annotation))
                                  .Where(tagsSpan => tagsSpan != null)
                                  .ToList();

            return new BuildTagsResult(buildArgs, tags, annotations);

        }

        struct BuildTagsArgs {

            public ITextSnapshot Snapshot { get; set; }
            public DocumentId DocumentId { get; set; }

        }

        sealed class BuildTagsResult {

            public BuildTagsResult(BuildTagsArgs buildArgs) {
                BuildArgs   = buildArgs;
                Annotations = new List<NavTaskAnnotation>();
                Tags        = new List<ITagSpan<IntraTextGoToTag>>();
            }

            public BuildTagsResult(BuildTagsArgs buildArgs, IList<ITagSpan<IntraTextGoToTag>> tags, IList<NavTaskAnnotation> annotations) {
                BuildArgs   = buildArgs;
                Tags        = tags;
                Annotations = annotations;
            }

            public BuildTagsArgs BuildArgs { get; }
            public IList<ITagSpan<IntraTextGoToTag>> Tags { get; }
            public IList<NavTaskAnnotation> Annotations { get; }
        }
    }
}