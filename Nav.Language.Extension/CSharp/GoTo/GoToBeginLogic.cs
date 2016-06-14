#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToBeginLogic: IntraTextGoToTag {

        readonly ITextBuffer _sourceBuffer;
        readonly string _beginItfFullyQualifiedName;
        readonly IList<string> _parameter;

        public GoToBeginLogic(ITextBuffer sourceBuffer, string beginItfFullyQualifiedName, IEnumerable<IParameterSymbol> parameter) {
            _sourceBuffer               = sourceBuffer;
            // der erste Parameter ist der BeginWfs
            _parameter                  = ToParameterList(parameter.Skip(1));
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

                var wfsClass = SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken)
                                           .Result.OfType<INamedTypeSymbol>().FirstOrDefault();

                var beginMethod = wfsClass?.GetMembers()
                                           .OfType<IMethodSymbol>()
                                           .FirstOrDefault(m=> m.Name == "BeginLogic" && // TODO Konstante in CodeGen NS Klasse packen
                                                               BindsToBeginLogic(ToParameterList(m.Parameters)));

                // TODO hier die richtige Überladung finden.
                var memberSyntax = beginMethod?.DeclaringSyntaxReferences.FirstOrDefault()
                                               ?.GetSyntax() as MethodDeclarationSyntax;
                var memberLocation = memberSyntax?.Identifier.GetLocation();

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

        static List<string> ToParameterList(IEnumerable<IParameterSymbol> parameter) {
            return parameter.OrderBy(p=>p.Ordinal).Select(p => p.ToDisplayString()).ToList();
        }

        bool BindsToBeginLogic(IList<string> beginLogicParameter) {

            if (_parameter.Count != beginLogicParameter.Count) {
                return false;
            }

            for (int i = 0; i < beginLogicParameter.Count; i++) {

                var pa = _parameter[i];
                var pb =  beginLogicParameter[i];
                if (pa != pb) {
                    return false;
                }
            }
            return true;
        }

        public override ImageMoniker ImageMoniker {
            get { return GoToImageMonikers.GoToBeginLogic; }
        }

        public override object ToolTip {
            get { return "Go To Begin Logic"; }
        }

    }

}