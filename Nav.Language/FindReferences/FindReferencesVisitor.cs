#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    sealed class FindReferencesVisitor: SymbolVisitor<IEnumerable<ISymbol>> {

        public DefinitionItem Definition { get; }

        FindReferencesVisitor(FindReferencesArgs args, DefinitionItem definition) {
            _args      = args;
            Definition = definition;

        }

        [NotNull]
        readonly FindReferencesArgs _args;

        [NotNull]
        public IFindReferencesContext Context => _args.Context;

        [CanBeNull]
        public DirectoryInfo SearchDirectory => _args.SolutionRoot;

        public static IEnumerable<ISymbol> Invoke(FindReferencesArgs args, DefinitionItem definition) {

            if (definition?.Symbol == null) {
                return Enumerable.Empty<ISymbol>();
            }

            var finder = new FindReferencesVisitor(args, definition);
            return finder.Visit(definition.Symbol);
        }

        protected override IEnumerable<ISymbol> DefaultVisit(ISymbol symbol) {
            yield break;
        }

        public override IEnumerable<ISymbol> VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {

            return initConnectionPointSymbol.TaskDeclaration
                                            .References
                                            .SelectMany(tn => tn.Incomings)
                                            .Select(edge => edge.TargetReference);
        }

        public override IEnumerable<ISymbol> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {
            return exitConnectionPointSymbol.TaskDeclaration
                                            .References
                                            .SelectMany(tn => tn.Outgoings)
                                            .Select(exitTrans => exitTrans.ExitConnectionPointReference)
                                            .Where(ep => ep?.Declaration == exitConnectionPointSymbol);
        }

        public override IEnumerable<ISymbol> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
            // Hat keine Referenzen...
            yield break;
        }

        public override IEnumerable<ISymbol> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            return FindAllReferences(taskDefinitionSymbol.CodeGenerationUnit, FindAllTaskReferences);

        }

        IEnumerable<ISymbol> FindAllTaskReferences(CodeGenerationUnit codeGeneration) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {

                foreach (var taskDefinition in codeGeneration.TaskDefinitions) {

                    foreach (var taskNode in FindAllTaskReferences(taskDefinition)) {
                        if (Context.CancellationToken.IsCancellationRequested) {
                            break;
                        }

                        yield return taskNode;

                        foreach (var nodeReference in taskNode.References) {
                            yield return nodeReference;
                        }
                    }
                }

                var taskDefinitionSymbol = Definition.Symbol as ITaskDefinitionSymbol;
                if (taskDefinitionSymbol == null) {
                    yield break;
                }

                foreach (var taskDeclaration in codeGeneration.TaskDeclarations
                                                              .Where(td => td.Origin == TaskDeclarationOrigin.TaskDeclaration)) {

                    if (taskDeclaration.Name          == Definition.Symbol.Name &&
                        taskDeclaration.CodeNamespace == taskDefinitionSymbol.CodeNamespace) {
                        yield return taskDeclaration;
                    }

                }
            }

            IEnumerable<ITaskNodeSymbol> FindAllTaskReferences(ITaskDefinitionSymbol taskDefinition) {

                return taskDefinition.NodeDeclarations
                                     .OfType<ITaskNodeSymbol>()
                                     .Where(tn => tn.Declaration?.Location == Definition.Symbol.Location);
            }
        }

        public override IEnumerable<ISymbol> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {
                foreach (var transition in initNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override IEnumerable<ISymbol> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {

                foreach (var exitTransition in taskNodeSymbol.Outgoings) {
                    yield return exitTransition.SourceReference;
                }

                foreach (var edge in taskNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {
                foreach (var edge in exitNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {
                foreach (var edge in endNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in dialogNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in dialogNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in viewNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in viewNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in choiceNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in choiceNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override IEnumerable<ISymbol> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            return OrderSymbols(FindReferences());

            IEnumerable<ISymbol> FindReferences() {
                foreach (var taskNode in taskDeclarationSymbol.References) {
                    yield return taskNode;
                }
            }

        }

        static IOrderedEnumerable<ISymbol> OrderSymbols(IEnumerable<ISymbol> symbols) {
            return symbols.OrderBy(s => s.Location.StartLine).ThenBy(s => s.Location.StartCharacter);

        }

        IEnumerable<ISymbol> FindAllReferences(CodeGenerationUnit definitionUnit,
                                               Func<CodeGenerationUnit, IEnumerable<ISymbol>> findReferences) {

            CancellationToken cancellationToken = Context.CancellationToken;

            // TODO Review and refactoring, Cancellation
            var semanticModelProvider = new SemanticModelProvider(new CachedSyntaxProvider());

            var seenFiles = new HashSet<string>();
            var navFile   = definitionUnit.Syntax.SyntaxTree.SourceText.FileInfo;
            var navDir    = navFile?.Directory;

            // 1. In dem File anfangen, in dem sich auch die Definition befindet, deren Referenzen gesucht werden
            foreach (var reference in findReferences(definitionUnit)) {
                yield return reference;
            }

            if (navFile != null) {
                // Wenn das Definitionsfile einen Dateinamen hat, dann zu den bereits gesehenen hinzufügen.
                seenFiles.Add(navFile.FullName);
            }

            // 2. Wir suchen in dem Verzeichnis, in dem sich auch das Nav File der Definition befindet. Die Wahscheinlichkeit ist recht groß,
            //    dass hier bereits erste Treffer ermittelt werden.
            if (navDir != null) {

                foreach (var fileName in Directory.EnumerateFiles(navDir.FullName, "*.nav")) {

                    if (cancellationToken.IsCancellationRequested) {
                        break;
                    }

                    foreach (var symbol in FindAllReferences(fileName)) {
                        yield return symbol;
                    }
                }

            }

            // 3. Zu guter letzt durchsuchen wir alle übrigen Files de "Solution", was mittlerweil ~1400 Dateien sind, und
            //    entsprechend lange dauert.
            foreach (var file in _args.SolutionFiles) {

                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                foreach (var taskNode in FindAllReferences(file.FullName)) {
                    yield return taskNode;
                }
            }

            IEnumerable<ISymbol> FindAllReferences(string fileName) {

                if (!seenFiles.Add(fileName)) {
                    yield break;
                }

                var codeGen = semanticModelProvider.GetSemanticModel(fileName, cancellationToken);
                if (codeGen == null) {
                    yield break;
                }

                foreach (var taskNode in findReferences(codeGen)) {
                    yield return taskNode;
                }
            }
        }

    }

}