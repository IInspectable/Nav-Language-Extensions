#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeAnalysis.Common;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using LocationKind = Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols.LocationKind;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskDeclarationLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly TaskCodeGenInfo _codegenInfo;

        public TaskDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskCodeGenInfo codegenInfo): base(sourceBuffer) {

            _codegenInfo  = codegenInfo;
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var locations = await LocationFinder.FindTaskDeclarationLocationsAsync(
                    project          : project, 
                    codegenInfo      : _codegenInfo, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return locations.Select(location =>
                                    LocationInfo.FromLocation(
                                        location   : location,
                                        // TODO Evtl. das Projekt mit angeben => das ist nicht notwendigerweise project!
                                        displayName: $"{PathHelper.GetRelativePath(project.FilePath, location.FilePath)}",
                                        kind       : LocationKind.TaskDeclaration))
                                .OrderBy(li=>li.DisplayName);                

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex));
            }           
        }
    }
}