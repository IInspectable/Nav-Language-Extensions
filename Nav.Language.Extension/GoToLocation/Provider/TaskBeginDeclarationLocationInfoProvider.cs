#region Using Directives

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

    class TaskBeginDeclarationLocationInfoProvider : CodeAnalysisLocationInfoProvider {

        readonly TaskBeginCodeModel _taskBeginCodeModel;

        public TaskBeginDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskBeginCodeModel taskBeginCodeModel): base(sourceBuffer) {
            _taskBeginCodeModel = taskBeginCodeModel;
        }

        static ImageMoniker ImageMoniker { get { return ImageMonikers.GoToMethodPublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var location = await LocationFinder.FindTaskBeginDeclarationLocationAsync(
                        project          : project, 
                        codegenInfo      : _taskBeginCodeModel, 
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                var locationInfo= LocationInfo.FromLocation(
                        location    : location,
                        displayName : $"{_taskBeginCodeModel.TaskCodeModel.WfsTypeName}.{_taskBeginCodeModel.BeginLogicMethodName}",
                        imageMoniker: ImageMoniker);

                return ToEnumerable(locationInfo);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }            
        }
    }
}