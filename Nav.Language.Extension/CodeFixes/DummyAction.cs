#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(DummyActionProvider))]
    class DummyActionProvider : ICodeFixActionProvider {

        public IEnumerable<ISuggestedAction> GetSuggestedActions(SnapshotSpan range, IEnumerable<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, CancellationToken cancellationToken) {
            foreach (var symbol in symbols) {
                yield return new DummyAction(symbol.Name);
            }
        }
    }

    class DummyAction: ISuggestedAction {
        private readonly string _symbolName;

        public DummyAction(string symbolName) {
            _symbolName = symbolName;
        }

        public void Dispose() {
            
        }

        public bool TryGetTelemetryId(out Guid telemetryId) {
            telemetryId = Guid.Empty;
            return false;
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken) {
            return null;
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken) {
            return null;
        }

        public void Invoke(CancellationToken cancellationToken) {
            ShellUtil.ShowInfoMessage("TODO");
        }

        public bool HasActionSets {
            get { return false; }
        }

        public string DisplayText {
            get { return $"Remove unused {_symbolName}"; }
        }

        public ImageMoniker IconMoniker {
            get { return default(ImageMoniker); }
        }

        public string IconAutomationText {
            get { return null; }
        }

        public string InputGestureText {
            get { return null; }
        }
        public bool HasPreview {
            get { return false; }
        }
    }
}
