#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.CSharp.GoTo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeAnalysis {

    static class LocationFinder {

        #region FindBeginLogicAsync

        /// <summary>
        /// Findet die entsprechende BeginXYLogic Implementierung.
        /// </summary>
        /// <param name="project">Das Projekt aus dem der Begin Aufruf erfolgt</param>
        /// <param name="beginItfFullyQualifiedName">Der vollqualifizierte Name des IBegin...WFS interfaces</param>
        /// <param name="beginParameter">Die Parameter der aus dem WFS aufgrufenen Begin Methode</param>
        /// <param name="cancellationToken">Das Abbruchtoken</param>
        /// <returns></returns>
        public static Task<LocationResult> FindBeginLogicAsync(Project project, string beginItfFullyQualifiedName, IList<string> beginParameter, CancellationToken cancellationToken) {

            var task = Task.Run(() => {
                
                var compilation = project.GetCompilationAsync(cancellationToken).Result;

                var beginItf = compilation.GetTypeByMetadataName(beginItfFullyQualifiedName);
                if(beginItf == null) {
                    return LocationResult.FromError($"Unable to find interface '{beginItfFullyQualifiedName}'.");
                }

                var metaLocation = beginItf.Locations.FirstOrDefault(l => l.IsInMetadata);
                if(metaLocation != null) {
                    return LocationResult.FromError($"Missing project for assembly '{metaLocation.MetadataModule.MetadataName}'.");
                }

                var wfsClass = SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken)
                                           .Result
                                           .OfType<INamedTypeSymbol>()
                                           .FirstOrDefault();

                if(wfsClass == null) {
                    return LocationResult.FromError($"Unable to find a class implementing interface '{beginItf.ToDisplayString()}'.\n\nAre you missing a project?");
                }

                var beginLogicMethods = wfsClass.GetMembers()
                                                .OfType<IMethodSymbol>()
                                                .Where(m => m.Name == "BeginLogic");

                // Der erste Parameter ist immer das IBegin---WFS interface.
                beginParameter = beginParameter.Skip(1).ToList();
                var beginMethod = FindBestBeginLogicOverload(beginParameter, beginLogicMethods);
                if(beginMethod == null) {
                    return LocationResult.FromError($"Unable to find a matching overload for method 'BeginLogic'.");
                }

                var memberSyntax = beginMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
                var memberLocation = memberSyntax?.Identifier.GetLocation();

                if(memberLocation == null) {
                    return LocationResult.FromError($"Unable to get the location for method 'BeginLogic'.");
                }

                var lineSpan = memberLocation.GetLineSpan();
                if(!lineSpan.IsValid) {
                    return LocationResult.FromError($"Unable to get the line span for method 'BeginLogic'.");
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return LocationResult.FromLocation(new Location(textExtent, lineExtent, filePath));
            }, cancellationToken);
            
            return task;
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

        #endregion

        #region FindNavLocationAsync

        public static Task<LocationResult> FindNavLocationAsync(string sourceText, NavTaskAnnotation taskAnnotation, CancellationToken cancellationToken) {

            var locationResult = Task.Run(() => {

                var syntaxTree = SyntaxTree.ParseText(sourceText, taskAnnotation.NavFileName, cancellationToken);
                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if (codeGenerationUnitSyntax == null) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError("");
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                var task = codeGenerationUnit.Symbols
                                             .OfType<ITaskDefinitionSymbol>()
                                             .FirstOrDefault(t => t.Name == taskAnnotation.TaskName);

                if (task == null) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError("");
                }
                // TODO If's refaktorieren. Evtl. Visitor um Annotations bauen
                var triggerAnnotation = taskAnnotation as NavTriggerAnnotation;
                if (triggerAnnotation != null) {
                    var trigger = task.Transitions
                                      .SelectMany(t => t.Triggers)
                                      .FirstOrDefault(t => t.Name == triggerAnnotation.TriggerName);

                    return LocationResult.FromLocation(trigger?.Location);
                }

                var exitAnnotation = taskAnnotation as NavExitAnnotation;
                if (exitAnnotation != null) {
                    // TODO: Was wollen wir hier eigentlich "markieren"? Die ganze Transition, oder nur die Quelle?
                    var exitTransition = task.ExitTransitions.FirstOrDefault(et => et.Source?.Name == exitAnnotation.ExitTaskName);
                    return LocationResult.FromLocation(exitTransition?.Location);
                }

                var initAnnotation = taskAnnotation as NavInitAnnotation;
                if (initAnnotation != null) {
                    var init = task.NodeDeclarations.OfType<IInitNodeSymbol>()
                            .FirstOrDefault(n => n.Name == initAnnotation.InitName);

                    return LocationResult.FromLocation(init?.Location);
                }

                return LocationResult.FromLocation(task.Syntax.Identifier.GetLocation());
            }, cancellationToken);

            return locationResult;
        }

        #endregion

        #region FindClassDeclarationAsync

        public static Task<LocationResult> FindClassDeclarationAsync(Project project, string fullyQualifiedTypeName, CancellationToken cancellationToken) {

            var task = Task.Run(() => {

                var compilation = project.GetCompilationAsync(cancellationToken).Result;
                var typeSymbol  = compilation?.GetTypeByMetadataName(fullyQualifiedTypeName);

                if (typeSymbol == null) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError($"Der Typ '{fullyQualifiedTypeName} wurde nicht gefunden.");
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

                    return LocationResult.FromLocation(new Location(textExtent, lineExtent, filePath));
                }

                // TODO Fehlermeldung
                return LocationResult.FromError("");

            }, cancellationToken);

            return task;
        }

        #endregion

        #region FindTriggerLocationAsync

        public static Task<LocationResult> FindTriggerLocationAsync(Project project, string fullyQualifiedWfsBaseName, string triggerMethodName, CancellationToken cancellationToken) {

            var task = Task.Run(() => {

                var compilation   = project.GetCompilationAsync(cancellationToken).Result;
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(fullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError("");
                }

                // Wir kennen de facto nur den Baisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken).Result;
                var memberSymbol = derived?.SelectMany(d => d.GetMembers(triggerMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError("");
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    // TODO Fehlermeldung
                    return LocationResult.FromError("");
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return LocationResult.FromLocation(new Location(textExtent, lineExtent, filePath));

            }, cancellationToken);

            return task;
        }

        static IImmutableSet<T> ToImmutableSet<T>(T item) {
            return new[] { item }.ToImmutableHashSet();
        }

        #endregion
    }
}