#region Using Directives

using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.SemanticAnalyzer;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class TaskDefinitionBuilderResult {

        public TaskDefinitionBuilderResult(TaskDefinitionSymbol taskDefinition, IReadOnlyList<Diagnostic> diagnostics) {
            TaskDefinition = taskDefinition;
            Diagnostics    = diagnostics ?? new List<Diagnostic>();
        }

        [CanBeNull]
        public TaskDefinitionSymbol TaskDefinition { get; }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }

    }

    sealed class TaskDefinitionSymbolBuilder: SyntaxNodeVisitor {

        readonly IReadOnlySymbolCollection<TaskDeclarationSymbol> _taskDeklarations;
        readonly List<Diagnostic>                                 _diagnostics;

        TaskDefinitionSymbol _taskDefinition;

        TaskDefinitionSymbolBuilder(IReadOnlySymbolCollection<TaskDeclarationSymbol> taskDeklarations) {
            _taskDeklarations = taskDeklarations;
            _diagnostics      = new List<Diagnostic>();
        }

        public override void VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {
            var identifier = taskDefinitionSyntax.Identifier;
            var location   = identifier.GetLocation();

            if (location == null) {
                return;
            }

            var taskName = identifier.ToString();

            var taskDeclaration = _taskDeklarations.TryFindSymbol(taskName);
            if (taskDeclaration.Location != location) {
                taskDeclaration = null;
            }

            _taskDefinition = new TaskDefinitionSymbol(taskName, location, taskDefinitionSyntax, taskDeclaration);

            // Declarations
            foreach (var nodeDeclarationSyntax in taskDefinitionSyntax.NodeDeclarationBlock?.NodeDeclarations ?? Enumerable.Empty<NodeDeclarationSyntax>()) {
                Visit(nodeDeclarationSyntax);
            }

            // Transitions
            foreach (var transitionDefinitionSyntax in taskDefinitionSyntax.TransitionDefinitionBlock?.TransitionDefinitions ?? Enumerable.Empty<TransitionDefinitionSyntax>()) {
                Visit(transitionDefinitionSyntax);
            }

            // ExitTransitions
            foreach (var transitionDefinitionSyntax in taskDefinitionSyntax.TransitionDefinitionBlock?.ExitTransitionDefinitions ?? Enumerable.Empty<ExitTransitionDefinitionSyntax>()) {
                Visit(transitionDefinitionSyntax);
            }

            AnalyzeTaskDefinition();
        }

        #region Node Declarations

        public override void VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) {

            var identifier = initNodeDeclarationSyntax.InitKeyword;
            var taskAlias  = initNodeDeclarationSyntax.Identifier;

            InitNodeAliasSymbol initNodeAlias = null;
            if (!taskAlias.IsMissing) {
                var aliasName     = taskAlias.ToString();
                var aliasLocation = taskAlias.GetLocation();
                initNodeAlias = new InitNodeAliasSymbol(aliasName, aliasLocation);
            }

            var location = identifier.GetLocation();
            if (location == null) {
                return;
            }

            var decl = new InitNodeSymbol(SyntaxFacts.InitKeywordAlt, location, initNodeDeclarationSyntax, initNodeAlias, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        public override void VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) {

            var identifier = exitNodeDeclarationSyntax.Identifier;
            var location   = identifier.GetLocation();
            var name       = identifier.ToString();

            if (location == null) {
                return;
            }

            var decl = new ExitNodeSymbol(name, location, exitNodeDeclarationSyntax, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        public override void VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) {

            var identifier = endNodeDeclarationSyntax.EndKeyword;
            var location   = identifier.GetLocation();
            var name       = identifier.ToString();

            if (location == null) {
                return;
            }

            var decl = new EndNodeSymbol(name, location, endNodeDeclarationSyntax, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        public override void VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) {

            var taskIdentifier = taskNodeDeclarationSyntax.Identifier;
            var taskAlias      = taskNodeDeclarationSyntax.IdentifierAlias;
            var nodeIdentifier = taskAlias.IsMissing ? taskIdentifier : taskAlias;

            if (nodeIdentifier.IsMissing) {
                // Diesen Fall haben wir, wenn nur "task ;" eingegeben wird. Dafür gibt es aber bereits einen Syntax Fehler.
                return;
            }

            TaskNodeAliasSymbol taskNodeAlias = null;
            if (!taskAlias.IsMissing) {
                var aliasName     = taskAlias.ToString();
                var aliasLocation = taskAlias.GetLocation();
                taskNodeAlias = new TaskNodeAliasSymbol(aliasName, aliasLocation);
            }

            var taskName        = taskIdentifier.ToString();
            var taskLocation    = taskIdentifier.GetLocation();
            var taskDeclaration = _taskDeklarations.TryFindSymbol(taskName);

            var taskNode = new TaskNodeSymbol(taskName, taskLocation, taskNodeDeclarationSyntax, taskNodeAlias, taskDeclaration, _taskDefinition);

            if (taskNode.Declaration == null) {
                if (taskLocation != null) {
                    _diagnostics.Add(new Diagnostic(
                                         taskLocation,
                                         DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0,
                                         taskName));
                }
            } else {
                taskNode.Declaration.References.Add(taskNode);
            }

            AddNodeDeclaration(taskNode);
        }

        public override void VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) {

            var identifier = dialogNodeDeclarationSyntax.Identifier;
            var location   = identifier.GetLocation();
            var name       = identifier.ToString();

            if (location == null) {
                return;
            }

            var decl = new DialogNodeSymbol(name, location, dialogNodeDeclarationSyntax, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        public override void VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) {

            var identifier = viewNodeDeclarationSyntax.Identifier;
            var location   = identifier.GetLocation();
            var name       = identifier.ToString();

            if (location == null) {
                return;
            }

            var decl = new ViewNodeSymbol(name, location, viewNodeDeclarationSyntax, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        public override void VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) {

            var identifier = choiceNodeDeclarationSyntax.Identifier;
            var location   = identifier.GetLocation();
            var name       = identifier.ToString();

            if (location == null) {
                return;
            }

            var decl = new ChoiceNodeSymbol(name, location, choiceNodeDeclarationSyntax, _taskDefinition);

            AddNodeDeclaration(decl);
        }

        void AddNodeDeclaration(INodeSymbol nodeSymbol) {

            if (_taskDefinition.NodeDeclarations.Contains(nodeSymbol.Name)) {

                var existing = _taskDefinition.NodeDeclarations[nodeSymbol.Name];

                _diagnostics.Add(new Diagnostic(
                                     nodeSymbol.Location,
                                     existing.Location,
                                     DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared,
                                     nodeSymbol.Name));
            } else {
                _taskDefinition.NodeDeclarations.Add(nodeSymbol);
            }
        }

        #endregion

        #region Transitions

        public override void VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) {

            // Target
            // TODO entsprechend des Knotens ein ensprechendes NodeReferenceSymbol Derivat instantieren
            NodeReferenceSymbol targetNodeReference = null;
            var                 targetNodeSyntax    = transitionDefinitionSyntax.TargetNode;
            if (targetNodeSyntax != null) {

                var targetNode   = _taskDefinition.NodeDeclarations.TryFindSymbol(targetNodeSyntax.Name);
                var modeLocation = targetNodeSyntax.GetLocation();
                if (modeLocation != null) {

                    targetNodeReference = new NodeReferenceSymbol(targetNodeSyntax.Name, modeLocation, targetNode, NodeReferenceType.Target);

                    switch (targetNodeReference.Declaration) {
                        case null:
                            _diagnostics.Add(new Diagnostic(
                                                 targetNodeReference.Location,
                                                 DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0,
                                                 targetNodeReference.Name));
                            break;
                        case InitNodeSymbol _:
                            _diagnostics.Add(new Diagnostic(
                                                 targetNodeReference.Location,
                                                 DiagnosticDescriptors.Semantic.Nav0103InitNodeMustNotContainIncomingEdges));
                            break;
                    }
                }
            }

            // Edge
            EdgeModeSymbol edgeMode   = null;
            var            edgeSyntax = transitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var edgeLocation = edgeSyntax.GetLocation();
                if (edgeLocation != null) {
                    edgeMode = new EdgeModeSymbol(edgeSyntax.ToString(), edgeLocation, edgeSyntax.Mode);
                }
            }

            // Source: Ohne Source Node können wir keine Transitions hinzufügen, da dieser Knoten relevant ist für die Bestimmung
            // der Transition (Init, Choice, Trigger)

            var sourceNodeSyntax = transitionDefinitionSyntax.SourceNode;
            if (sourceNodeSyntax == null) {
                return;
            }

            var sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name);

            // Special case "init": Hier ist implizit auch Großschreibung erlaubt
            if (sourceNode == null && sourceNodeSyntax.Name == SyntaxFacts.InitKeyword) {
                sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(SyntaxFacts.InitKeywordAlt);
            }

            var location = sourceNodeSyntax.GetLocation();
            if (location == null) {
                return;
            }

            switch (sourceNode) {
                case null:
                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0,
                                         sourceNodeSyntax.Name));
                    break;
                case TaskNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.Semantic.Nav0100TaskNode0MustNotContainLeavingEdges,
                                         sourceNodeSyntax.Name));
                    break;
                case ExitNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.Semantic.Nav0101ExitNodeMustNotContainLeavingEdges));
                    break;
                case EndNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         location,
                                         DiagnosticDescriptors.Semantic.Nav0102EndNodeMustNotContainLeavingEdges));
                    break;
                case InitNodeSymbol initNode:
                    AddInitTransition(
                        transitionDefinitionSyntax: transitionDefinitionSyntax,
                        targetNodeReference       : targetNodeReference,
                        edgeMode                  : edgeMode,
                        sourceNodeSyntax          : sourceNodeSyntax,
                        location                  : location,
                        initNode                  : initNode);
                    break;
                case ChoiceNodeSymbol choiceNode  :
                    AddChoiceTransition(
                        transitionDefinitionSyntax: transitionDefinitionSyntax,
                        targetNodeReference       : targetNodeReference,
                        edgeMode                  : edgeMode,
                        sourceNodeSyntax          : sourceNodeSyntax,
                        location                  : location,
                        choiceNode                : choiceNode);
                    break;
                case DialogNodeSymbol dialogNode  :
                    AddTriggerTransition(
                        transitionDefinitionSyntax: transitionDefinitionSyntax,
                        targetNodeReference       : targetNodeReference,
                        edgeMode                  : edgeMode,
                        sourceNodeSyntax          : sourceNodeSyntax,
                        location                  : location,
                        guiNode                   : dialogNode);
                    break;
                case ViewNodeSymbol viewNodeNode  :
                    AddTriggerTransition(
                        transitionDefinitionSyntax: transitionDefinitionSyntax,
                        targetNodeReference       : targetNodeReference,
                        edgeMode                  : edgeMode,
                        sourceNodeSyntax          : sourceNodeSyntax,
                        location                  : location,
                        guiNode                   : viewNodeNode);
                    break;
            }
        }

        private void AddInitTransition(TransitionDefinitionSyntax transitionDefinitionSyntax, NodeReferenceSymbol targetNodeReference, EdgeModeSymbol edgeMode, SourceNodeSyntax sourceNodeSyntax, Location location, InitNodeSymbol initNode) {

            var initNodeReference = new InitNodeReferenceSymbol(sourceNodeSyntax.Name, location, initNode, NodeReferenceType.Source);
            var initTransition    = new InitTransition(transitionDefinitionSyntax, _taskDefinition, initNodeReference, edgeMode, targetNodeReference);

            _taskDefinition.InitTransitions.Add(initTransition);

            initNode.Outgoings.Add(initTransition);
            initNode.References.Add(initTransition.SourceReference);
            
            WireTargetNodeReferences(initTransition);

            VerifyTransition(initTransition);

            var triggers = GetTriggers(transitionDefinitionSyntax).OfType<ISignalTriggerSymbol>().ToList();

            if (triggers.Any()) {

                _diagnostics.Add(new Diagnostic(
                                     triggers.First().Location,
                                     triggers.Skip(1).Select(t => t.Location),
                                     DiagnosticDescriptors.Semantic.Nav0200SignalTriggerNotAllowedAfterInit));
            }
        }

        private void AddChoiceTransition(TransitionDefinitionSyntax transitionDefinitionSyntax, NodeReferenceSymbol targetNodeReference, EdgeModeSymbol edgeMode, SourceNodeSyntax sourceNodeSyntax, Location location, ChoiceNodeSymbol choiceNode) {

            var choiceNodeReference = new ChoiceNodeReferenceSymbol(sourceNodeSyntax.Name, location, choiceNode, NodeReferenceType.Source);
            var choiceTransition    = new ChoiceTransition(transitionDefinitionSyntax, _taskDefinition, choiceNodeReference, edgeMode, targetNodeReference);

            _taskDefinition.ChoiceTransitions.Add(choiceTransition);

            choiceNode.Outgoings.Add(choiceTransition);
            choiceNode.References.Add(choiceTransition.SourceReference);

            WireTargetNodeReferences(choiceTransition);

            VerifyTransition(choiceTransition);

            var triggers = GetTriggers(transitionDefinitionSyntax);

            if (triggers.Any()) {

                _diagnostics.Add(new Diagnostic(
                                     triggers.First().Location,
                                     triggers.Skip(1).Select(t => t.Location),
                                     DiagnosticDescriptors.Semantic.Nav0203TriggerNotAllowedAfterChoice));
            }
        }

        private void AddTriggerTransition(
            TransitionDefinitionSyntax transitionDefinitionSyntax, 
            NodeReferenceSymbol targetNodeReference, 
            EdgeModeSymbol edgeMode, 
            SourceNodeSyntax sourceNodeSyntax, 
            Location location, 
            IGuiNodeSymbolConstruction guiNode) {

            // Triggers
            var triggers          = GetTriggers(transitionDefinitionSyntax);
            var guiNodeReference  = new GuiNodeReferenceSymbol(sourceNodeSyntax.Name, location, guiNode, NodeReferenceType.Source);
            var triggerTransition = new TriggerTransition(transitionDefinitionSyntax, _taskDefinition, guiNodeReference, edgeMode, targetNodeReference, triggers);

            _taskDefinition.TriggerTransitions.Add(triggerTransition);

            guiNode.Outgoings.Add(triggerTransition);
            guiNode.References.Add(triggerTransition.SourceReference);

            WireTargetNodeReferences(triggerTransition);

            VerifyTransition(triggerTransition);

            //==============================
            // Nav0201SpontaneousNotAllowedInSignalTrigger
            //==============================
            foreach (var trigger in triggers.Where(t => t.IsSignalTrigger)) {
                if (trigger.Name == SyntaxFacts.SpontaneousKeyword || trigger.Name == SyntaxFacts.SpontKeyword) {
                    _diagnostics.Add(new Diagnostic(
                                         trigger.Location,
                                         DiagnosticDescriptors.Semantic.Nav0201SpontaneousNotAllowedInSignalTrigger));
                }
            }

            //==============================
            // Nav0220ConditionsAreNotAllowedInTriggerTransitions
            //==============================
            if (transitionDefinitionSyntax.ConditionClause != null) {

                _diagnostics.Add(new Diagnostic(
                                     transitionDefinitionSyntax.ConditionClause.GetLocation(),
                                     DiagnosticDescriptors.Semantic.Nav0220ConditionsAreNotAllowedInTriggerTransitions));

            }

        }

        void VerifyTransition(Transition transition) {

            //==============================
            // Edge Errors
            //==============================
            if (transition.EdgeMode != null) {

                if (transition.EdgeMode.EdgeMode != EdgeMode.Goto) {

                    if (transition.TargetReference?.Declaration is ChoiceNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             transition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo,
                                             transition.TargetReference.Name));
                    }

                    if (transition.TargetReference?.Declaration is ExitNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             transition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo,
                                             transition.TargetReference.Name));
                    }

                    if (transition.TargetReference?.Declaration is EndNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             transition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo,
                                             transition.TargetReference.Name));
                    }
                }
            }

            // TODO High Nav0202SpontaneousOnlyAllowedAfterViewAndInitNodes
            //==============================
            // Trigger Errors
            //==============================
            //if (transition.Triggers.Any()) {

            //    foreach (var trigger in transition.Triggers.Where(t => t.IsSpontaneousTrigger)) {

            //        if (!(transition.SourceReference?.Declaration is DialogNodeSymbol ||
            //              transition.SourceReference?.Declaration is ViewNodeSymbol ||
            //              transition.SourceReference?.Declaration is InitNodeSymbol)) {

            //            _diagnostics.Add(new Diagnostic(
            //                trigger.Location,
            //                DiagnosticDescriptors.Semantic.Nav0202SpontaneousOnlyAllowedAfterViewAndInitNodes));
            //        }
            //    }

            //}

        }

        private static void WireTargetNodeReferences(Transition transition) {

            //==============================
            // Target
            //==============================
            if (transition.TargetReference != null) {
                switch (transition.TargetReference.Declaration) {
                    case EndNodeSymbol endNode:
                        endNode.Incomings.Add(transition);
                        endNode.References.Add(transition.TargetReference);
                        break;
                    case ExitNodeSymbol exitNode:
                        exitNode.Incomings.Add(transition);
                        exitNode.References.Add(transition.TargetReference);
                        break;
                    case DialogNodeSymbol dialogNode:
                        dialogNode.Incomings.Add(transition);
                        dialogNode.References.Add(transition.TargetReference);
                        break;
                    case ViewNodeSymbol viewNode:
                        viewNode.Incomings.Add(transition);
                        viewNode.References.Add(transition.TargetReference);
                        break;
                    case ChoiceNodeSymbol choiceNode:
                        choiceNode.Incomings.Add(transition);
                        choiceNode.References.Add(transition.TargetReference);
                        break;
                    case TaskNodeSymbol taskNode:
                        taskNode.Incomings.Add(transition);
                        taskNode.References.Add(transition.TargetReference);
                        break;
                }
            }
        }

        List<TriggerSymbol> _triggers;
        SymbolCollection<TriggerSymbol> GetTriggers(TransitionDefinitionSyntax transitionDefinitionSyntax) {

            var triggers = new List<TriggerSymbol>();

            if (transitionDefinitionSyntax.Trigger != null) {
                _triggers = triggers;
                Visit(transitionDefinitionSyntax.Trigger);
                _triggers = null;
            }

            var result = new SymbolCollection<TriggerSymbol>();
            foreach (var trigger in triggers) {
                var existing = result.TryFindSymbol(trigger.Name);
                if (existing != null) {

                    _diagnostics.Add(new Diagnostic(
                                         trigger.Location,
                                         existing.Location,
                                         DiagnosticDescriptors.Semantic.Nav0026TriggerWithName0AlreadyDeclared,
                                         existing.Name));

                } else {
                    result.Add(trigger);
                }
            }

            return result;
        }

        public override void VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) {
            var location = spontaneousTriggerSyntax.GetLocation();
            if (location != null) {
                var trigger = new SpontaneousTriggerSymbol(location, spontaneousTriggerSyntax);
                _triggers.Add(trigger);
            }
        }

        public override void VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) {

            if (signalTriggerSyntax.IdentifierOrStringList == null) {
                return;
            }

            foreach (var signal in signalTriggerSyntax.IdentifierOrStringList) {
                var location = signal.GetLocation();
                if (location != null) {
                    var trigger = new SignalTriggerSymbol(signal.Text, location, signal);
                    _triggers.Add(trigger);
                }
            }
        }

        #endregion

        #region ExitTransitions

        public override void VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) {
            // Source
            ITaskNodeSymbol         sourceTaskNodeSymbol = null;
            TaskNodeReferenceSymbol sourceNodeReference  = null;
            var                     sourceNodeSyntax     = exitTransitionDefinitionSyntax.SourceNode;

            if (sourceNodeSyntax != null) {
                // Source in Exit Transition muss immer ein Task sein
                sourceTaskNodeSymbol = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name) as ITaskNodeSymbol;
                var location = sourceNodeSyntax.GetLocation();

                if (location != null) {
                    sourceNodeReference = new TaskNodeReferenceSymbol(sourceNodeSyntax.Name, location, sourceTaskNodeSymbol, NodeReferenceType.Source);
                }
            }

            // ConnectionPoint
            ConnectionPointReferenceSymbol connectionPointReference = null;
            var                            exitIdentifier           = exitTransitionDefinitionSyntax.ExitIdentifier;
            if (!exitIdentifier.IsMissing && sourceTaskNodeSymbol != null) {

                var exitIdentifierName  = exitIdentifier.ToString();
                var exitConnectionPoint = sourceTaskNodeSymbol.Declaration?.ConnectionPoints.TryFindSymbol(exitIdentifierName) as IExitConnectionPointSymbol;
                var location            = exitIdentifier.GetLocation();

                if (location != null) {
                    connectionPointReference = new ConnectionPointReferenceSymbol(exitIdentifierName, location, exitConnectionPoint);
                }
            }

            // Target
            // TODO entsprechend des Knotens ein ensprechendes NodeReferenceSymbol Derivat instantieren
            NodeReferenceSymbol targetNodeReference = null;
            var                 targetNodeSyntax    = exitTransitionDefinitionSyntax.TargetNode;
            if (targetNodeSyntax != null) {

                var targetNode = _taskDefinition.NodeDeclarations.TryFindSymbol(targetNodeSyntax.Name);
                var location   = targetNodeSyntax.GetLocation();

                if (location != null) {
                    targetNodeReference = new NodeReferenceSymbol(targetNodeSyntax.Name, location, targetNode, NodeReferenceType.Target);
                }
            }

            // Edge
            EdgeModeSymbol edgeMode   = null;
            var            edgeSyntax = exitTransitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var location = edgeSyntax.GetLocation();

                if (location != null) {
                    edgeMode = new EdgeModeSymbol(edgeSyntax.ToString(), location, edgeSyntax.Mode);
                }
            }

            var exitTransition = new ExitTransition(exitTransitionDefinitionSyntax, _taskDefinition, sourceNodeReference, connectionPointReference, edgeMode, targetNodeReference);

            AddExitTransition(exitTransition);
        }

        void AddExitTransition(ExitTransition exitTransition) {

            //==============================
            // Source Errors
            //==============================
            if (exitTransition.SourceReference != null) {
                // Source in Exit Transition muss immer ein Task sein
                if (exitTransition.SourceReference.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.SourceReference.Location,
                                         DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0,
                                         exitTransition.SourceReference.Name));

                } else {
                    var sourceNode = (TaskNodeSymbol) exitTransition.SourceReference.Declaration;
                    sourceNode.Outgoings.Add(exitTransition);
                    sourceNode.References.Add(exitTransition.SourceReference);
                }
            }

            //==============================
            // ConnectionPoint Errors
            //==============================
            if (exitTransition.ConnectionPointReference != null) {

                if (exitTransition.ConnectionPointReference.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.ConnectionPointReference.Location,
                                         DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0,
                                         exitTransition.ConnectionPointReference.Name));

                } else if (exitTransition.ConnectionPointReference.Declaration.Kind != ConnectionPointKind.Exit) {
                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.ConnectionPointReference.Location,
                                         DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0,
                                         exitTransition.ConnectionPointReference.Name));
                }
            }

            //==============================
            // Target
            //==============================
            if (exitTransition.TargetReference != null) {

                if (exitTransition.TargetReference.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.TargetReference.Location,
                                         DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0,
                                         exitTransition.TargetReference.Name));

                } else if (exitTransition.TargetReference.Declaration is InitNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.TargetReference.Location,
                                         DiagnosticDescriptors.Semantic.Nav0103InitNodeMustNotContainIncomingEdges));

                } else if (exitTransition.TargetReference.Declaration is EndNodeSymbol endNode) {
                    endNode.Incomings.Add(exitTransition);
                    endNode.References.Add(exitTransition.TargetReference);

                } else if (exitTransition.TargetReference.Declaration is ExitNodeSymbol exitNode) {
                    exitNode.Incomings.Add(exitTransition);
                    exitNode.References.Add(exitTransition.TargetReference);

                } else if (exitTransition.TargetReference.Declaration is DialogNodeSymbol dialogNode) {
                    dialogNode.Incomings.Add(exitTransition);
                    dialogNode.References.Add(exitTransition.TargetReference);

                } else if (exitTransition.TargetReference.Declaration is ViewNodeSymbol viewNode) {
                    viewNode.Incomings.Add(exitTransition);
                    viewNode.References.Add(exitTransition.TargetReference);

                } else if (exitTransition.TargetReference.Declaration is ChoiceNodeSymbol choiceNode) {
                    choiceNode.Incomings.Add(exitTransition);
                    choiceNode.References.Add(exitTransition.TargetReference);

                } else if (exitTransition.TargetReference.Declaration is TaskNodeSymbol taskNode) {
                    taskNode.Incomings.Add(exitTransition);
                    taskNode.References.Add(exitTransition.TargetReference);
                }
            }

            //==============================
            // Edge Errors
            //==============================
            if (exitTransition.EdgeMode != null) {

                if (exitTransition.EdgeMode.EdgeMode != EdgeMode.Goto) {

                    if (exitTransition.TargetReference?.Declaration is ChoiceNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             exitTransition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo,
                                             exitTransition.TargetReference.Name));
                    }

                    if (exitTransition.TargetReference?.Declaration is ExitNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             exitTransition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo,
                                             exitTransition.TargetReference.Name));
                    }

                    if (exitTransition.TargetReference?.Declaration is EndNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                             exitTransition.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo,
                                             exitTransition.TargetReference.Name));
                    }
                }
            }

            //==============================
            // Condition Clause Errors
            //==============================
            if (exitTransition.Syntax.ConditionClause != null) {

                if (!(exitTransition.Syntax.ConditionClause is IfConditionClauseSyntax)) {

                    _diagnostics.Add(new Diagnostic(
                                         exitTransition.Syntax.ConditionClause.GetLocation(),
                                         DiagnosticDescriptors.Semantic.Nav0221OnlyIfConditionsAllowedInExitTransitions));
                }

            }

            _taskDefinition.ExitTransitions.Add(exitTransition);
        }

        #endregion

        void AnalyzeTaskDefinition() {

            var analyzers = Analyzer.GetTaskDefinitionAnalyzer();
            var context   = new AnalyzerContext();
            foreach (var anlyzer in analyzers) {
                _diagnostics.AddRange(anlyzer.Analyze(_taskDefinition, context));
            }


            //==============================
            // Trigger Errors
            //==============================
            var triggerMap = new Dictionary<INodeSymbol, ITriggerSymbol>();
            foreach (var trans in _taskDefinition.TriggerTransitions) {
                // Nicht deklarierte Sourcenodes interessieren uns nicht
                var nodeSymbol = trans.SourceReference?.Declaration;
                if (nodeSymbol == null) {
                    continue;
                }

                triggerMap.TryGetValue(nodeSymbol, out var existing);

                foreach (var trigger in trans.Triggers) {

                    if (existing != null && trigger.Name == existing.Name) {

                        _diagnostics.Add(new Diagnostic(
                                             trigger.Location,
                                             existing.Location,
                                             DiagnosticDescriptors.Semantic.Nav0023AnOutgoingEdgeForTrigger0IsAlreadyDeclared,
                                             existing.Name));

                    } else {
                        triggerMap[nodeSymbol] = trigger;
                    }
                }
            }

            //==============================
            // Edge Errors
            //==============================
            foreach (var initNode in _taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {

                // Interessanterweise darf eine Init-Transition merhr als einen Ausgang haben, und hat somit
                // eine "eingebaute choice".
                foreach (var initTransition in initNode.Outgoings) {
                    foreach (var reachableCall in initTransition.GetReachableCalls()
                                                                .Where(c => c.EdgeMode.EdgeMode != EdgeMode.Goto)) {
                        _diagnostics.Add(new Diagnostic(
                                             reachableCall.EdgeMode.Location,
                                             DiagnosticDescriptors.Semantic.Nav0110Edge0NotAllowedIn1BecauseItsReachableFromInit2,
                                             reachableCall.EdgeMode.DisplayName,
                                             reachableCall.Node.Name,
                                             initNode.Name));
                    }
                }
            }

            foreach (IEdge edge in _taskDefinition.Edges()) {

                foreach (var nodeCalls in edge.GetReachableCalls().GroupBy(c => c.Node)) {

                    if (nodeCalls.GroupBy(c => c.EdgeMode.EdgeMode).Count() > 1) {

                        _diagnostics.Add(new Diagnostic(
                                             nodeCalls.First().EdgeMode.Location,
                                             nodeCalls.Skip(1).Select(call => call.EdgeMode.Location),
                                             DiagnosticDescriptors.Semantic.Nav0222Node0IsReachableByDifferentEdgeModes,
                                             nodeCalls.Key.Name
                                         ));
                    }
                }

            }
        }

        public static TaskDefinitionBuilderResult Build(TaskDefinitionSyntax taskDefinitionSyntax, IReadOnlySymbolCollection<TaskDeclarationSymbol> taskDeklarations) {           
            var builder = new TaskDefinitionSymbolBuilder(taskDeklarations);
            builder.Visit(taskDefinitionSyntax);
            return new TaskDefinitionBuilderResult(builder._taskDefinition, builder._diagnostics);
        }

    }

}