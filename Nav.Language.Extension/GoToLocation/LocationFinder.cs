#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    // TODO Diese Klasse sollte besser in die CodeAnalysis Assembly. Momentan gibt es allerdings noch eine Abhängigkeit zu den ImageMonikers...
    public static class LocationFinder {

        #region FindNavDefinitionLocationsAsync

        public static Task<IEnumerable<LocationInfo>> FindNavDefinitionLocationsAsync(string sourceText, NavTaskAnnotation taskAnnotation, CancellationToken cancellationToken) {

            var locationResult = Task.Run(() => {

                var syntaxTree = SyntaxTree.ParseText(sourceText, taskAnnotation.NavFileName, cancellationToken);
                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if (codeGenerationUnitSyntax == null) {
                    // TODO Fehlermeldung
                    return ToEnumerable(LocationInfo.FromError("Unable to parse nav file."));
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                var task = codeGenerationUnit.Symbols
                                             .OfType<ITaskDefinitionSymbol>()
                                             .FirstOrDefault(t => t.Name == taskAnnotation.TaskName);

                if (task == null) {
                    // TODO Fehlermeldung
                    return ToEnumerable(LocationInfo.FromError($"Unable to locate task '{taskAnnotation.TaskName}'"));
                }
                // TODO If's refaktorieren. Evtl. Visitor um Annotations bauen, oder gleich explizit auflösen
                var triggerAnnotation = taskAnnotation as NavTriggerAnnotation;
                if (triggerAnnotation != null) {
                    var trigger = task.Transitions
                                      .SelectMany(t => t.Triggers)
                                      .FirstOrDefault(t => t.Name == triggerAnnotation.TriggerName);

                    if (trigger == null) {
                        // TODO Fehlermeldung
                        return ToEnumerable(LocationInfo.FromError($"Unable to locate signal trigger '{triggerAnnotation.TriggerName}'"));
                    }

                    return ToEnumerable(LocationInfo.FromLocation(
                        location    : trigger.Location,
                        displayName : trigger.Name,
                        imageMoniker: SymbolImageMonikers.SignalTrigger));
                }

                var exitAnnotation = taskAnnotation as NavExitAnnotation;
                if (exitAnnotation != null) {
                    var exitTransition = task.ExitTransitions.Where(et => et.Source?.Name == exitAnnotation.ExitTaskName)
                                             .Where(et => et.ConnectionPoint != null)
                                             .Select(et => LocationInfo.FromLocation(
                                                 location: et.ConnectionPoint?.Location,
                                                 displayName: et.ConnectionPoint?.Name,
                                                 imageMoniker: SymbolImageMonikers.ExitConnectionPoint));
                    return exitTransition;
                }

                var initAnnotation = taskAnnotation as NavInitAnnotation;
                if (initAnnotation != null) {
                    var init = task.NodeDeclarations
                                   .OfType<IInitNodeSymbol>()
                                   .FirstOrDefault(n => n.Name == initAnnotation.InitName);

                    if (init == null) {
                        // TODO Fehlermeldung
                        return ToEnumerable(LocationInfo.FromError($"Unable to locate init '{initAnnotation.InitName}'"));
                    }

                    return ToEnumerable(LocationInfo.FromLocation(
                        location: init.Location,
                        displayName: init.Name,
                        imageMoniker: SymbolImageMonikers.InitConnectionPoint));
                }

                return ToEnumerable(
                    LocationInfo.FromLocation(
                        location: task.Syntax.Identifier.GetLocation(),
                        displayName: task.Name,
                        imageMoniker: SymbolImageMonikers.TaskDefinition));

            }, cancellationToken);

            return locationResult;
        }

        #endregion

        #region FindCallBeginLogicDeclarationLocationsAsync

        /// <summary>
        /// Findet die entsprechende BeginXYLogic Implementierung.
        /// </summary>
        /// <returns></returns>
        public static Task<LocationInfo> FindCallBeginLogicDeclarationLocationsAsync(Project project, NavInitCallAnnotation initCallAnnotation, CancellationToken cancellationToken) {

            var task = Task.Run(() => {
                
                var compilation = project.GetCompilationAsync(cancellationToken).Result;

                var beginItf = compilation.GetTypeByMetadataName(initCallAnnotation.BeginItfFullyQualifiedName);
                if(beginItf == null) {
                    return LocationInfo.FromError($"Unable to find interface '{initCallAnnotation.BeginItfFullyQualifiedName}'.");
                }

                var metaLocation = beginItf.Locations.FirstOrDefault(l => l.IsInMetadata);
                if(metaLocation != null) {
                    return LocationInfo.FromError($"Missing project for assembly '{metaLocation.MetadataModule.MetadataName}'.");
                }

                var wfsClass = SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken)
                                           .Result
                                           .OfType<INamedTypeSymbol>()
                                           .FirstOrDefault();

                if(wfsClass == null) {
                    return LocationInfo.FromError($"Unable to find a class implementing interface '{beginItf.ToDisplayString()}'.\n\nAre you missing a project?");
                }

                var beginLogicMethods = wfsClass.GetMembers()
                                                .OfType<IMethodSymbol>()
                                                .Where(m => m.Name == "BeginLogic");

                // Der erste Parameter ist immer das IBegin---WFS interface.
                var beginParameter = initCallAnnotation.Parameter.Skip(1).ToList();
                var beginMethod = FindBestBeginLogicOverload(beginParameter, beginLogicMethods);
                if(beginMethod == null) {
                    return LocationInfo.FromError($"Unable to find a matching overload for method 'BeginLogic'.");
                }

                var memberSyntax = beginMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
                var memberLocation = memberSyntax?.Identifier.GetLocation();

                if(memberLocation == null) {
                    return LocationInfo.FromError($"Unable to get the location for method 'BeginLogic'.");
                }

                var lineSpan = memberLocation.GetLineSpan();
                if(!lineSpan.IsValid) {
                    return LocationInfo.FromError($"Unable to get the line span for method 'BeginLogic'.");
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return LocationInfo.FromLocation(
                    location    : new Location(textExtent, lineExtent, filePath),
                    displayName : "Go To BeginLogic",
                    imageMoniker: GoToImageMonikers.GoToInitCallDeclaration);

            }, cancellationToken);
            
            return task;
        }

       

        static IMethodSymbol FindBestBeginLogicOverload(IList<string> beginParameter, IEnumerable<IMethodSymbol> beginLogicMethods) {

            var bestMatch = beginLogicMethods.Select(m => new {
                MatchCount     = GetParameterMatchCount(beginParameter, AnnotationReader.ToParameterTypeList(m.Parameters)),
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
        
        #region FindTaskDeclarationLocationsAsync

        public static Task<IEnumerable<LocationInfo>> FindTaskDeclarationLocationsAsync(Project project, TaskCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(() => {

                var compilation = project.GetCompilationAsync(cancellationToken).Result;
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.FullyQualifiedWfsBaseName);

                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    return ToEnumerable( LocationInfo.FromError($"Der Typ '{codegenInfo.FullyQualifiedWfsBaseName} wurde nicht gefunden."));
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken).Result;

                var derivedSyntaxes = derived.SelectMany(d => d.DeclaringSyntaxReferences)
                                             .Select(dsr => dsr.GetSyntax())
                                             .OfType<TypeDeclarationSyntax>();

                var locs = new List<LocationInfo>();
                foreach (var ds in derivedSyntaxes) {
                    
                    var loc = ds.Identifier.GetLocation();

                    var filePath = loc.SourceTree?.FilePath;
                    // TODO Option .generated auch anzuzeigen
                    if (filePath?.EndsWith("generated.cs") == true) {
                        continue;
                    }

                    var lineSpan = loc.GetLineSpan();
                    if (!lineSpan.IsValid) {
                        continue;
                    }

                    var textExtent = loc.SourceSpan.ToTextExtent();
                    var lineExtent = lineSpan.ToLinePositionExtent();

                    locs.Add( LocationInfo.FromLocation(
                        location    : new Location(textExtent, lineExtent, filePath), 
                        // TODO Hier evtl. relativen Pfad angeben
                        displayName : filePath, 
                        imageMoniker: GoToImageMonikers.GoToTaskDeclaration));
                }

                if(!locs.Any()) {
                    // TODO Fehlermeldung
                    return ToEnumerable(LocationInfo.FromError("Unable to locate WFS"));
                }
                return locs;

            }, cancellationToken);

            return task;
        }

        #endregion

        #region FindTriggerDeclarationLocationsAsync

        public static Task<LocationInfo> FindTriggerDeclarationLocationsAsync(Project project, SignalTriggerCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(() => {

                var compilation   = project.GetCompilationAsync(cancellationToken).Result;
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError($"Unable to locate '{codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName}'");
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken).Result;
                var memberSymbol = derived?.SelectMany(d => d.GetMembers(codegenInfo.TriggerLogicMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError("Unable to locate member location.");
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError("Invalid linespan.");
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath   = memberLocation.SourceTree?.FilePath;

                return LocationInfo.FromLocation(
                    new Location(textExtent, lineExtent, filePath),
                    "Go To Trigger Declaration",
                    GoToImageMonikers.GoToTriggerDeclaration);

            }, cancellationToken);

            return task;
        }
        
        #endregion

        #region FindTaskExitDeclarationLocationAsync

        public static Task<LocationInfo> FindTaskExitDeclarationLocationAsync(Project project, TaskExitCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(() => {

                var compilation = project.GetCompilationAsync(cancellationToken).Result;
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError($"Unable to locate '{codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName}'");
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken).Result;
                var memberSymbol = derived?.SelectMany(d => d.GetMembers(codegenInfo.AfterLogicMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();

                if (memberLocation == null) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError("Unable to locate member location.");
                }

                var lineSpan = memberLocation.GetLineSpan();
                if (!lineSpan.IsValid) {
                    // TODO Fehlermeldung
                    return LocationInfo.FromError("Invalid linespan.");
                }

                var textExtent = memberLocation.SourceSpan.ToTextExtent();
                var lineExtent = lineSpan.ToLinePositionExtent();
                var filePath = memberLocation.SourceTree?.FilePath;

                return LocationInfo.FromLocation(
                    new Location(textExtent, lineExtent, filePath),
                    $"{codegenInfo.TaskCodeGenInfo.WfsTypeName}.{codegenInfo.AfterLogicMethodName}",
                    GoToImageMonikers.GoToTaskExitDeclaration);

            }, cancellationToken);

            return task;
        }

        #endregion

        static IEnumerable<T> ToEnumerable<T>(T value) {
            return new[] { value };
        }

        static IImmutableSet<T> ToImmutableSet<T>(T item) {
            return new[] { item }.ToImmutableHashSet();
        }
    }
}