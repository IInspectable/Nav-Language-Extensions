#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    sealed class NavTag {
        public string TagName { get; set; }
        public string Content { get; set; }
    }

    public static class AnnotationReader {

        public static IEnumerable<NavTaskAnnotation> ReadNavTaskAnnotations(Document document) {
            var semanticModel = document.GetSemanticModelAsync().Result;
            var rootNode = semanticModel.SyntaxTree.GetRoot();

            var classDeclarations = rootNode.DescendantNodesAndSelf()
                                            .OfType<ClassDeclarationSyntax>();

            // TODO Diese ganze Gaudi hätte ich gerne von den "Tags" entkoppelt, und in den AnnotationReader gelegt
            foreach (var classDeclaration in classDeclarations) {

                var navTaskAnnotation = ReadNavTaskAnnotation(semanticModel, classDeclaration);

                if (navTaskAnnotation == null) {
                    continue;
                }

                yield return navTaskAnnotation;

                // Method Annotations
                var methodDeclarations = classDeclaration.DescendantNodes()
                                                        .OfType<MethodDeclarationSyntax>();
                var methodAnnotations = ReadMethodAnnotations(semanticModel, navTaskAnnotation, methodDeclarations);

                foreach (var methodAnnotation in methodAnnotations) {
                    yield return methodAnnotation;
                }


                var invocationExpressions = classDeclaration.DescendantNodes()
                                                            .OfType<InvocationExpressionSyntax>();
                var callAnnotations = ReadInitCallAnnotation(semanticModel, navTaskAnnotation, invocationExpressions);

                foreach (var initCallAnnotation in callAnnotations) {

                    yield return initCallAnnotation;
                }
            }
        }

        #region ReadNavTaskAnnotation

        [CanBeNull]
        public static NavTaskAnnotation ReadNavTaskAnnotation(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax) {

            if(semanticModel == null || classDeclarationSyntax == null) {
                return null;
            }

            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            var navTaskInfo = ReadNavTaskAnnotation(classDeclarationSyntax, classSymbol);
           
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotation(ClassDeclarationSyntax classDeclarationSyntax, INamedTypeSymbol classSymbol) {

            if (classDeclarationSyntax == null || classSymbol==null) {
                return null;
            }

            var navTaskInfo = ReadNavTaskAnnotationInternal(classSymbol);

            // Nicht gefunden? Dann in der Basisklasse nachsehen...
            if (navTaskInfo == null) {
                navTaskInfo = ReadNavTaskAnnotationInternal(classSymbol.BaseType);
            }

            if(navTaskInfo != null) {
                navTaskInfo.ClassDeclarationSyntax = classDeclarationSyntax;
                return navTaskInfo;
            }
           
            return null;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotationInternal([CanBeNull] INamedTypeSymbol classSymbol) {

            // Die Klasse kann in mehrere partial classes aufgeteilt sein
            var navTaskInfo = classSymbol?.DeclaringSyntaxReferences
                                          .Select(dsr => dsr.GetSyntax() as ClassDeclarationSyntax)
                                          .Select(ReadNavTaskAnnotationInternal).FirstOrDefault(nti => nti != null);
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotationInternal(ClassDeclarationSyntax classDeclaration) {

            var tags = ReadNavTags(classDeclaration).ToList();

            var navFileTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavFile);
            var navFileName = navFileTag?.Content;

            var navTaskTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTask);
            var navTaskName = navTaskTag?.Content;

            // Dateiname und Taskname müssen immer paarweise vorhanden sein
            if (String.IsNullOrWhiteSpace(navFileName) || String.IsNullOrWhiteSpace(navTaskName)) {
                return null;
            }

            // Relative Pfadangaben in absolute auflösen
            if (!Path.IsPathRooted(navFileName)) {
                var declaringNodePath = classDeclaration.GetLocation().GetLineSpan().Path;

                if (String.IsNullOrWhiteSpace(declaringNodePath)) {
                    return null;
                }

                var declaringDir = Path.GetDirectoryName(declaringNodePath);
                if (declaringDir == null) {
                    return null;
                }

                navFileName = Path.GetFullPath(Path.Combine(declaringDir, navFileName));
            }

            var candidate = new NavTaskAnnotation {
                NavFileName = navFileName,
                TaskName = navTaskName
            };

            return candidate;
        }

        #endregion

        #region ReadMethodAnnotations

        public static IEnumerable<NavMethodAnnotation> ReadMethodAnnotations(SemanticModel semanticModel, NavTaskAnnotation navTaskAnnotation, IEnumerable<MethodDeclarationSyntax> methodDeclarations) {

            foreach (var methodDeclaration in methodDeclarations) {

                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
                if(methodSymbol == null) {
                    continue;
                }

                // Init Annotation
                var initAnnotation = ReadNavInitAnnotation(navTaskAnnotation, methodSymbol);
                if(initAnnotation != null) {
                    initAnnotation.MethodDeclarationSyntax = methodDeclaration;
                    yield return initAnnotation;
                }

                // Exit Annotation
                var navExitAnnotation = ReadNavExitAnnotation(navTaskAnnotation, methodSymbol);
                if (navExitAnnotation != null) {
                    navExitAnnotation.MethodDeclarationSyntax = methodDeclaration;
                    yield return navExitAnnotation;
                }

                // Trigger Annotation
                var triggerAnnotation = ReadNavTriggerAnnotation(navTaskAnnotation, methodSymbol);
                if (triggerAnnotation != null) {
                    triggerAnnotation.MethodDeclarationSyntax = methodDeclaration;
                    yield return triggerAnnotation;
                }               
            }
        }

        #endregion

        #region ReadInitCallAnnotation

        public static IEnumerable<NavInitCallAnnotation> ReadInitCallAnnotation(SemanticModel semanticModel, NavTaskAnnotation navTaskAnnotation, IEnumerable<InvocationExpressionSyntax> invocationExpressions) {

            if (semanticModel == null || navTaskAnnotation == null) {
                yield break;
            }

            foreach (var invocationExpression in invocationExpressions) {
                var identifier = invocationExpression.Expression as IdentifierNameSyntax;
                if (identifier == null) {
                    continue;
                }

                var methodSymbol = semanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;
                if (methodSymbol == null) {
                    continue;
                }

                var declaringMethodNode = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
                var navInitCallTag = ReadNavTags(declaringMethodNode).FirstOrDefault(tag => tag.TagName == AnnotationTagNames.NavInitCall);
                if (navInitCallTag == null) {
                    continue;
                }

                var callAnnotation = new NavInitCallAnnotation {
                    NavFileName = navTaskAnnotation.NavFileName,
                    TaskName    = navTaskAnnotation.TaskName,
                    Identifier  = identifier,
                    BeginItfFullyQualifiedName = navInitCallTag.Content,
                    Parameter   = ToParameterTypeList(methodSymbol.Parameters)
                };

                yield return callAnnotation;
            }
        }

        #endregion

        #region ReadNavTriggerAnnotation

        [CanBeNull]
        public static NavTriggerAnnotation ReadNavTriggerAnnotation([NotNull] NavTaskAnnotation navTaskAnnotation, [CanBeNull] IMethodSymbol methodSymbol) {

            if (navTaskAnnotation == null) {
                throw new ArgumentNullException(nameof(navTaskAnnotation));
            }

            var triggerAnnotation = ReadNavTriggerAnnotationInternal(navTaskAnnotation, methodSymbol);
            // In der überschriebenen Methode nachsehen
            if (triggerAnnotation == null) {
                triggerAnnotation = ReadNavTriggerAnnotationInternal(navTaskAnnotation, methodSymbol?.OverriddenMethod);
            }
            return triggerAnnotation;
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotationInternal([NotNull] NavTaskAnnotation navTaskAnnotation, [CanBeNull] IMethodSymbol method) {

            var triggerAnnotation = method?.DeclaringSyntaxReferences
                                           .Select(dsr => dsr.GetSyntax())
                                           .OfType<MethodDeclarationSyntax>()
                                           .Select(syntax => ReadNavTriggerAnnotationInternal(navTaskAnnotation, syntax))
                                           .FirstOrDefault(nti => nti != null);

            return triggerAnnotation;
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotationInternal([NotNull] NavTaskAnnotation navTaskAnnotation, [NotNull] MethodDeclarationSyntax methodDeclaration) {

            var tags           = ReadNavTags(methodDeclaration);
            var navTriggerTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTrigger);
            var navTriggerName = navTriggerTag?.Content;

            if (String.IsNullOrEmpty(navTriggerName)) {
                return null;
            }

            return new NavTriggerAnnotation {
                NavFileName = navTaskAnnotation.NavFileName,
                TaskName    = navTaskAnnotation.TaskName,
                TriggerName = navTriggerName
            };
        }

        #endregion 

        #region ReadNavInitAnnotation

        [CanBeNull]
        public static NavInitAnnotation ReadNavInitAnnotation([NotNull] NavTaskAnnotation navTaskAnnotation, [CanBeNull] IMethodSymbol methodSymbol) {

            if(navTaskAnnotation == null) {
                throw new ArgumentNullException(nameof(navTaskAnnotation));
            }

            var initAnnotation = ReadNavInitAnnotationInternal(navTaskAnnotation, methodSymbol);
            // In der überschriebenen Methode nachsehen
            if (initAnnotation == null) {
                initAnnotation = ReadNavInitAnnotationInternal(navTaskAnnotation, methodSymbol?.OverriddenMethod);
            }
            return initAnnotation;
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotationInternal([NotNull] NavTaskAnnotation taskAnnotation, [CanBeNull] IMethodSymbol method) {

            var initAnnotation = method?.DeclaringSyntaxReferences
                                        .Select(dsr => dsr.GetSyntax())
                                        .OfType<MethodDeclarationSyntax>()
                                        .Select(syntax => ReadNavInitAnnotationInternal(taskAnnotation, syntax))
                                        .FirstOrDefault(nti => nti != null);

            return initAnnotation;
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotationInternal([NotNull] NavTaskAnnotation taskAnnotation, [NotNull] MethodDeclarationSyntax methodDeclaration) {

            var tags        = ReadNavTags(methodDeclaration);
            var navInitTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavInit);
            var navInitName = navInitTag?.Content;

            if (String.IsNullOrEmpty(navInitName)) {
                return null;
            }

            return new NavInitAnnotation {
                NavFileName = taskAnnotation.NavFileName,
                TaskName    = taskAnnotation.TaskName,
                InitName    = navInitName
            };
        }

        #endregion

        #region ReadNavExitAnnotation

        [CanBeNull]
        public static NavExitAnnotation ReadNavExitAnnotation([NotNull] NavTaskAnnotation navTaskAnnotation, [CanBeNull] IMethodSymbol methodSymbol) {

            if (navTaskAnnotation == null) {
                throw new ArgumentNullException(nameof(navTaskAnnotation));
            }
            
            var navExitAnnotation = ReadNavExitAnnotationInternal(navTaskAnnotation, methodSymbol);
            // In der überschriebenen Methode nachsehen
            if (navExitAnnotation == null) {
                navExitAnnotation = ReadNavExitAnnotationInternal(navTaskAnnotation, methodSymbol?.OverriddenMethod);
            }

            return navExitAnnotation;
        }

        [CanBeNull]
        static NavExitAnnotation ReadNavExitAnnotationInternal([NotNull] NavTaskAnnotation taskAnnotation, [CanBeNull] IMethodSymbol methodSymbol) {

            var navExitAnnotation = methodSymbol?.DeclaringSyntaxReferences
                                                 .Select(dsr => dsr.GetSyntax())
                                                 .OfType<MethodDeclarationSyntax>()
                                                 .Select(syntax => ReadNavExitAnnotationInternal(taskAnnotation, syntax))
                                                 .FirstOrDefault(nti => nti != null);

            return navExitAnnotation;
        }

        [CanBeNull]
        static NavExitAnnotation ReadNavExitAnnotationInternal([NotNull] NavTaskAnnotation taskAnnotation, [NotNull] MethodDeclarationSyntax methodDeclaration) {

            var tags        = ReadNavTags(methodDeclaration);
            var navExitTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavExit);
            var navExitName = navExitTag?.Content;

            if (String.IsNullOrEmpty(navExitName)) {
                return null;
            }

            return new NavExitAnnotation {
                NavFileName  = taskAnnotation.NavFileName,
                TaskName     = taskAnnotation.TaskName,
                ExitTaskName = navExitName
            };
        }

        #endregion

        [NotNull]
        static IEnumerable<NavTag> ReadNavTags(Microsoft.CodeAnalysis.SyntaxNode node) {

            if (node == null) {
                yield break;
            }

            var trivias = node.GetLeadingTrivia()
                              .Where(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            foreach (var trivia in trivias) {

                if (!trivia.HasStructure) {
                    continue;
                }

                var xmlElementSyntaxes = trivia.GetStructure()
                                               .ChildNodes()
                                               .OfType<XmlElementSyntax>()
                                               .ToList();

                // Wir suchen alle Tags, deren Namen mit Nav beginnen
                foreach (var xmlElementSyntax in xmlElementSyntaxes) {
                    var startTagName = xmlElementSyntax.StartTag?.Name?.ToString();
                    if (startTagName?.StartsWith(AnnotationTagNames.TagPrefix) == true) {
                        yield return new NavTag {
                            TagName = startTagName,
                            Content = xmlElementSyntax.Content.ToString()
                        };
                    }
                }
            }
        }

        public static List<string> ToParameterTypeList(IEnumerable<IParameterSymbol> beginLogicParameter) {
            return beginLogicParameter.OrderBy(p => p.Ordinal).Select(p => p.ToDisplayString()).ToList();
        }
    }
}
