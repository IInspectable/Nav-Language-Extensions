#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    
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
                // Diesen Fall haben wir, wenn nur "task ;" eingegeben wird. Daf�r gibt es aber bereits einen Syntax Fehler.
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

            taskNode.Declaration?.References.Add(taskNode);

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

        #region Edges (Transitions / ExitTransitions)

        public override void VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) {

            // Target            
            var targetNodeReference = CreateTargetNodeReference(transitionDefinitionSyntax.TargetNode);

            // Edge
            EdgeModeSymbol edgeMode   = null;
            var            edgeSyntax = transitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var edgeLocation = edgeSyntax.GetLocation();
                if (edgeLocation != null) {
                    edgeMode = new EdgeModeSymbol(edgeSyntax.ToString(), edgeLocation, edgeSyntax.Mode);
                }
            }

            // Source: Ohne Source Node k�nnen wir keine Transitions hinzuf�gen, da dieser Knoten relevant ist f�r die Bestimmung
            // der Transition (Init, Choice, Trigger)

            var sourceNodeSyntax = transitionDefinitionSyntax.SourceNode;
            if (sourceNodeSyntax == null) {
                return;
            }

            var sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name);

            // Special case "init": Hier ist implizit auch Gro�schreibung erlaubt
            if (sourceNode == null && sourceNodeSyntax.Name == SyntaxFacts.InitKeyword) {
                sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(SyntaxFacts.InitKeywordAlt);
            }

            var sourceNodeLocation = sourceNodeSyntax.GetLocation();
            if (sourceNodeLocation == null) {
                return;
            }

            switch (sourceNode) {
                case null:
                   _diagnostics.Add(new Diagnostic(
                                        sourceNodeLocation,
                                        DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0,
                                        sourceNodeSyntax.Name));
                    break;
                case TaskNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         sourceNodeLocation,
                                         DiagnosticDescriptors.Semantic.Nav0100TaskNode0MustNotContainLeavingEdges,
                                         sourceNodeSyntax.Name));
                    break;
                case ExitNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         sourceNodeLocation,
                                         DiagnosticDescriptors.Semantic.Nav0101ExitNodeMustNotContainLeavingEdges));
                    break;
                case EndNodeSymbol _:
                    _diagnostics.Add(new Diagnostic(
                                         sourceNodeLocation,
                                         DiagnosticDescriptors.Semantic.Nav0102EndNodeMustNotContainLeavingEdges));
                    break;
                case InitNodeSymbol initNode:
                    AddInitTransition(initNode                     : initNode,
                                      transitionDefinitionSyntax   : transitionDefinitionSyntax,
                                      sourceNodeSyntax             : sourceNodeSyntax,
                                      sourceNodeLocation           : sourceNodeLocation,
                                      edgeMode                     : edgeMode,
                                      targetNodeReference          : targetNodeReference);
                    break;
                case ChoiceNodeSymbol choiceNode                   :
                    AddChoiceTransition(choiceNode                 : choiceNode,
                                        transitionDefinitionSyntax : transitionDefinitionSyntax, 
                                        sourceNodeSyntax           : sourceNodeSyntax, 
                                        sourceNodelocation         : sourceNodeLocation, 
                                        edgeMode                   : edgeMode, 
                                        targetNodeReference        : targetNodeReference);
                    break;
                case DialogNodeSymbol dialogNode                   :
                    AddTriggerTransition(guiNode                   : dialogNode,
                                         transitionDefinitionSyntax: transitionDefinitionSyntax, 
                                         sourceNodeSyntax          : sourceNodeSyntax, 
                                         sourceNodelocation        : sourceNodeLocation, 
                                         edgeMode                  : edgeMode, 
                                         targetNodeReference       : targetNodeReference);
                    break;
                case ViewNodeSymbol viewNodeNode                   :
                    AddTriggerTransition(guiNode                   : viewNodeNode,
                                         transitionDefinitionSyntax: transitionDefinitionSyntax, 
                                         sourceNodeSyntax          : sourceNodeSyntax, 
                                         sourceNodelocation        : sourceNodeLocation, 
                                         edgeMode                  : edgeMode, 
                                         targetNodeReference       : targetNodeReference);
                    break;
            }
        }
        
        public override void VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) {
            
            // Source
            ITaskNodeSymbol            sourceTaskNodeSymbol = null;
            TaskNodeReferenceSymbol    sourceNodeReference  = null;
            IdentifierSourceNodeSyntax sourceNodeSyntax     = exitTransitionDefinitionSyntax.SourceNode;

            if (sourceNodeSyntax != null) {
                // Source in Exit Transition muss immer ein Task sein
                sourceTaskNodeSymbol = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name) as ITaskNodeSymbol;
                var sourceNodeLocation = sourceNodeSyntax.GetLocation();

                if (sourceNodeLocation != null) {
                    sourceNodeReference = new TaskNodeReferenceSymbol(sourceNodeSyntax.Name, sourceNodeLocation, sourceTaskNodeSymbol, NodeReferenceType.Source);
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
            var targetNodeReference = CreateTargetNodeReference(exitTransitionDefinitionSyntax.TargetNode);

            // Edge
            EdgeModeSymbol edgeMode   = null;
            var            edgeSyntax = exitTransitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var location = edgeSyntax.GetLocation();

                if (location != null) {
                    edgeMode = new EdgeModeSymbol(edgeSyntax.ToString(), location, edgeSyntax.Mode);
                }
            }

            AddExitTransition(exitTransitionDefinitionSyntax, sourceNodeReference, connectionPointReference, edgeMode, targetNodeReference);
        }

        private void AddExitTransition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax, TaskNodeReferenceSymbol sourceNodeReference, ConnectionPointReferenceSymbol connectionPointReference, EdgeModeSymbol edgeMode, NodeReferenceSymbol targetNodeReference) {

            var exitTransition = new ExitTransition(exitTransitionDefinitionSyntax, _taskDefinition, sourceNodeReference, connectionPointReference, edgeMode, targetNodeReference);
            var taskNode       = exitTransition.TaskNodeSourceReference?.Declaration as TaskNodeSymbol;

            _taskDefinition.ExitTransitions.Add(exitTransition);

            taskNode?.Outgoings.Add(exitTransition);
            taskNode?.References.Add(exitTransition.SourceReference);

            WireTargetNodeReferences(exitTransition);
        }

        private void AddInitTransition(InitNodeSymbol initNode, TransitionDefinitionSyntax transitionDefinitionSyntax, SourceNodeSyntax sourceNodeSyntax, Location sourceNodeLocation, EdgeModeSymbol edgeMode, NodeReferenceSymbol targetNodeReference) {

            var initNodeReference = new InitNodeReferenceSymbol(sourceNodeSyntax.Name, sourceNodeLocation, initNode, NodeReferenceType.Source);
            var initTransition    = new InitTransition(transitionDefinitionSyntax, _taskDefinition, initNodeReference, edgeMode, targetNodeReference);

            _taskDefinition.InitTransitions.Add(initTransition);

            initNode.Outgoings.Add(initTransition);
            initNode.References.Add(initTransition.SourceReference);
            
            WireTargetNodeReferences(initTransition);
        }

        private void AddChoiceTransition(ChoiceNodeSymbol choiceNode, TransitionDefinitionSyntax transitionDefinitionSyntax, SourceNodeSyntax sourceNodeSyntax, Location sourceNodelocation, EdgeModeSymbol edgeMode, NodeReferenceSymbol targetNodeReference) {

            var choiceNodeReference = new ChoiceNodeReferenceSymbol(sourceNodeSyntax.Name, sourceNodelocation, choiceNode, NodeReferenceType.Source);
            var choiceTransition    = new ChoiceTransition(transitionDefinitionSyntax, _taskDefinition, choiceNodeReference, edgeMode, targetNodeReference);

            _taskDefinition.ChoiceTransitions.Add(choiceTransition);

            choiceNode.Outgoings.Add(choiceTransition);
            choiceNode.References.Add(choiceTransition.SourceReference);

            WireTargetNodeReferences(choiceTransition);
        }

        private void AddTriggerTransition(IGuiNodeSymbolConstruction guiNode, TransitionDefinitionSyntax transitionDefinitionSyntax, SourceNodeSyntax sourceNodeSyntax, Location sourceNodelocation, EdgeModeSymbol edgeMode, NodeReferenceSymbol targetNodeReference) {

            var (triggers, diagnostics) = TriggerSymbolBuilder.Build(transitionDefinitionSyntax);

            _diagnostics.AddRange(diagnostics);

            var guiNodeReference  = new GuiNodeReferenceSymbol(sourceNodeSyntax.Name, sourceNodelocation, guiNode, NodeReferenceType.Source);
            var triggerTransition = new TriggerTransition(transitionDefinitionSyntax, _taskDefinition, guiNodeReference, edgeMode, targetNodeReference, triggers);

            _taskDefinition.TriggerTransitions.Add(triggerTransition);

            guiNode.Outgoings.Add(triggerTransition);
            guiNode.References.Add(triggerTransition.SourceReference);

            WireTargetNodeReferences(triggerTransition);
        }

        private NodeReferenceSymbol CreateTargetNodeReference(TargetNodeSyntax targetNodeSyntax) {

            if (targetNodeSyntax == null) {
                return null;
            }

            var targetNodeDeclaration = _taskDefinition.NodeDeclarations.TryFindSymbol(targetNodeSyntax.Name);
            var targetNodeLocation    = targetNodeSyntax.GetLocation();

            if (targetNodeLocation == null) {
                return null;
            }

            NodeReferenceSymbol targetNodeReference;
            switch (targetNodeDeclaration) {
                case IInitNodeSymbol initNode:
                    targetNodeReference = new InitNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, initNode, NodeReferenceType.Target);
                    break;
                case IChoiceNodeSymbol choiceNode:
                    targetNodeReference = new ChoiceNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, choiceNode, NodeReferenceType.Target);
                    break;
                case ITaskNodeSymbol taskNode:
                    targetNodeReference = new TaskNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, taskNode, NodeReferenceType.Target);
                    break;
                case IGuiNodeSymbol guiNode:
                    targetNodeReference = new GuiNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, guiNode, NodeReferenceType.Target);
                    break;
                case IExitNodeSymbol exitNode:
                    targetNodeReference = new ExitNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, exitNode, NodeReferenceType.Target);
                    break;
                case IEndNodeSymbol endNode:
                    targetNodeReference = new EndNodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, endNode, NodeReferenceType.Target);
                    break;
                default:
                    targetNodeReference = new NodeReferenceSymbol(targetNodeSyntax.Name, targetNodeLocation, targetNodeDeclaration, NodeReferenceType.Target);
                    break;
            }
            
            return targetNodeReference;
        }
        
        private static void WireTargetNodeReferences(IEdge edge) {

            //==============================
            // Target
            //==============================
            if (edge.TargetReference != null) {
                switch (edge.TargetReference.Declaration) {
                    case null:
                        // Nav0011CannotResolveNode0
                        break;
                    case InitNodeSymbol _:
                        // Nav0103InitNodeMustNotContainIncomingEdges
                        break;
                    case EndNodeSymbol endNode:
                        endNode.Incomings.Add(edge);
                        endNode.References.Add(edge.TargetReference);
                        break;
                    case ExitNodeSymbol exitNode:
                        exitNode.Incomings.Add(edge);
                        exitNode.References.Add(edge.TargetReference);
                        break;
                    case DialogNodeSymbol dialogNode:
                        dialogNode.Incomings.Add(edge);
                        dialogNode.References.Add(edge.TargetReference);
                        break;
                    case ViewNodeSymbol viewNode:
                        viewNode.Incomings.Add(edge);
                        viewNode.References.Add(edge.TargetReference);
                        break;
                    case ChoiceNodeSymbol choiceNode:
                        choiceNode.Incomings.Add(edge);
                        choiceNode.References.Add(edge.TargetReference);
                        break;
                    case TaskNodeSymbol taskNode:
                        taskNode.Incomings.Add(edge);
                        taskNode.References.Add(edge.TargetReference);
                        break;
                }
            }
        }

        #endregion

        public static (
            TaskDefinitionSymbol TaskDefinition,
            List<Diagnostic>     Diagnostics)
            Build(TaskDefinitionSyntax taskDefinitionSyntax, IReadOnlySymbolCollection<TaskDeclarationSymbol> taskDeklarations) {

            var builder = new TaskDefinitionSymbolBuilder(taskDeklarations);
            builder.Visit(taskDefinitionSyntax);
            return (builder._taskDefinition, builder._diagnostics);
        }

    }

}