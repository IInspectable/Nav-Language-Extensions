#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.CSharp.GoTo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeAnalysis {

    static class LocationFinder {

        /// <summary>
        /// Findet die entsprechende BeginXYLogic Implementierung.
        /// </summary>
        /// <param name="project">Das Projekt aus dem der Begin Aufruf erfolgt</param>
        /// <param name="beginItfFullyQualifiedName">Der vollqualifizierte Name des IBegin...WFS interfaces</param>
        /// <param name="beginParameter">Die Parameter der aus dem WFS aufgrufenen Begin Methode</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<LocationResult> FindBeginLogicAsync(Project project, string beginItfFullyQualifiedName, IList<string> beginParameter, CancellationToken cancellationToken) {


            beginParameter = beginParameter.Skip(1).ToList();

            var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                    
            var beginItf = compilation.GetTypeByMetadataName(beginItfFullyQualifiedName);
            if (beginItf == null) {
                // TODO Messagebox, da Assembly evtl. nicht geladen.
                return new LocationResult {
                    ErrorMessage = $"Das Begin Interface {beginItfFullyQualifiedName} wurde nicht gefunden."
                };
            }

            var wfsClass = (await SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken).ConfigureAwait(false))
                             .OfType<INamedTypeSymbol>()
                             .FirstOrDefault();

            var beginLogicMethods = wfsClass?.GetMembers()
                                             .OfType<IMethodSymbol>()
                                             .Where(m => m.Name == "BeginLogic");


            var beginMethod = FindBestBeginLogicOverload(beginParameter, beginLogicMethods);
            if (beginMethod == null) {
                return new LocationResult {
                    ErrorMessage = $"Die passende BeginLogic Überladung wurde nicht gefunden."
                };
            }

            var memberSyntax   = beginMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
            var memberLocation = memberSyntax?.Identifier.GetLocation();

            if (memberLocation == null) {
                // TODO Messagebox, da Assembly evtl. nicht geladen.
                //var loc = beginMethod?.Locations[0];

                return new LocationResult {
                    ErrorMessage = $"memberLocation wurde nicht gefunden."
                };
            }

            var lineSpan = memberLocation.GetLineSpan();
            if (!lineSpan.IsValid) {
                return new LocationResult {
                    ErrorMessage = $"lineSpan is not valid"
                };
            }

            var textExtent = memberLocation.SourceSpan.ToTextExtent();
            var lineExtent = lineSpan.ToLinePositionExtent();
            var filePath   = memberLocation.SourceTree?.FilePath;

            return new LocationResult {
                Location = new Location(textExtent, lineExtent, filePath)
            };
        }

        public static List<string> ToParameterTypeList(IEnumerable<IParameterSymbol> beginLogicParameter) {
            return beginLogicParameter.OrderBy(p=>p.Ordinal).Select(p => p.ToDisplayString()).ToList();
        }

        static IMethodSymbol FindBestBeginLogicOverload(IList<string> beginParameter, IEnumerable<IMethodSymbol> beginLogicMethods) {

            var bestMatch= beginLogicMethods.Select(m => new {
                MatchCount     = GetParameterMatchCount(beginParameter, ToParameterTypeList(m.Parameters)),
                ParameterCount = m.Parameters.Length,
                Method         = m})
                    .Where(x => x.MatchCount >=0 ) // 0 ist OK, falls der Init keine Argumente hat!
                    .OrderByDescending(x=> x.MatchCount)
                    .ThenBy(x=> x.ParameterCount)
                    .Select(x=> x.Method)
                    .FirstOrDefault();

            return bestMatch;
        }

        static int GetParameterMatchCount(IList<string> beginParameter, IList<string> beginLogicParameter) {
            
            if (beginLogicParameter.Count  < beginParameter.Count) {
                return -1;
            }

            var matchCount = 0;
            for (int i = 0; i < beginParameter.Count; i++) {

                if (beginParameter[i] != beginLogicParameter[i]) {
                    break;
                }
                matchCount++;
            }
            return matchCount;
        }
        
        public static Task<Location> FindNavLocationAsync(string sourceText, NavTaskAnnotation taskAnnotation, CancellationToken cancellationToken) {

            var location = Task.Run(() => {
                //Thread.Sleep(4000);

                var syntaxTree = SyntaxTree.ParseText(sourceText, taskAnnotation.NavFileName, cancellationToken);
                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if (codeGenerationUnitSyntax == null) {
                    return null;
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                var task = codeGenerationUnit.Symbols
                        .OfType<ITaskDefinitionSymbol>()
                        .FirstOrDefault(t => t.Name == taskAnnotation.TaskName);

                if (task == null) {
                    return null;
                }
                // TODO If's refaktorieren. Evtl. Visitor um Annotations bauen
                var triggerAnnotation = taskAnnotation as NavTriggerAnnotation;
                if (triggerAnnotation != null) {
                    var trigger = task.Transitions
                            .SelectMany(t => t.Triggers)
                            .FirstOrDefault(t => t.Name == triggerAnnotation.TriggerName);

                    return trigger?.Location;
                }

                var exitAnnotation = taskAnnotation as NavExitAnnotation;
                if (exitAnnotation != null) {
                    // TODO: Was wollen wir hier eigentlich "markieren"? Die ganze Transition, oder nur die Quelle?
                    var exitTransition = task.ExitTransitions.FirstOrDefault(et => et.Source?.Name == exitAnnotation.ExitTaskName);
                    return exitTransition?.Location;
                }

                var initAnnotation = taskAnnotation as NavInitAnnotation;
                if (initAnnotation != null) {
                    var init = task.NodeDeclarations.OfType<IInitNodeSymbol>()
                            .FirstOrDefault(n => n.Name == initAnnotation.InitName);

                    return init?.Location;
                }

                return task.Syntax.Identifier.GetLocation();
            }, cancellationToken);

            return location;
        }

        public static Task<Location> FindClassDeclaration(Project project, string fullyQualifiedTypeName, CancellationToken cancellationToken) {

            var task = Task.Run(async () => {

                var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                var typeSymbol = compilation?.GetTypeByMetadataName(fullyQualifiedTypeName);

                if (typeSymbol == null) {
                    return null;
                }

                foreach (var refe in typeSymbol.DeclaringSyntaxReferences) {
                    var loc = refe.GetSyntax().GetLocation();

                    var filePath = loc.SourceTree?.FilePath;
                    if (filePath?.EndsWith("generated.cs") == true) {
                        continue;
                    }

                    var lineSpan = loc.GetLineSpan();
                    if (!lineSpan.IsValid) {
                        continue;
                    }

                    var textExtent = loc.SourceSpan.ToTextExtent();
                    var lineExtent = lineSpan.ToLinePositionExtent();

                    return new Location(textExtent, lineExtent, filePath);
                }

                return null;
            }, cancellationToken);

            return task;
        }

        public static Task<Location> FindTriggerLocation(Project project, string fullyQualifiedWfsBaseName, string triggerMethodName, CancellationToken cancellationToken) {

            var task = Task.Run(async () => {

                var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(fullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    return null;
                }

                // Wir kennen de facto nur den Baisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = await SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken);
                var memberSymbol = derived?.SelectMany(d => d.GetMembers(triggerMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    return null;
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    return null;
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath = memberLocation.SourceTree?.FilePath;

                return new Location(textExtent, lineExtent, filePath);
            }, cancellationToken);

            return task;
        }

        static IImmutableSet<T> ToImmutableSet<T>(T item) {
            return new[] { item }.ToImmutableHashSet();
        }
    }
}