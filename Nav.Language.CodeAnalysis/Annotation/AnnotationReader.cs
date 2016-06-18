using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pharmatechnik.Nav.Language.CodeGen;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    // Würde ich gerne wieder internal machen
    public sealed class NavTag {
        public string TagName { get; set; }
        public string Content { get; set; }
    }

    public static class AnnotationReader {

        #region ReadNavTaskAnnotation

        [CanBeNull]
        public static NavTaskAnnotation ReadNavTaskAnnotation(INamedTypeSymbol classSymbol) {

            var navTaskInfo = ReadNavTaskAnnotationInternal(classSymbol);

            // In der Basisklasse nachsehen
            if (navTaskInfo == null) {
                navTaskInfo = ReadNavTaskAnnotationInternal(classSymbol?.BaseType);
            }
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotationInternal(INamedTypeSymbol classSymbol) {

            // Die Klasse kann in mehrere partial classes aufgeteilt sein
            var navTaskInfo = classSymbol?.DeclaringSyntaxReferences
                                          .Select(dsr => dsr.GetSyntax() as ClassDeclarationSyntax)
                                          .Select(ReadNavTaskAnnotationInternal).FirstOrDefault(nti => nti != null);
            return navTaskInfo;
        }

        [CanBeNull]
        static NavTaskAnnotation ReadNavTaskAnnotationInternal(ClassDeclarationSyntax classDeclaration) {

            var tags = ReadNavTags(classDeclaration).ToList();

            var navFileTag = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavFile);
            var navFileName = navFileTag?.Content;

            var navTaskTag = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTask);
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

        #region ReadNavTriggerAnnotation

        [CanBeNull]
        public static NavTriggerAnnotation ReadNavTriggerAnnotation(IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation) {

            var triggerAnnotation = ReadNavTriggerAnnotationInternal(methodSymbol, navTaskAnnotation);
            // In der überschriebenen Methode nachsehen
            if (triggerAnnotation == null) {
                triggerAnnotation = ReadNavTriggerAnnotationInternal(methodSymbol?.OverriddenMethod, navTaskAnnotation);
            }
            return triggerAnnotation;
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotationInternal(IMethodSymbol method, NavTaskAnnotation taskAnnotation) {

            var triggerAnnotation = method?.DeclaringSyntaxReferences
                                           .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                           .Select(syntax => ReadNavTriggerAnnotationInternal(syntax, taskAnnotation))
                                           .FirstOrDefault(nti => nti != null);

            return triggerAnnotation;
        }

        [CanBeNull]
        static NavTriggerAnnotation ReadNavTriggerAnnotationInternal(MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation taskAnnotation) {

            var tags           = ReadNavTags(methodDeclaration);
            var navTriggerTag  = tags.FirstOrDefault(t => t.TagName == AnnotationTagNames.NavTrigger);
            var navTriggerName = navTriggerTag?.Content;

            if (String.IsNullOrEmpty(navTriggerName)) {
                return null;
            }

            return new NavTriggerAnnotation {
                NavFileName = taskAnnotation.NavFileName,
                TaskName    = taskAnnotation.TaskName,
                TriggerName = navTriggerName
            };
        }

        #endregion 

        #region ReadNavInitAnnotation

        [CanBeNull]
        public static NavInitAnnotation ReadNavInitAnnotation(IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation) {

            var initAnnotation = ReadNavInitAnnotationInternal(methodSymbol, navTaskAnnotation);
            // In der überschriebenen Methode nachsehen
            if (initAnnotation == null) {
                initAnnotation = ReadNavInitAnnotationInternal(methodSymbol?.OverriddenMethod, navTaskAnnotation);
            }
            return initAnnotation;
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotationInternal(IMethodSymbol method, NavTaskAnnotation taskAnnotation) {

            var initAnnotation = method?.DeclaringSyntaxReferences
                                        .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                        .Select(syntax => ReadNavInitAnnotationInternal(syntax, taskAnnotation))
                                        .FirstOrDefault(nti => nti != null);

            return initAnnotation;
        }

        [CanBeNull]
        static NavInitAnnotation ReadNavInitAnnotationInternal(MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation taskAnnotation) {

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
        public static NavExitAnnotation ReadNavExitAnnotation(IMethodSymbol methodSymbol, NavTaskAnnotation navTaskAnnotation) {

            if(methodSymbol == null || navTaskAnnotation == null) {
                return null;
            }
            
            var navExitAnnotation = ReadNavExitAnnotationInternal(methodSymbol, navTaskAnnotation);
            // In der überschriebenen Methode nachsehen
            if (navExitAnnotation == null) {
                navExitAnnotation = ReadNavExitAnnotationInternal(methodSymbol.OverriddenMethod, navTaskAnnotation);
            }

            return navExitAnnotation;
        }

        [CanBeNull]
        static NavExitAnnotation ReadNavExitAnnotationInternal(IMethodSymbol methodSymbol, NavTaskAnnotation taskAnnotation) {

            var navExitAnnotation = methodSymbol?.DeclaringSyntaxReferences
                                                 .Select(dsr => dsr.GetSyntax() as MethodDeclarationSyntax)
                                                 .Select(syntax => ReadNavExitAnnotationInternal(syntax, taskAnnotation))
                                                 .FirstOrDefault(nti => nti != null);

            return navExitAnnotation;
        }

        [CanBeNull]
        static NavExitAnnotation ReadNavExitAnnotationInternal(MethodDeclarationSyntax methodDeclaration, NavTaskAnnotation taskAnnotation) {

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
        public static IEnumerable<NavTag> ReadNavTags(Microsoft.CodeAnalysis.SyntaxNode node) {

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
    }
}
