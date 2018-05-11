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
                                          var codeGenerationUnitAndSnapshot = await BuildAsync(ParserService.SyntaxTreeAndSnapshot, token).ConfigureAwait(false);

                                          return Observable.Return(codeGenerationUnitAndSnapshot);
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
        
        public static SemanticModelService GetOrCreateSingelton(ITextBuffer textBuffer) {
            return textBuffer.Properties.GetOrCreateSingletonProperty (
                typeof(SemanticModelService),
                () => new SemanticModelService(textBuffer));
        }
        
        [CanBeNull]
        public static SemanticModelService TryGet(ITextBuffer textBuffer) {
            textBuffer.Properties.TryGetProperty<SemanticModelService>(typeof(SemanticModelService), out var semanticModelService);
            return semanticModelService;
        }

        public void Invalidate() {
            OnSemanticModelChanging(EventArgs.Empty);
            OnRebuildTriggered(EventArgs.Empty);
        }

        public CodeGenerationUnitAndSnapshot UpdateSynchronously(CancellationToken cancellationToken = default) {

            var codeGenerationUnitAndSnapshot = CodeGenerationUnitAndSnapshot;
            if(codeGenerationUnitAndSnapshot != null && codeGenerationUnitAndSnapshot.IsCurrent(TextBuffer)) {
                return codeGenerationUnitAndSnapshot;
            }

            var syntaxTreeAndSnapshot=ParserService.UpdateSynchronously(cancellationToken);

            codeGenerationUnitAndSnapshot = BuildSynchronously(syntaxTreeAndSnapshot, cancellationToken);
            TrySetResult(codeGenerationUnitAndSnapshot);

            return codeGenerationUnitAndSnapshot;
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

        static async Task<CodeGenerationUnitAndSnapshot> BuildAsync(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, CancellationToken cancellationToken) {
            return await Task.Run(() => {
                using(Logger.LogBlock(nameof(BuildAsync))) {
                    return BuildSynchronously(syntaxTreeAndSnapshot, cancellationToken);
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        static CodeGenerationUnitAndSnapshot BuildSynchronously(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, CancellationToken cancellationToken) {

            if(syntaxTreeAndSnapshot == null) {
                Logger.Debug("Es gibt kein ParesResult. Der Vorgang wird abgebrochen.");
                return null;
            }

            var syntaxTree = syntaxTreeAndSnapshot.SyntaxTree;
            var snapshot   = syntaxTreeAndSnapshot.Snapshot;

            if(!(syntaxTree.Root is CodeGenerationUnitSyntax codeGenerationUnitSyntax)) {
                Logger.Debug($"Der SyntaxRoot ist nicht vom Typ {typeof(CodeGenerationUnitSyntax)}. Der Vorgang wird abgebrochen.");
                return null;
            }

            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

            return new CodeGenerationUnitAndSnapshot(codeGenerationUnit, snapshot);
        }

        void TrySetResult(CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            // Dieser Fall kann eintreten, da wir im Ctor "blind" ein Invalidate aufrufen. Möglicherweise gibt es aber noch kein SyntaxTreeAndSnapshot,
            // welches aber noch folgen wird und im Zuge eines OnParseResultChanging abgerbeitet wird.
            if(codeGenerationUnitAndSnapshot == null) {
                return;
            }
            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (!codeGenerationUnitAndSnapshot.IsCurrent(TextBuffer)) {
                return;
            }

            _codeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot;

            OnSemanticModelChanged(new SnapshotSpanEventArgs(codeGenerationUnitAndSnapshot.Snapshot.GetFullSpan()));
        }        
    }
}