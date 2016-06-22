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

    class TaskDeclarationLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly TaskCodeGenInfo _codegenInfo;

        public TaskDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskCodeGenInfo codegenInfo): base(sourceBuffer) {

            _codegenInfo  = codegenInfo;
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            var locations = await LocationFinder.FindTaskDeclarationLocationsAsync(project, _codegenInfo, cancellationToken)
                                               .ConfigureAwait(false);

            return locations;
        }
    }
}