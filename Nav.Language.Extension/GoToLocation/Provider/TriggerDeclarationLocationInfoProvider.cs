#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TriggerDeclarationLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly SignalTriggerCodeGenInfo _codegenInfo;

        public TriggerDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, SignalTriggerCodeGenInfo codegenInfo): base(sourceBuffer) {
            _codegenInfo  = codegenInfo;
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var location = await LocationFinder.FindTriggerDeclarationLocationsAsync(
                    project          : project, 
                    codegenInfo      : _codegenInfo, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var locationInfo = LocationInfo.FromLocation(
                    location    : location,
                    displayName : "Go To Trigger Declaration",
                    imageMoniker: KnownMonikers.MethodPublic);

                return ToEnumerable(locationInfo);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex));
            }
        }
    }
}