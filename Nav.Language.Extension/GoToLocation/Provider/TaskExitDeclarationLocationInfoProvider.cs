#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskExitDeclarationLocationInfoProvider : LocationInfoProvider {

        readonly ITextBuffer _sourceBuffer;
        readonly TaskExitCodeGenInfo _codegenInfo;

        public TaskExitDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskExitCodeGenInfo codegenInfo) {

            _sourceBuffer = sourceBuffer;
            _codegenInfo  = codegenInfo;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError(""));
            }

            var location = await LocationFinder.FindTaskExitDeclarationLocationAsync(project, _codegenInfo, cancellationToken)
                                               .ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}