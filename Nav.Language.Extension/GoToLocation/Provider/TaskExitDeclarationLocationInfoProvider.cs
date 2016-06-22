#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskExitDeclarationLocationInfoProvider : CodeAnalysisLocationInfoProvider {

        readonly TaskExitCodeGenInfo _codegenInfo;

        public TaskExitDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskExitCodeGenInfo codegenInfo): base(sourceBuffer) {
            _codegenInfo  = codegenInfo;
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

           var location = await LocationFinder.FindTaskExitDeclarationLocationAsync(project, _codegenInfo, cancellationToken)
                                              .ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}