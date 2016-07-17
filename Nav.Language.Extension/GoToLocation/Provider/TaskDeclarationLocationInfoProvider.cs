#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Utilities.IO;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskDeclarationLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly TaskCodeModel _taskCodeModel;

        public TaskDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskCodeModel taskCodeModel): base(sourceBuffer) {
            _taskCodeModel  = taskCodeModel;
        }

        static ImageMoniker ImageMoniker { get { return KnownMonikers.ClassPublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var locations = await LocationFinder.FindTaskDeclarationLocationsAsync(
                    project          : project, 
                    codegenInfo      : _taskCodeModel, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return locations.Select(location =>
                                    LocationInfo.FromLocation(
                                        location    : location,
                                        displayName : $"{PathHelper.GetRelativePath(project.FilePath, location.FilePath)}",
                                        imageMoniker: ImageMoniker))
                                .OrderBy(li=>li.DisplayName);                

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }           
        }
    }
}