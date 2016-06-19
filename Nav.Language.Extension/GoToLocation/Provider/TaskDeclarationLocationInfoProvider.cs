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

    class TaskDeclarationLocationInfoProvider: LocationInfoProvider {

        readonly ITextBuffer _sourceBuffer;
        readonly TaskCodeGenInfo _codegenInfo;

        public TaskDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskCodeGenInfo codegenInfo) {

            _sourceBuffer = sourceBuffer;
            _codegenInfo  = codegenInfo;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError($"Das Projekt konnte nicht ermittelt werden."));
            }

            var locations = await LocationFinder.FindTaskDeclarationLocationsAsync(project, _codegenInfo, cancellationToken)
                                               .ConfigureAwait(false);

            return locations;
        }
    }
}
