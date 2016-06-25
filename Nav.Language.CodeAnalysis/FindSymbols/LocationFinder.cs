#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.Common;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols {

    public static class LocationFinder {

        #region FindNavLocationsAsync

        /// <exception cref="LocationNotFoundException"/>
        public static Task<IEnumerable<Location>> FindNavLocationsAsync(string sourceText, NavTaskAnnotation annotation, CancellationToken cancellationToken) {
            return FindNavLocationsAsync(sourceText, annotation, GetTaskLocations, cancellationToken);
        }

        /// <exception cref="LocationNotFoundException"/>
        public static Task<IEnumerable<Location>> FindNavLocationsAsync(string sourceText, NavInitAnnotation annotation, CancellationToken cancellationToken) {
            return FindNavLocationsAsync(sourceText, annotation, GetInitLocations, cancellationToken);
        }

        /// <exception cref="LocationNotFoundException"/>
        public static Task<IEnumerable<AmbiguousLocation>> FindNavLocationsAsync(string sourceText, NavExitAnnotation annotation, CancellationToken cancellationToken) {
            return FindNavLocationsAsync(sourceText, annotation, GetExitLocations, cancellationToken);
        }

        /// <exception cref="LocationNotFoundException"/>
        public static Task<IEnumerable<Location>> FindNavLocationsAsync(string sourceText, NavTriggerAnnotation annotation, CancellationToken cancellationToken) {
            return FindNavLocationsAsync(sourceText, annotation, GetTriggerLocations, cancellationToken);
        }

        /// <exception cref="LocationNotFoundException"/>
        static Task<IEnumerable<TLocation>> FindNavLocationsAsync<TAnnotation, TLocation>(
                        string sourceText, 
                        TAnnotation annotation, 
                        Func<ITaskDefinitionSymbol, TAnnotation, IEnumerable<TLocation>> locBuilder, 
                        CancellationToken cancellationToken)
            where TAnnotation: NavTaskAnnotation 
            where TLocation: Location {

            var locationResult = Task.Run(() => {

                var syntaxTree = SyntaxTree.ParseText(sourceText, annotation.NavFileName, cancellationToken);
                var codeGenerationUnitSyntax = syntaxTree.GetRoot() as CodeGenerationUnitSyntax;
                if (codeGenerationUnitSyntax == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Error while parsing nav file '{annotation.NavFileName}'.");
                }

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, cancellationToken);

                var task = codeGenerationUnit.Symbols
                                             .OfType<ITaskDefinitionSymbol>()
                                             .FirstOrDefault(t => t.Name == annotation.TaskName);

                if (task == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find task '{annotation.TaskName}' in file '{annotation.NavFileName}'");
                }
                
                return locBuilder(task, annotation);                

            }, cancellationToken);

            return locationResult;
        }

        static IEnumerable<Location> GetTaskLocations(ITaskDefinitionSymbol task, NavTaskAnnotation nav) {

            return ToEnumerable(task.Syntax.Identifier.GetLocation());
        }

        static IEnumerable<Location> GetTriggerLocations(ITaskDefinitionSymbol task, NavTriggerAnnotation triggerAnnotation) {

            var trigger = task.Transitions
                              .SelectMany(t => t.Triggers)
                              .FirstOrDefault(t => t.Name == triggerAnnotation.TriggerName);

            if (trigger == null) {
                // TODO Fehlermeldung
                // TODO Evtl. sollte es Locations mit Fehlern geben? Dann würden wir in diesem Fall wenigstens zum task selbst navigieren,
                //      nachdem wir eine Fehlermeldung angezeigt haben.
                throw new LocationNotFoundException($"Unable to find signal trigger '{triggerAnnotation.TriggerName}' in task '{task.Name}'");
            }

            return ToEnumerable(trigger.Location);
        }

        static IEnumerable<Location> GetInitLocations(ITaskDefinitionSymbol task, NavInitAnnotation initAnnotation) {

            var initNode = task.NodeDeclarations
                           .OfType<IInitNodeSymbol>()
                           .FirstOrDefault(n => n.Name == initAnnotation.InitName);

            if (initNode == null) {
                // TODO Fehlermeldung
                // TODO Evtl. sollte es Locations mit Fehlern geben? Dann würden wir in diesem Fall wenigstens zum task selbst navigieren,
                //      nachdem wir eine Fehlermeldung angezeigt haben.
                throw new LocationNotFoundException($"Unable to find init '{initAnnotation.InitName}' in task '{task.Name}'");
            }

            return ToEnumerable(initNode.Location);            
        }

        static IEnumerable<AmbiguousLocation> GetExitLocations(ITaskDefinitionSymbol task, NavExitAnnotation exitAnnotation) {

            var exitTransitions = task.ExitTransitions
                                      .Where(et => et.Source?.Name == exitAnnotation.ExitTaskName)
                                      .Where(et => et.ConnectionPoint != null)
                                      .Select(et => new AmbiguousLocation(et.ConnectionPoint?.Location, et.ConnectionPoint.Name))
                                      .ToList();

            if (!exitTransitions.Any()) {
                // TODO Fehlermeldung
                // TODO Evtl. sollte es Locations mit Fehlern geben? Dann würden wir in diesem Fall wenigstens zum task selbst navigieren,
                //      nachdem wir eine Fehlermeldung angezeigt haben.
                throw new LocationNotFoundException($"Unable to find the exit transitions in task '{exitAnnotation.ExitTaskName}'");
            }

            return exitTransitions;
        }

        #endregion

        #region FindCallBeginLogicDeclarationLocationsAsync

        /// <summary>
        /// Findet die entsprechende BeginXYLogic Implementierung.
        /// </summary>
        /// <exception cref="LocationNotFoundException"/>
        /// <returns></returns>
        public static Task<Location> FindCallBeginLogicDeclarationLocationsAsync(Project project, NavInitCallAnnotation initCallAnnotation, CancellationToken cancellationToken) {

            var task = Task.Run(async () => {
                
                var compilation = await project.GetCompilationAsync(cancellationToken);
                var beginItf    = compilation.GetTypeByMetadataName(initCallAnnotation.BeginItfFullyQualifiedName);
                if(beginItf == null) {
                    throw new LocationNotFoundException($"Unable to find interface '{initCallAnnotation.BeginItfFullyQualifiedName}'.");
                }

                var metaLocation = beginItf.Locations.FirstOrDefault(l => l.IsInMetadata);
                if(metaLocation != null) {
                    throw new LocationNotFoundException($"Missing project for assembly '{metaLocation.MetadataModule.MetadataName}'.");
                }

                var wfsClass = (await SymbolFinder.FindImplementationsAsync(beginItf, project.Solution, null, cancellationToken))
                                           .OfType<INamedTypeSymbol>()
                                           .FirstOrDefault();

                if(wfsClass == null) {
                    throw new LocationNotFoundException($"Unable to find a class implementing interface '{beginItf.ToDisplayString()}'.\n\nAre you missing a project?");
                }

                var beginLogicMethods = wfsClass.GetMembers()
                                                .OfType<IMethodSymbol>()
                                                // TODO String ais CodeGenInfo o.ä.
                                                .Where(m => m.Name == "BeginLogic");

                // Der erste Parameter ist immer das IBegin---WFS interface.
                var beginParameter = initCallAnnotation.Parameter.Skip(1).ToList();
                var beginMethod    = FindBestBeginLogicOverload(beginParameter, beginLogicMethods);

                if (beginMethod == null) {
                    throw new LocationNotFoundException($"Unable to find a matching overload for method 'BeginLogic'.");
                }

                var memberSyntax   = beginMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
                var memberLocation = memberSyntax?.Identifier.GetLocation();
                var location       = ToLocation(memberLocation);

                if(location == null) {
                    throw new LocationNotFoundException($"Unable to get the location for method 'BeginLogic'.");
                }

                return location;

            }, cancellationToken);
            
            return task;
        }
        
        static IMethodSymbol FindBestBeginLogicOverload(IList<string> beginParameter, IEnumerable<IMethodSymbol> beginLogicMethods) {

            var bestMatch = beginLogicMethods.Select(m => new {
                                MatchCount     = GetParameterMatchCount(beginParameter, AnnotationReader.ToComparableParameterTypeList(m.Parameters)),
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

        /// <exception cref="LocationNotFoundException"/>
        public static Task<IList<Location>> FindTaskDeclarationLocationsAsync(Project project, TaskCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(async () => {

                var compilation   = await project.GetCompilationAsync(cancellationToken);
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.FullyQualifiedWfsBaseName);

                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find type '{codegenInfo.FullyQualifiedWfsBaseName}'.");
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived = await SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken);

                var derivedSyntaxes = derived.SelectMany(d => d.DeclaringSyntaxReferences)
                                             .Select(dsr => dsr.GetSyntax())
                                             .OfType<TypeDeclarationSyntax>();

                IList<Location> locs = new List<Location>();
                foreach (var ds in derivedSyntaxes) {
                    
                    var loc = ds.Identifier.GetLocation();

                    var filePath = loc.SourceTree?.FilePath;
                    // TODO Evtl. Option um .generated files auch anzuzeigen
                    if (filePath?.EndsWith("generated.cs") == true) {
                        continue;
                    }

                    var lineSpan = loc.GetLineSpan();
                    if (!lineSpan.IsValid) {
                        continue;
                    }
                    
                    var textExtent = loc.SourceSpan.ToTextExtent();
                    var lineExtent = lineSpan.ToLinePositionExtent();
                    
                    locs.Add(new Location(textExtent, lineExtent, filePath));
                }

                if(!locs.Any()) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find any classes derived from '{codegenInfo.FullyQualifiedWfsBaseName}'");
                }
                return locs;

            }, cancellationToken);

            return task;
        }

        #endregion

        #region FindTriggerDeclarationLocationsAsync

        /// <exception cref="LocationNotFoundException"/>
        public static Task<Location> FindTriggerDeclarationLocationsAsync(Project project, SignalTriggerCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(async ()  =>  {

                var compilation   = await project.GetCompilationAsync(cancellationToken);
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find type '{codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName}'");
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived        = await SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken);
                var memberSymbol   = derived?.SelectMany(d => d.GetMembers(codegenInfo.TriggerLogicMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();
                var location       = ToLocation(memberLocation);

                if (location == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException("Unable to get member location.");
                }

                return location;

            }, cancellationToken);

            return task;
        }

        #endregion

        #region FindTaskBeginDeclarationLocationAsync

        /// <exception cref="LocationNotFoundException"/>
        public static Task<Location> FindTaskBeginDeclarationLocationAsync(Project project, TaskBeginCodeGenInfo codegenInfo, CancellationToken cancellationToken) {
            var task = Task.Run(async () => {
                var compilation = await project.GetCompilationAsync(cancellationToken);
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find '{codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName}'");
                }

                var taskAnnotation = wfsBaseSymbol.DeclaringSyntaxReferences
                                                  .Select(sr => sr.GetSyntax())
                                                  .OfType<ClassDeclarationSyntax>()
                                                  .Select(cd => AnnotationReader.ReadNavTaskAnnotation(cd, wfsBaseSymbol))
                                                  .FirstOrDefault();

                if (taskAnnotation == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException("Unable to find WFS");
                }

                var derived = await SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken);

                var beginLogics = derived.SelectMany(d=> d.GetMembers(codegenInfo.BeginLogicMethodName).OfType<IMethodSymbol>());

                foreach(var beginLogic in beginLogics) {

                    var methodDeclaration = beginLogic.DeclaringSyntaxReferences
                                                      .Select(sr => sr.GetSyntax())
                                                      .OfType<MethodDeclarationSyntax>()
                                                      .FirstOrDefault();

                    if (methodDeclaration == null) {
                        continue;
                    }
                
                    var initAnnotation = AnnotationReader.ReadNavInitAnnotation(taskAnnotation, methodDeclaration, beginLogic);
                    if (initAnnotation?.InitName != codegenInfo.InitName) {
                        continue;
                    }

                    var location = ToLocation(methodDeclaration.Identifier.GetLocation());
                    if (location == null) {
                        // TODO Fehlermeldung
                        throw new LocationNotFoundException("Unable to get member location.");
                    }

                    return location;                    
                }

                // TODO Fehlermeldung
                throw new LocationNotFoundException("Unable to find BeginLogic");

            }, cancellationToken);

            return task;
        }

        #endregion

        #region FindTaskExitDeclarationLocationAsync

        /// <exception cref="LocationNotFoundException"/>
        public static Task<Location> FindTaskExitDeclarationLocationAsync(Project project, TaskExitCodeGenInfo codegenInfo, CancellationToken cancellationToken) {

            var task = Task.Run(async () => {

                var compilation   = await project.GetCompilationAsync(cancellationToken);
                var wfsBaseSymbol = compilation?.GetTypeByMetadataName(codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName);
                if (wfsBaseSymbol == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException($"Unable to find '{codegenInfo.TaskCodeGenInfo.FullyQualifiedWfsBaseName}'");
                }

                // Wir kennen de facto nur den Basisklassen Namespace + Namen, da die abgeleiteten Klassen theoretisch in einem
                // anderen Namespace liegen können. Deshalb steigen wir von der Basisklasse zu den abgeleiteten Klassen ab.
                var derived        = await SymbolFinder.FindDerivedClassesAsync(wfsBaseSymbol, project.Solution, ToImmutableSet(project), cancellationToken);
                var memberSymbol   = derived?.SelectMany(d => d.GetMembers(codegenInfo.AfterLogicMethodName)).FirstOrDefault();
                var memberLocation = memberSymbol?.Locations.FirstOrDefault();
                
                var location = ToLocation(memberLocation);
                if (location == null) {
                    // TODO Fehlermeldung
                    throw new LocationNotFoundException("Unable to get member location.");
                }

                return location;

            }, cancellationToken);

            return task;
        }

        #endregion

        [CanBeNull]
        static Location ToLocation([CanBeNull] Microsoft.CodeAnalysis.Location memberLocation) {

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

            var loc = new Location(textExtent, lineExtent, filePath);

            return loc;
        }

        static IEnumerable<T> ToEnumerable<T>(T value) {
            return new[] { value };
        }

        static IImmutableSet<T> ToImmutableSet<T>(T item) {
            return new[] { item }.ToImmutableHashSet();
        }        
    }
}