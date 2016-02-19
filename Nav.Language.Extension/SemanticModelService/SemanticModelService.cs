#region Using Directives

using System;
using System.Threading;
using System.Reactive.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    sealed class SemanticModelService: ParserServiceDependent {

        readonly IDisposable _observable;
        SemanticModelResult _semanticModelResult;
        bool _waitingForAnalysis;

        public SemanticModelService(ITextBuffer textBuffer): base(textBuffer) {

            _observable = Observable.FromEventPattern<EventArgs>(
                                              handler => RebuildTriggered += handler,
                                              handler => RebuildTriggered -= handler)
                                  .Throttle(ServiceProperties.SemanticModelServiceThrottleTime)
                                  .Select(_ => Observable.DeferAsync(async token =>
                                      {
                                          var parseResult = await BuildResultAsync(ParserService.ParseResult, token).ConfigureAwait(false);

                                          return Observable.Return(parseResult);
                                      }))
                                  .Switch()
                                  .ObserveOn(SynchronizationContext.Current)
                                  .Subscribe(TrySetResult);

            _waitingForAnalysis = true;
        }

        public override void Dispose() {
            base.Dispose();
            _observable?.Dispose();
        }

        public event EventHandler<EventArgs> SemanticModelChanging;
        public event EventHandler<SnapshotSpanEventArgs> SemanticModelChanged;
        // Dieses Event feuern wir u.a. um den Observer zu "füttern".
        event EventHandler<EventArgs> RebuildTriggered;

        public bool WaitingForAnalysis {
            get { return _waitingForAnalysis; }
        }

        [CanBeNull]
        public SemanticModelResult SemanticModelResult {
            get { return _semanticModelResult; }
        }
        
        public static TextBufferScopedValue<SemanticModelService> GetOrCreateSingelton(ITextBuffer textBuffer) {
            return TextBufferScopedValue<SemanticModelService>.GetOrCreate(
                textBuffer,
                typeof(SemanticModelService),
                () => new SemanticModelService(textBuffer));
        }
        
        public static SemanticModelService TryGet(ITextBuffer textBuffer) {
            return TextBufferScopedValue<SemanticModelService>.TryGet(textBuffer, typeof(SemanticModelService));
        }

        public void Invalidate() {
            OnSemanticModelChanging(EventArgs.Empty);
            OnRebuildTriggered(EventArgs.Empty);
        }
        
        protected override void OnParseResultChanging(object sender, EventArgs e) {
            OnSemanticModelChanging(EventArgs.Empty);
        }

        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            Invalidate();
        }
        
        void OnRebuildTriggered(EventArgs e) {
            RebuildTriggered?.Invoke(this, e);
        }

        void OnSemanticModelChanging(EventArgs e) {
            _waitingForAnalysis = true;
            SemanticModelChanging?.Invoke(this, e);
        }

        void OnSemanticModelChanged(SnapshotSpanEventArgs e) {
            _waitingForAnalysis = false;
            SemanticModelChanged?.Invoke(this, e);
        }

         static async Task<SemanticModelResult> BuildResultAsync(ParseResult parseResult, CancellationToken cancellationToken) {
            return await Task.Run(() => {
                
                if(parseResult == null) {
                    return null;
                }

                var syntaxTree = parseResult.SyntaxTree;
                var snapshot   = parseResult.Snapshot;

                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if(codeGenerationUnitSyntax == null) {
                    return null;
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                return new SemanticModelResult(codeGenerationUnit, snapshot);

            }, cancellationToken).ConfigureAwait(false);
        }

        void TrySetResult(SemanticModelResult result) {

            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (TextBuffer.CurrentSnapshot != result.Snapshot) {
                if (!WaitingForAnalysis) {
                    // Dieser Fall sollte eigentlich nicht eintreten, denn es muss bereits eine neue Analyse angetriggert worden sein
                    Invalidate();
                }
                return;
            }

            _semanticModelResult = result;

            OnSemanticModelChanged(new SnapshotSpanEventArgs(result.Snapshot.ToSnapshotSpan()));
        }        
    }
}