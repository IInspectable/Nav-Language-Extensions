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

        readonly TaskInitCodeModel _taskInitCodeModel;

        public TaskBeginDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskInitCodeModel taskInitCodeModel): base(sourceBuffer) {
            _taskInitCodeModel = taskInitCodeModel;
        }

        static ImageMoniker ImageMoniker { get { return ImageMonikers.GoToMethodPublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var location = await LocationFinder.FindTaskBeginDeclarationLocationAsync(
                        project          : project, 
                        codegenInfo      : _taskInitCodeModel, 
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                var locationInfo= LocationInfo.FromLocation(
                        location    : location,
                        displayName : $"{_taskInitCodeModel.TaskCodeModel.WfsTypeName}.{_taskInitCodeModel.BeginLogicMethodName}",
                        imageMoniker: ImageMoniker);

                return ToEnumerable(locationInfo);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }            
        }
    }
}