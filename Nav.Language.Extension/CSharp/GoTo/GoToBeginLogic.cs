#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        class LocationResult {
            public Location Location { get; set; }
            public string ErrorMessage { get; set; }
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
                    // TODO Messagebox, da Assembly evtl. nicht geladen.
                    return new LocationResult {ErrorMessage =$"Das Begin Interface {_beginItfFullyQualifiedName} wurde nicht gefunden." };
                }

                var wfsClass = SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken)
                                           .Result.OfType<INamedTypeSymbol>().FirstOrDefault();

                var beginMethods =  wfsClass?.GetMembers()
                                             .OfType<IMethodSymbol>()
                                             .Where(m=> m.Name == "BeginLogic");

                // TODO hier die richtige Überladung finden.
                var beginMethod = FindBestBeginLogicOverload(beginMethods);

                if (beginMethod == null) {
                    return new LocationResult { ErrorMessage = $"Die passende BeginLogic Methode wurde nicht gefunden." };
                }
                
                var memberSyntax = beginMethod.DeclaringSyntaxReferences.FirstOrDefault()
                                               ?.GetSyntax() as MethodDeclarationSyntax;
                var memberLocation = memberSyntax?.Identifier.GetLocation();

                if (memberLocation == null) {
                    // TODO Messagebox, da Assembly evtl. nicht geladen.
                    //var loc = beginMethod?.Locations[0];

                    return new LocationResult { ErrorMessage = $"memberLocation wurde  icht gefunden." };
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    return new LocationResult { ErrorMessage = $"lineSpan is not valid" };
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return new LocationResult { Location = new Location(textExtent, lineExtent, filePath)};

            }, cancellationToken);

            if (location.Location != null) {
                NavLanguagePackage.GoToLocationInPreviewTab(location.Location);
            } else {
                MessageBox.Show(location.ErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return location.Location;
        }

        static List<string> ToParameterList(IEnumerable<IParameterSymbol> beginLogicParameter) {
            return beginLogicParameter.OrderBy(p=>p.Ordinal).Select(p => p.ToDisplayString()).ToList();
        }
       
        IMethodSymbol FindBestBeginLogicOverload(IEnumerable<IMethodSymbol> beginLogicMethods) {

           var bestMatch= beginLogicMethods.Select(m => new {
                                        MatchCount     = GetParameterMatchCount(ToParameterList(m.Parameters)),
                                        ParameterCount = m.Parameters.Length,
                                        Method         = m})
                                 .Where(x => x.MatchCount >=0 ) // 0 ist OK, falls der Init keine Argumente hat!
                                 .OrderByDescending(x=> x.MatchCount)
                                 .ThenBy(x=> x.ParameterCount)
                                 .Select(x=> x.Method)
                                 .FirstOrDefault();

            return bestMatch;
        }

        int GetParameterMatchCount(IList<string> beginLogicParameter) {
            
            if (beginLogicParameter.Count  < _parameter.Count) {
                return -1;
            }

            var matchCount = 0;
            for (int i = 0; i < _parameter.Count; i++) {

                if (_parameter[i] != beginLogicParameter[i]) {
                    break;
                }
                matchCount++;
            }
            return matchCount;
        }

        public override ImageMoniker ImageMoniker {
            get { return GoToImageMonikers.GoToBeginLogic; }
        }

        public override object ToolTip {
            get { return "Go To Begin Logic"; }
        }
    }

}