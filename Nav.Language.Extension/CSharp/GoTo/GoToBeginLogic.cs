#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToBeginLogic: IntraTextGoToTag {

        readonly ITextBuffer _sourceBuffer;
        readonly string _beginItfFullyQualifiedName;

        public GoToBeginLogic(ITextBuffer sourceBuffer, string beginItfFullyQualifiedName) {
            _sourceBuffer = sourceBuffer;
            _beginItfFullyQualifiedName = beginItfFullyQualifiedName;

        }

        public override async Task<Location> GoToLocationAsync(CancellationToken cancellationToken = new CancellationToken()) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                return null;
            }

            var location = await Task.Run(() => {

                var compilation = project.GetCompilationAsync(cancellationToken).Result;
                var beginItf = compilation.GetTypeByMetadataName(_beginItfFullyQualifiedName);
                if (beginItf == null) {
                    return null;
                }

                // TODO hier die richtige Überladung finden.
                // semanticModel.ResolveOverloads?
                var beginMethod = beginItf.GetMembers().OfType<IMethodSymbol>().FirstOrDefault();

                var beginImpl = SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken)
                                         .Result
                                         .OfType<ITypeSymbol>()
                                         .Select(d => d.FindImplementationForInterfaceMember(beginMethod))
                                         .FirstOrDefault(m => m != null);

                var memberLocation = beginImpl?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    return null;
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    return null;
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return new Location(textExtent, lineExtent, filePath);

            }, cancellationToken);

            NavLanguagePackage.GoToLocationInPreviewTab(location);

            return location;
        }

        public override ImageMoniker ImageMoniker {
            get { return GoToImageMonikers.GoToBeginLogic; }
        }

        public override object ToolTip {
            get { return "Go To Begin Logic"; }
        }

    }

}