#region Using Directives

using System;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    abstract class ParserServiceDependent: IDisposable {
        
        readonly TextBufferScopedValue<ParserService> _parserServiceSingelton;

        protected ParserServiceDependent(ITextBuffer textBuffer) {

            TextBuffer = textBuffer;

            _parserServiceSingelton = ParserService.GetOrCreateSingelton(textBuffer);

            ParserService.ParseResultChanging += OnParseResultChanging;
            ParserService.ParseResultChanged  += OnParseResultChanged;
        }

        public virtual void Dispose() {

            ParserService.ParseResultChanging -= OnParseResultChanging;
            ParserService.ParseResultChanged  -= OnParseResultChanged;

            _parserServiceSingelton.Dispose();
        }

        protected ITextBuffer TextBuffer { get; }

        protected ParserService ParserService {
            get { return _parserServiceSingelton.Value; }
        }

        protected virtual void OnParseResultChanging(object sender, EventArgs e) {
        }

        protected virtual void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
        }        
    }
}
