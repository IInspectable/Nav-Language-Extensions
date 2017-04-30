#region Using Directives

using System;
using System.Threading;
using System.Reactive.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    sealed class SemanticModelService: ParserServiceDependent {

        static readonly Logger Logger = Logger.Create<SemanticModelService>();

        readonly IDisposable _observable;
        CodeGenerationUnitAndSnapshot _codeGenerationUnitAndSnapshot;
        bool _waitingForAnalysis;

        public SemanticModelService(ITextBuffer textBuffer): base(textBuffer) {

            _observable = Observable.FromEventPattern<EventArgs>(
                                              handler => RebuildTriggered += handler,
                                              handler => RebuildTriggered -= handler)
                                  .Throttle(ServiceProperties.SemanticModelServiceThrottleTime)
                                  .Select(_ => Observable.DeferAsync(async token =>
                                      {
                                          var parseResult = await BuildResultAsync(ParserService.SyntaxTreeAndSnapshot, token).ConfigureAwait(false);

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
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot {
            get { return _codeGenerationUnitAndSnapshot; }
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

         static async Task<CodeGenerationUnitAndSnapshot> BuildResultAsync(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, CancellationToken cancellationToken) {
            return await Task.Run(() => {

                using(Logger.LogBlock(nameof(BuildResultAsync))) {

                    if(syntaxTreeAndSnapshot == null) {
                        Logger.Debug("Es gibt kein ParesResult. Der Vorgang wird abgebrochen.");
                        return null;
                    }

                    var syntaxTree = syntaxTreeAndSnapshot.SyntaxTree;
                    var snapshot = syntaxTreeAndSnapshot.Snapshot;

                    var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                    if(codeGenerationUnitSyntax == null) {
                        Logger.Debug($"Der SyntaxRoot ist nicht vom Typ {typeof(CodeGenerationUnitSyntax)}. Der Vorgang wird abgebrochen.");
                        return null;
                    }

                    var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                    return new CodeGenerationUnitAndSnapshot(codeGenerationUnit, snapshot);
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        void TrySetResult(CodeGenerationUnitAndSnapshot result) {

            // Dieser Fall kann eintreten, da wir im Ctor "blind" ein Invalidate aufrufen. Möglicherweise gibt es aber noch kein SyntaxTreeAndSnapshot,
            // welches aber noch folgen wird und im Zuge eines OnParseResultChanging abgerbeitet wird.
            if(result == null) {
                return;
            }
            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (TextBuffer.CurrentSnapshot != result.Snapshot) {
                if (!WaitingForAnalysis) {
                    // Dieser Fall sollte eigentlich nicht eintreten, denn es muss bereits eine neue Analyse angetriggert worden sein
                    Invalidate();
                }
                return;
            }

            _codeGenerationUnitAndSnapshot = result;

            OnSemanticModelChanged(new SnapshotSpanEventArgs(result.Snapshot.GetFullSpan()));
        }        
    }
}