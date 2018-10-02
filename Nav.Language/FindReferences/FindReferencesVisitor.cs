#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    class FindReferencesVisitor: SymbolVisitor<Task> {

        readonly FindReferencesArgs _args;

        FindReferencesVisitor(FindReferencesArgs args) {
            _args = args;

        }

        private IFindReferencesContext Context => _args.Context;

        public static Task Invoke(FindReferencesArgs args) {
            var finder = new FindReferencesVisitor(args);

            return finder.Visit(args.OriginatingSymbol);
        }

        protected override Task DefaultVisit(ISymbol symbol) {
            return Task.CompletedTask;
        }

        #region Task Declaration

        public override async Task VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {

            var initReferences = initConnectionPointSymbol.TaskDeclaration
                                                          .References
                                                          .SelectMany(tn => tn.Incomings)
                                                          .Select(edge => edge.TargetReference)
                                                          .Where(nodeRef => nodeRef != null)
                                                          .OrderByLocation();

            var definitionItem = CreateInitConnectionPointDefinition(initConnectionPointSymbol, expandedByDefault: true);

            await Context.OnDefinitionFoundAsync(definitionItem);

            foreach (var reference in initReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem);

            }
        }

        public override async Task VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            var exitReferences = exitConnectionPointSymbol.TaskDeclaration
                                                          .References
                                                          .SelectMany(tn => tn.Outgoings)
                                                          .Select(exitTrans => exitTrans.ExitConnectionPointReference)
                                                          .Where(ep => ep?.Declaration == exitConnectionPointSymbol);

            var definitionItem = CreateExitConnectionPointDefinition(exitConnectionPointSymbol, expandedByDefault: true);

            await Context.OnDefinitionFoundAsync(definitionItem);

            foreach (var reference in exitReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem);

            }

        }

        public override Task VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
            // Hat keine Referenzen...
            return Task.CompletedTask;
        }

        public override async Task VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclaration) {

            var taskrefDefinition              = CreateTaskDeclarationItem(taskDeclaration);
            var initConnectionPointDefinition  = CreateInitConnectionPointDefinition(taskDeclaration, expandedByDefault: false);
            var exitConnectionPointDefinitions = CreateExitConnectionPointDefinitions(taskDeclaration, expandedByDefault: false);

            // Auch wenn wir keine Referenzen auf den Task finden sollten, soll zumindest
            // Ein Eintrag "No References found..." für erscheinen.
            await Context.OnDefinitionFoundAsync(taskrefDefinition);

            await FindReferencesAsync(taskDeclaration,
                                      taskDeclaration.CodeGenerationUnit,
                                      taskrefDefinition,
                                      initConnectionPointDefinition,
                                      exitConnectionPointDefinitions,
                                      Context);

        }

        #endregion

        public override async Task VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinition) {

            var nodeDefinition                 = CreateTaskDefinitionItem(taskDefinition);
            var initConnectionPointDefinition  = CreateInitConnectionPointDefinition(taskDefinition, expandedByDefault: false);
            var exitConnectionPointDefinitions = CreateExitConnectionPointDefinitions(taskDefinition, expandedByDefault: false);

            // Auch wenn wir keine Referenzen auf den Task finden sollten, soll zumindest
            // Ein Eintrag "No References found..." erscheinen.
            await Context.OnDefinitionFoundAsync(nodeDefinition);

            await _args.Solution.ProcessCodeGenerationUnitsAsync(
                codeGenerationUnit => FindReferencesAsync(taskDefinition.AsTaskDeclaration,
                                                          codeGenerationUnit,
                                                          nodeDefinition,
                                                          initConnectionPointDefinition,
                                                          exitConnectionPointDefinitions,
                                                          Context),
                _args.OriginatingCodeGenerationUnit, Context.CancellationToken);

        }

        // TODO find taskref "Pfad zum file"?

        // WICHTIG: Diese Methode muss Thread safe sein!
        static async Task FindReferencesAsync(ITaskDeclarationSymbol taskDeclaration,
                                              CodeGenerationUnit codeGenerationUnit,
                                              DefinitionItem taskDefinitionItem,
                                              DefinitionItem initConnectionPointDefinitionItem,
                                              ImmutableDictionary<Location, DefinitionItem> exitConnectionPointDefinitionsItems,
                                              IFindReferencesContext context) {

            var taskNodeReferences = FindTaskNodeReferences(taskDeclaration, codeGenerationUnit, taskDefinitionItem);
            var initNodeReferences = FindInitNodeReferences(taskDeclaration, codeGenerationUnit, initConnectionPointDefinitionItem);
            var exitNodeReferences = FindExitNodeReferences(taskDeclaration, codeGenerationUnit, exitConnectionPointDefinitionsItems);

            var referenceItems = taskNodeReferences.Concat(initNodeReferences).Concat(exitNodeReferences).OrderByLocation();

            foreach (var referenceItem in referenceItems) {

                if (context.CancellationToken.IsCancellationRequested) {
                    break;
                }

                await context.OnReferenceFoundAsync(referenceItem);
            }

            // Taskrefs aufsammeln wäre schön, ist aber komplett unvollständig, und praktisch unmöglich
            //foreach (var taskDeclaration in codeGeneration.TaskDeclarations
            //                                              .Where(td => td.Origin == TaskDeclarationOrigin.TaskDeclaration)) {

            //    if (taskDeclaration.Name          == TaskDefinition.Name &&
            //        taskDeclaration.CodeNamespace == TaskDefinition.CodeNamespace) {
            //        yield return taskDeclaration;
            //    }

            //}

        }

        static IEnumerable<ReferenceItem> FindTaskNodeReferences(ITaskDeclarationSymbol taskDeclaration,
                                                                 CodeGenerationUnit codeGenerationUnit,
                                                                 DefinitionItem taskDefinitionItem) {

            if (taskDefinitionItem == null) {
                yield break;
            }

            foreach (var task in codeGenerationUnit.TaskDefinitions.OrderByLocation()) {

                foreach (var taskNode in task.NodeDeclarations
                                             .OfType<ITaskNodeSymbol>()
                                             .Where(taskNode => taskNode.Declaration?.Location == taskDeclaration.Location)
                                             .OrderByLocation()) {

                    var referenceItem = ReferenceItemBuilder.Invoke(taskDefinitionItem, taskNode);
                    yield return referenceItem;
                }
            }
        }

        static IEnumerable<ReferenceItem> FindInitNodeReferences(ITaskDeclarationSymbol taskDeclaration,
                                                                 CodeGenerationUnit codeGenerationUnit,
                                                                 DefinitionItem initConnectionPointDefinitionItem) {

            if (initConnectionPointDefinitionItem == null) {
                yield break;
            }

            // Init Calls aufsammeln 
            foreach (var task in codeGenerationUnit.TaskDefinitions.OrderByLocation()) {

                foreach (var taskNode in task.NodeDeclarations
                                             .OfType<ITaskNodeSymbol>()
                                             .Where(taskNode => taskNode.Declaration?.Location == taskDeclaration.Location)
                                             .OrderByLocation()) {

                    foreach (var targetReference in taskNode.Incomings
                                                            .Select(edge => edge.TargetReference)
                                                            .Where(targetReference => targetReference != null)
                                                            .OrderByLocation()) {

                        var initReference = ReferenceItemBuilder.Invoke(initConnectionPointDefinitionItem, targetReference);

                        yield return initReference;
                    }
                }
            }
        }

        static IEnumerable<ReferenceItem> FindExitNodeReferences(ITaskDeclarationSymbol taskDeclaration,
                                                                 CodeGenerationUnit codeGenerationUnit,
                                                                 ImmutableDictionary<Location, DefinitionItem> exitConnectionPointDefinitionsItems) {

            if (!exitConnectionPointDefinitionsItems.Any()) {
                yield break;
            }

            // Exits in Exit Transitions aufsammeln
            foreach (var task in codeGenerationUnit.TaskDefinitions.OrderByLocation()) {

                foreach (var taskNode in task.NodeDeclarations
                                             .OfType<ITaskNodeSymbol>()
                                             .Where(taskNode => taskNode.Declaration?.Location == taskDeclaration.Location)
                                             .OrderByLocation()) {

                    foreach (var exitConnectionPointReference in taskNode.Outgoings
                                                                         .Select(edge => edge.ExitConnectionPointReference)
                                                                         .Where(exitConnectionPointReference => exitConnectionPointReference != null)
                                                                         .OrderByLocation()) {

                        var exitConnectionPoint = exitConnectionPointReference?.Declaration;
                        if (exitConnectionPoint == null) {
                            continue;
                        }

                        if (exitConnectionPointDefinitionsItems.TryGetValue(exitConnectionPoint.Location, out var exitConnectionPointDefinition)) {
                            var exitReference = ReferenceItemBuilder.Invoke(exitConnectionPointDefinition, exitConnectionPointReference);

                            yield return exitReference;
                        }

                    }

                }
            }
        }

        #region Nodes

        public override Task VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return VisitInitNodeSymbol(initNodeAliasSymbol.InitNode);
        }

        public override async Task VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            var initReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                 .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                initNodeSymbol,
                initNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in initReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {
                foreach (var transition in initNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }
            }
        }

        public override async Task VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            var exitReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                 .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                exitNodeSymbol,
                exitNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in exitReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {
                foreach (var edge in exitNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override async Task VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            var endReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                endNodeSymbol,
                endNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in endReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {
                foreach (var edge in endNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override Task VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return Visit(taskNodeAliasSymbol.TaskNode);
        }

        public override async Task VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            var taskReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                 .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                taskNodeSymbol,
                taskNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in taskReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {

                foreach (var exitTransition in taskNodeSymbol.Outgoings) {
                    yield return exitTransition.SourceReference;
                }

                foreach (var edge in taskNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override async Task VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            var dialogReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                   .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                dialogNodeSymbol,
                dialogNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in dialogReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in dialogNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in dialogNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override async Task VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            var viewReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                 .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                viewNodeSymbol,
                viewNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in viewReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in viewNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in viewNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        public override async Task VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            var viewReferences = FindReferences().Where(nodeRef => nodeRef != null)
                                                 .OrderByLocation();

            var definitionItem = new DefinitionItem(
                GetSolutionRoot(),
                choiceNodeSymbol,
                choiceNodeSymbol.ToDisplayParts());

            await Context.OnDefinitionFoundAsync(definitionItem).ConfigureAwait(false);

            foreach (var reference in viewReferences) {

                var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);
                await Context.OnReferenceFoundAsync(referenceItem).ConfigureAwait(false);

            }

            IEnumerable<ISymbol> FindReferences() {

                foreach (var transition in choiceNodeSymbol.Outgoings) {
                    yield return transition.SourceReference;
                }

                foreach (var edge in choiceNodeSymbol.Incomings) {
                    yield return edge.TargetReference;
                }
            }
        }

        #endregion

        #region Node References

        public override Task VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration != null) {
                return Visit(nodeReferenceSymbol.Declaration);
            }

            return DefaultVisit(nodeReferenceSymbol);
        }

        public override Task VisitExitConnectionPointReferenceSymbol(IExitConnectionPointReferenceSymbol exitConnectionPointReferenceSymbol) {
            if (exitConnectionPointReferenceSymbol.Declaration != null) {
                return Visit(exitConnectionPointReferenceSymbol.Declaration);
            }

            return DefaultVisit(exitConnectionPointReferenceSymbol);
        }

        #endregion

        string GetSolutionRoot() {
            return _args.Solution.SolutionDirectory?.FullName ?? "Miscellaneous Files";
        }

        private const string TaskDeclarationSortKey     = "a";
        private const string InitConnectionPointSortKey = "b";
        private const string ExitConnectionPointSortKey = "c";

        DefinitionItem CreateTaskDefinitionItem(ITaskDefinitionSymbol taskDefinition) {
            return new DefinitionItem(
                GetSolutionRoot(),
                taskDefinition,
                taskDefinition.ToDisplayParts(),
                sortKey: TaskDeclarationSortKey);
        }

        DefinitionItem CreateTaskDeclarationItem(ITaskDeclarationSymbol taskDeclaration) {
            return new DefinitionItem(
                GetSolutionRoot(),
                taskDeclaration,
                taskDeclaration.ToDisplayParts(),
                sortKey: TaskDeclarationSortKey);
        }

        [CanBeNull]
        DefinitionItem CreateInitConnectionPointDefinition(ITaskDefinitionSymbol taskDefinition, bool expandedByDefault = true) {
            return CreateInitConnectionPointDefinition(taskDefinition.AsTaskDeclaration, expandedByDefault);

        }

        [CanBeNull]
        DefinitionItem CreateInitConnectionPointDefinition(ITaskDeclarationSymbol taskDeclaration, bool expandedByDefault = true) {

            var initConnectionPoint = taskDeclaration?.Inits().FirstOrDefault();
            if (initConnectionPoint == null) {
                return null;
            }

            return CreateInitConnectionPointDefinition(initConnectionPoint, expandedByDefault);

        }

        [NotNull]
        DefinitionItem CreateInitConnectionPointDefinition(IInitConnectionPointSymbol initConnectionPoint, bool expandedByDefault = true) {

            return new DefinitionItem(
                GetSolutionRoot(),
                initConnectionPoint,
                DisplayPartsBuilder.BuildInitConnectionPointSymbol(initConnectionPoint, neutralName: true),
                expandedByDefault,
                sortKey: InitConnectionPointSortKey);

        }

        ImmutableDictionary<Location, DefinitionItem> CreateExitConnectionPointDefinitions(ITaskDefinitionSymbol taskDefinition, bool expandedByDefault = true) {
            return CreateExitConnectionPointDefinitions(taskDefinition.AsTaskDeclaration, expandedByDefault);

        }

        ImmutableDictionary<Location, DefinitionItem> CreateExitConnectionPointDefinitions(ITaskDeclarationSymbol taskDeclaration, bool expandedByDefault = true) {

            var defs = ImmutableDictionary<Location, DefinitionItem>.Empty;

            if (taskDeclaration == null) {
                return defs;
            }

            foreach (var exitConnectionPoint in taskDeclaration.Exits()) {
                var exitDefinition = CreateExitConnectionPointDefinition(exitConnectionPoint, expandedByDefault);
                defs = defs.Add(exitDefinition.Location, exitDefinition);
            }

            return defs;
        }

        private DefinitionItem CreateExitConnectionPointDefinition(IConnectionPointSymbol exitConnectionPoint, bool expandedByDefault = true) {

            var exitDefinition = new DefinitionItem(
                GetSolutionRoot(),
                exitConnectionPoint,
                exitConnectionPoint.ToDisplayParts(),
                expandedByDefault,
                sortKey: ExitConnectionPointSortKey);
            return exitDefinition;
        }

    }

}