#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TriggerDeclarationLocationInfoProvider: LocationInfoProvider {

        readonly ITextBuffer _sourceBuffer;
        readonly SignalTriggerCodeGenInfo _codegenInfo;

        public TriggerDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, SignalTriggerCodeGenInfo codegenInfo) {

            _sourceBuffer = sourceBuffer;
            _codegenInfo  = codegenInfo;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError(""));
            }

            var location = await LocationFinder.FindTriggerDeclarationLocationsAsync(project, _codegenInfo, cancellationToken)
                                               .ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}
