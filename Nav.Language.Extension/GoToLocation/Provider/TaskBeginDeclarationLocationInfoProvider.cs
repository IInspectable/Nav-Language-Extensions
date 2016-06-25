#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TaskBeginDeclarationLocationInfoProvider : CodeAnalysisLocationInfoProvider {

        readonly TaskBeginCodeGenInfo _codegenInfo;

        public TaskBeginDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, TaskBeginCodeGenInfo codegenInfo): base(sourceBuffer) {
            _codegenInfo = codegenInfo;
        }

        static ImageMoniker ImageMoniker { get { return KnownMonikers.MethodPublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var location = await LocationFinder.FindTaskBeginDeclarationLocationAsync(
                        project          : project, 
                        codegenInfo      : _codegenInfo, 
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                var locationInfo= LocationInfo.FromLocation(
                        location    : location,
                        displayName : $"{_codegenInfo.TaskCodeGenInfo.WfsTypeName}.{_codegenInfo.BeginLogicMethodName}",
                        imageMoniker: ImageMoniker);

                return ToEnumerable(locationInfo);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }            
        }
    }
}