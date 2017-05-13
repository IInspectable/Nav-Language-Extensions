#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskIBeginInterfaceDeclarationLocationInfoProvider : CodeAnalysisLocationInfoProvider {

        readonly TaskDeclarationCodeModel _taskDeclarationCodeModel;

        public TaskIBeginInterfaceDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskDeclarationCodeModel taskDeclarationCodeModel) : base(sourceBuffer) {
            _taskDeclarationCodeModel = taskDeclarationCodeModel;
        }

        static ImageMoniker ImageMoniker { get { return ImageMonikers.GoToInterfacePublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {

                var locations = await LocationFinder.FindTaskIBeginInterfaceDeclarationLocations(
                   project          : project,
                   codegenInfo      : _taskDeclarationCodeModel,
                   cancellationToken: cancellationToken).ConfigureAwait(false);
                
                return locations.Select(location =>
                    LocationInfo.FromLocation(
                        location    : location,
                        displayName : _taskDeclarationCodeModel.FullyQualifiedBeginInterfaceName,
                        imageMoniker: ImageMoniker))
                    .OrderBy(li => li.DisplayName);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }
        }
    }
}