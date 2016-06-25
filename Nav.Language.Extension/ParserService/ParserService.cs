﻿#region Using Directives 

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

    public delegate SyntaxNode ParseMethod(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken));

    sealed class ParserService: IDisposable {

        static readonly Logger Logger = Logger.Create<ParserService>();

        static readonly object ParseMethodKey = new Object();
        readonly IDisposable _parserObs;
        ParseResult _parseResult;
        bool _waitingForAnalysis;

        ParserService(ITextBuffer textBuffer) {
            TextBuffer = textBuffer;

            WeakEventDispatcher.Connect(this, textBuffer);

            _parserObs = Observable.FromEventPattern<EventArgs>(
                                               handler => RebuildTriggered += handler,
                                               handler => RebuildTriggered -= handler)
                                   .Select( _ => CreateBuildResultArgs())
                                   .Throttle(ServiceProperties.ParserServiceThrottleTime)
                                   .Select( args => Observable.DeferAsync(async token => 
                                       {
                                           var parseResult = await BuildResultAsync(args, token).ConfigureAwait(false);

                                           return Observable.Return(parseResult);
                                       }))
                                   .Switch()                                 
                                   .ObserveOn(SynchronizationContext.Current)
                                   .Subscribe(TrySetResult);

            _waitingForAnalysis = true;
            // Initiales Parsen antriggern
            Invalidate();
        }
        
        public void Dispose() {
            _parserObs.Dispose();
        }

        public event EventHandler<EventArgs> ParseResultChanging;
        public event EventHandler<SnapshotSpanEventArgs> ParseResultChanged;
        // Dieses Event feuern wir um den Observer zu "füttern".
        event EventHandler<EventArgs> RebuildTriggered;

        [NotNull]
        public ITextBuffer TextBuffer { get; }

        public bool WaitingForAnalysis {
            get { return _waitingForAnalysis; }
        }

        [CanBeNull]
        public ParseResult ParseResult {
            get { return _parseResult; }           
        }
        
        public static ParseMethod GetParseMethod(ITextBuffer textBuffer) {
            ParseMethod parseMethod;
            textBuffer.Properties.TryGetProperty(ParseMethodKey, out parseMethod);
            return parseMethod ?? Syntax.ParseCodeGenerationUnit;
        }

        public static TextBufferScopedValue<ParserService> GetOrCreateSingelton(ITextBuffer textBuffer) {
            return TextBufferScopedValue<ParserService>.GetOrCreate(
                textBuffer, 
                typeof(ParserService), 
                () => new ParserService(textBuffer));
        }

        public static ParserService TryGet(ITextBuffer textBuffer) {
            return TextBufferScopedValue<ParserService>.TryGet(textBuffer, typeof(ParserService));
        }
        
        public static void SetParseMethod(ITextBuffer textBuffer, ParseMethod parseMethod) {
            textBuffer.Properties.AddProperty(ParseMethodKey, parseMethod);
        }

        public void Invalidate() {
            OnParseResultChanging(EventArgs.Empty);
            OnRebuildTriggered(EventArgs.Empty);
        }

        void OnRebuildTriggered(EventArgs e) {
            RebuildTriggered?.Invoke(this, e);
        }

        void OnParseResultChanging(EventArgs e) {
            _waitingForAnalysis = true;
            ParseResultChanging?.Invoke(this, e);
        }

        void OnParseResultChanged(SnapshotSpanEventArgs e) {
            _waitingForAnalysis = false;
            ParseResultChanged?.Invoke(this, e);
        }

        struct BuildResultArgs {
            public ITextSnapshot Snapshot { get; set; }
            public string Text { get; set; }
            public string FilePath { get; set; }
            public ParseMethod ParseMethod { get; set; }
        }

        /// <summary>
        /// Diese Methode muss im GUI Thread aufgerufen werden!
        /// </summary>
        BuildResultArgs CreateBuildResultArgs() {
            var args = new BuildResultArgs {
                Snapshot    = TextBuffer.CurrentSnapshot,
                Text        = TextBuffer.CurrentSnapshot.GetText(),
                FilePath    = TextBuffer.GetTextDocument()?.FilePath,
                ParseMethod = GetParseMethod(TextBuffer)
            };

            return args;
        }

        /// <summary>
        /// Achtung: Diese Methode wird bereits in einem Background Thread aufgerufen. Also vorischt bzgl. thread safety!
        /// Deshalb werden die BuildResultArgs bereits vorab im GUI Thread erstellt.
        /// </summary>
        static async Task<ParseResult> BuildResultAsync(BuildResultArgs args, CancellationToken cancellationToken) {
            
            return await Task.Run(() => {

                using(Logger.LogBlock(nameof(BuildResultAsync))) {

                    var syntaxTree = args.ParseMethod(args.Text, args.FilePath, cancellationToken).SyntaxTree;

                    return new ParseResult(syntaxTree, args.Snapshot);
                }

            }, cancellationToken).ConfigureAwait(false);            
        }
        
        void TrySetResult(ParseResult parseResult) {

            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (TextBuffer.CurrentSnapshot != parseResult.Snapshot) {
                if (!WaitingForAnalysis) {
                    // Dieser Fall sollte eigentlich nicht eintreten, denn es muss bereits eine neue Berechnung angetriggert worden sein
                    Invalidate();
                }
                return;
            }

            _parseResult = parseResult;

            var snapshotSpan = parseResult.Snapshot.GetFullSpan();
            OnParseResultChanged(new SnapshotSpanEventArgs(snapshotSpan));
        }
        
        // Irgend jemand scheint den ITextBuffer länger als erhofft im Speicher zu halten
        // Damit der Parserservice nicht genauso lange im Speicher verbleibt, verknüpfen wir
        // hier die Events "weak".
        sealed class WeakEventDispatcher {
            readonly WeakReference _target;

            WeakEventDispatcher(ParserService service) {
                _target = new WeakReference(service);
            }

            public static void Connect(ParserService service, ITextBuffer textBuffer) {
                var dispatcher=new WeakEventDispatcher(service);
                textBuffer.Changed += dispatcher.OnTextBufferChanged;
            }

            void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) {
                var textBuffer = (ITextBuffer) sender;
                ParserService target =_target.Target as ParserService;
                if (null != target) {
                    target.Invalidate();
                } else {
                    textBuffer.Changed -= OnTextBufferChanged;
                }
            }
        }
    }
}