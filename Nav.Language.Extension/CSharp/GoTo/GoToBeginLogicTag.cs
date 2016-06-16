#region Using Directives

using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToBeginLogicTag: IntraTextGoToTag {

        readonly ITextBuffer _sourceBuffer;
        readonly string _beginItfFullyQualifiedName;
        readonly IList<string> _beginParameter;

        public GoToBeginLogicTag(ITextBuffer sourceBuffer, string beginItfFullyQualifiedName, IEnumerable<IParameterSymbol> beginParameter) {
            _sourceBuffer               = sourceBuffer;
            _beginParameter             = LocationFinder.ToParameterTypeList(beginParameter);
            _beginItfFullyQualifiedName = beginItfFullyQualifiedName;
        }
        
        public override async Task<Location> GetLocationAsync(CancellationToken cancellationToken = new CancellationToken()) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return null;
            }

            var location = await LocationFinder.FindBeginLogicAsync(
                project                   : project, 
                beginItfFullyQualifiedName: _beginItfFullyQualifiedName, 
                beginParameter            : _beginParameter, 
                cancellationToken         : cancellationToken).ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested) {
                return null;
            }

            // TODO Das muss wieder raus. Der Aufrufer soll entscheiden...
           //if (location.Location != null) {
           //    NavLanguagePackage.GoToLocationInPreviewTab(location.Location);
           //} else {
           //    MessageBox.Show(location.ErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Error);
           //}

            return location.Location;
        }

        public override ImageMoniker ImageMoniker {
            get { return GoToImageMonikers.GoToBeginLogic; }
        }

        public override object ToolTip {
            get { return "Go To Begin Logic"; }
        }
    }
}