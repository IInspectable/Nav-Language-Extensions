#region Using Directives

using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    // TODO Perfomante Sortierung!
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
        public DirectoryInfo SearchDirectory => _args.SearchDirectory;

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

        public override IEnumerable<ISymbol> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var semanticModelProvider = new SemanticModelProvider(new CachedSyntaxProvider());
            // TODO Review and refactoring, Cancellation
            if (SearchDirectory != null) {

                foreach (var file in Directory.EnumerateFiles(SearchDirectory.FullName, "*.nav", SearchOption.AllDirectories)) {

                    if (Context.CancellationToken.IsCancellationRequested) {
                        break;
                    }

                    var codeGen = semanticModelProvider.GetSemanticModel(file, Context.CancellationToken);
                    if (codeGen == null) {
                        continue;
                    }

                    foreach (var taskNode in FindTaskReference(codeGen)) {
                        yield return taskNode;
                    }
                }
            } else {
                foreach (var taskNode in FindTaskReference(taskDefinitionSymbol.CodeGenerationUnit)) {
                    yield return taskNode;
                }
            }

        }

        IEnumerable<ISymbol> FindTaskReference(CodeGenerationUnit codeGeneration) {

            foreach (var taskDefinition in codeGeneration.TaskDefinitions) {

                foreach (var taskNode in FindTaskReference(taskDefinition)) {
                    if (Context.CancellationToken.IsCancellationRequested) {
                        break;
                    }

                    yield return taskNode;
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

        IEnumerable<ISymbol> FindTaskReference(ITaskDefinitionSymbol taskDefinition) {

            return taskDefinition.NodeDeclarations
                                 .OfType<ITaskNodeSymbol>()
                                 .Where(tn => tn.Declaration?.Location == Definition.Symbol.Location);
        }

        public override IEnumerable<ISymbol> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            foreach (var transition in initNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }
        }

        public override IEnumerable<ISymbol> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override IEnumerable<ISymbol> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            foreach (var exitTransition in taskNodeSymbol.Outgoings) {
                yield return exitTransition.SourceReference;
            }

            foreach (var edge in taskNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            foreach (var edge in exitNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            foreach (var edge in endNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            foreach (var transition in dialogNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in dialogNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            foreach (var transition in viewNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in viewNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            foreach (var transition in choiceNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in choiceNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            foreach (var taskNode in taskDeclarationSymbol.References) {
                yield return taskNode;
            }
        }

    }

}