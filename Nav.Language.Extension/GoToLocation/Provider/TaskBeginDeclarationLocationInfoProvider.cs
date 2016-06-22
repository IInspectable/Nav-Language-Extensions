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

    class TaskBeginDeclarationLocationInfoProvider : CodeAnalysisLocationInfoProvider {

        readonly TaskBeginCodeGenInfo _codegenInfo;

        public TaskBeginDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskBeginCodeGenInfo codegenInfo): base(sourceBuffer) {
            _codegenInfo = codegenInfo;
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {
            
            var location = await LocationFinder.FindTaskBeginDeclarationLocationAsync(project, _codegenInfo, cancellationToken)
                                               .ConfigureAwait(false);
            
            return ToEnumerable(location);
        }
    }
}