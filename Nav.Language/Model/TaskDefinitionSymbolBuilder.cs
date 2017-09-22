#region Using Directives

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    
    sealed class TaskDefinitionBuilderResult {

        public TaskDefinitionBuilderResult(TaskDefinitionSymbol taskDefinition, IReadOnlyList<Diagnostic> diagnostics) {
            TaskDefinition = taskDefinition;
            Diagnostics = diagnostics ?? new List<Diagnostic>();
        }

        [CanBeNull]
        public TaskDefinitionSymbol TaskDefinition { get; }
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
    }

    sealed class TaskDefinitionSymbolBuilder: SyntaxNodeVisitor {

        readonly IReadOnlySymbolCollection<TaskDeclarationSymbol> _taskDeklarations;
        readonly List<Diagnostic> _diagnostics;

        TaskDefinitionSymbol _taskDefinition;

        TaskDefinitionSymbolBuilder(IReadOnlySymbolCollection<TaskDeclarationSymbol> taskDeklarations) {
            _taskDeklarations = taskDeklarations;
            _diagnostics = new List<Diagnostic>();
        }

        public override void VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {
            var identifier = taskDefinitionSyntax.Identifier;
            var location   = identifier.GetLocation();

            if(location == null) {
                return;
            }

            var taskName = identifier.ToString();

            var taskDeclaration = _taskDeklarations.TryFindSymbol(taskName);
            if(taskDeclaration.Location != location) {
                taskDeclaration = null;
            }

            _taskDefinition = new TaskDefinitionSymbol(taskName, location, taskDefinitionSyntax, taskDeclaration);

            // Declarations
            foreach(var nodeDeclarationSyntax in taskDefinitionSyntax.NodeDeclarationBlock?.NodeDeclarations ?? Enumerable.Empty<NodeDeclarationSyntax>()) {
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

            VerifyTaskDefinition();
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

            if(location == null) {
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
                taskNodeAlias     = new TaskNodeAliasSymbol(aliasName, aliasLocation);
            }
            
            var taskName        = taskIdentifier.ToString();
            var taskLocation    = taskIdentifier.GetLocation();
            var taskDeclaration = _taskDeklarations.TryFindSymbol(taskName);

            var taskNode = new TaskNodeSymbol(taskName, taskLocation, taskNodeDeclarationSyntax, taskNodeAlias, taskDeclaration, _taskDefinition);

            if(taskNode.Declaration == null) {
                if(taskLocation != null) {
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
            var location = identifier.GetLocation();
            var name = identifier.ToString();

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
            
            // Source
            NodeReferenceSymbol sourceNodeReference = null;
            var sourceNodeSyntax = transitionDefinitionSyntax.SourceNode;
            if (sourceNodeSyntax != null) {

                var sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name);

                // Special case "init": Hier ist implizit auch Großschreibung erlaubt
                if (sourceNode == null && sourceNodeSyntax.Name== SyntaxFacts.InitKeyword) {
                    sourceNode = _taskDefinition.NodeDeclarations.TryFindSymbol(SyntaxFacts.InitKeywordAlt);
                }

                var location   = sourceNodeSyntax.GetLocation();
                if(location != null) {
                    sourceNodeReference = new NodeReferenceSymbol(sourceNodeSyntax.Name, location, sourceNode, NodeReferenceType.Source);
                }
            }

            // Target
            NodeReferenceSymbol targetNodeReference = null;
            var targetNodeSyntax = transitionDefinitionSyntax.TargetNode;
            if (targetNodeSyntax != null) {

                var targetNode = _taskDefinition.NodeDeclarations.TryFindSymbol(targetNodeSyntax.Name);
                var location = targetNodeSyntax.GetLocation();
                if (location != null) {
                    targetNodeReference = new NodeReferenceSymbol(targetNodeSyntax.Name, location, targetNode, NodeReferenceType.Target);                
                }
            }
            
            // Edge
            EdgeModeSymbol edgeMode=null;
            var edgeSyntax = transitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var location = edgeSyntax.GetLocation();
                if(location != null) {
                    edgeMode = new EdgeModeSymbol(edgeSyntax.ToString(), location, edgeSyntax.Mode);
                }
            }

            // Triggers
            var triggers = GetTriggers(transitionDefinitionSyntax);

            var transition = new Transition(transitionDefinitionSyntax, _taskDefinition, sourceNodeReference, edgeMode, targetNodeReference, triggers);
            
            AddTransition(transition);
        }
        
        void AddTransition(Transition transition) {

            //==============================
            // Source Errors
            //==============================
            if(transition.Source != null) {

                if(transition.Source.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Source.Location, 
                        DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0, 
                        transition.Source.Name));

                } else if(transition.Source.Declaration is TaskNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Source.Location, 
                        DiagnosticDescriptors.Semantic.Nav0100TaskNode0MustNotContainLeavingEdges, 
                        transition.Source.Name));

                } else if(transition.Source.Declaration is ExitNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Source.Location, 
                        DiagnosticDescriptors.Semantic.Nav0101ExitNodeMustNotContainLeavingEdges));

                } else if(transition.Source.Declaration is EndNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Source.Location, 
                        DiagnosticDescriptors.Semantic.Nav0102EndNodeMustNotContainLeavingEdges) );

                } else if(transition.Source.Declaration is InitNodeSymbol) {
                    var node = (InitNodeSymbol) transition.Source.Declaration;
                    node.Outgoings.Add(transition);
                    node.References.Add(transition.Source);

                } else if(transition.Source.Declaration is DialogNodeSymbol) {
                    var node = (DialogNodeSymbol) transition.Source.Declaration;
                    node.Outgoings.Add(transition);
                    node.References.Add(transition.Source);

                } else if(transition.Source.Declaration is ViewNodeSymbol) {
                    var node = (ViewNodeSymbol) transition.Source.Declaration;
                    node.Outgoings.Add(transition);
                    node.References.Add(transition.Source);

                } else if(transition.Source.Declaration is ChoiceNodeSymbol) {                   
                    var node = (ChoiceNodeSymbol) transition.Source.Declaration;
                    node.Outgoings.Add(transition);
                    node.References.Add(transition.Source);
                }
            }

            //==============================
            // Target Errors
            //==============================
            if (transition.Target != null) {

                if (transition.Target.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Target.Location, 
                        DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0, 
                        transition.Target.Name));

                } else if (transition.Target.Declaration is InitNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                        transition.Target.Location,
                        DiagnosticDescriptors.Semantic.Nav0103InitNodeMustNotContainIncomingEdges));

                } else if (transition.Target.Declaration is EndNodeSymbol) {
                    var node = (EndNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);

                } else if (transition.Target.Declaration is ExitNodeSymbol) {
                    var node = (ExitNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);

                } else if (transition.Target.Declaration is DialogNodeSymbol) {
                    var node = (DialogNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);

                } else if (transition.Target.Declaration is ViewNodeSymbol) {
                    var node = (ViewNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);

                } else if (transition.Target.Declaration is ChoiceNodeSymbol) {
                    var node = (ChoiceNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);

                } else if (transition.Target.Declaration is TaskNodeSymbol) {
                    var node = (TaskNodeSymbol)transition.Target.Declaration;
                    node.Incomings.Add(transition);
                    node.References.Add(transition.Target);
                }
            }

            //==============================
            // Edge Errors
            //==============================
            if (transition.EdgeMode !=null) {

                if(transition.EdgeMode.EdgeMode != EdgeMode.Goto) {

                    if(transition.Target?.Declaration is ChoiceNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                            transition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo,
                            transition.Target.Name));
                    }

                    if(transition.Target?.Declaration is ExitNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                            transition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo,
                            transition.Target.Name) );
                    }

                    if(transition.Target?.Declaration is EndNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                            transition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo,
                            transition.Target.Name) );
                    }
                }
            }

            //==============================
            // Trigger Errors
            //==============================
            if (transition.Triggers.Any()) {

                foreach(var trigger in transition.Triggers.Where(t=>t.IsSpontaneousTrigger)) {

                    if(!(transition.Source?.Declaration is DialogNodeSymbol ||
                         transition.Source?.Declaration is ViewNodeSymbol ||
                         transition.Source?.Declaration is InitNodeSymbol)) {

                        _diagnostics.Add(new Diagnostic(
                            trigger.Location,
                            DiagnosticDescriptors.Semantic.Nav0202SpontaneousOnlyAllowedAfterViewAndInitNodes));
                    }
                }

                foreach (var trigger in transition.Triggers.Where(t => t.IsSignalTrigger)) {

                    if (transition.Source?.Declaration is InitNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                            trigger.Location, 
                            DiagnosticDescriptors.Semantic.Nav0200SignalTriggerNotAllowedAfterInit));
                    }

                    if (transition.Source?.Declaration is ChoiceNodeSymbol) {
                        _diagnostics.Add(new Diagnostic(
                                trigger.Location,
                                DiagnosticDescriptors.Semantic.Nav0203TriggerNotAllowedAfterChoice));
                    }

                    if (trigger.Name == SyntaxFacts.SpontaneousKeyword || trigger.Name == SyntaxFacts.SpontKeyword) {
                        _diagnostics.Add(new Diagnostic(
                            trigger.Location,
                            DiagnosticDescriptors.Semantic.Nav0201SpontaneousNotAllowedInSignalTrigger));
                    }
                }
            }

            //==============================
            // Condition Clause Errors
            //==============================
            if (transition.Syntax.ConditionClause != null) {

                if(transition.Source != null && ! (
                    transition.Source?.Declaration is InitNodeSymbol ||
                    transition.Source?.Declaration is ChoiceNodeSymbol)){

                    _diagnostics.Add(new Diagnostic(
                            transition.Syntax.ConditionClause.GetLocation(),
                            DiagnosticDescriptors.Semantic.Nav0220ConditionsAreOnlySupportedAfterInitAndChoiceNodes));

                }
            }

            _taskDefinition.Transitions.Add(transition);
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
            foreach(var trigger in triggers) {
                var existing = result.TryFindSymbol(trigger.Name);
                if(existing != null) {

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
            if(location != null) {
                var trigger = new SpontaneousTriggerSymbol(location, spontaneousTriggerSyntax);
                _triggers.Add(trigger);
            }            
        }

        public override void VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) {

            if (signalTriggerSyntax.IdentifierOrStringList == null) {
                return;
            }

            foreach(var signal in signalTriggerSyntax.IdentifierOrStringList) {
                var location = signal.GetLocation();
                if(location != null) {
                    var trigger = new SignalTriggerSymbol(signal.Text, location, signal);
                    _triggers.Add(trigger);
                }
            }
        }

        #endregion

        #region ExitTransitions

        public override void VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) {
            // Source
            ITaskNodeSymbol sourceTaskNodeSymbol = null;
            NodeReferenceSymbol sourceNodeReference = null;
            var sourceNodeSyntax = exitTransitionDefinitionSyntax.SourceNode;
            if (sourceNodeSyntax != null) {

                // Source in Exit Transition muss immer ein Task sein
                sourceTaskNodeSymbol = _taskDefinition.NodeDeclarations.TryFindSymbol(sourceNodeSyntax.Name) as ITaskNodeSymbol;
                var location         = sourceNodeSyntax.GetLocation();

                if (location != null) {
                    sourceNodeReference = new NodeReferenceSymbol(sourceNodeSyntax.Name, location, sourceTaskNodeSymbol, NodeReferenceType.Source);
                }
            }

            // ConnectionPoint
            ConnectionPointReferenceSymbol connectionPointReference = null;
            var exitIdentifier = exitTransitionDefinitionSyntax.ExitIdentifier;
            if (!exitIdentifier.IsMissing && sourceTaskNodeSymbol != null) {

                var exitIdentifierName  = exitIdentifier.ToString();
                var exitConnectionPoint = sourceTaskNodeSymbol.Declaration?.ConnectionPoints.TryFindSymbol(exitIdentifierName) as IExitConnectionPointSymbol;
                var location            = exitIdentifier.GetLocation();

                if (location != null) {
                    connectionPointReference = new ConnectionPointReferenceSymbol(exitIdentifierName, location, exitConnectionPoint);
                }
            }

            // Target
            NodeReferenceSymbol targetNodeReference = null;
            var targetNodeSyntax = exitTransitionDefinitionSyntax.TargetNode;
            if (targetNodeSyntax != null) {

                var targetNode = _taskDefinition.NodeDeclarations.TryFindSymbol(targetNodeSyntax.Name);
                var location   = targetNodeSyntax.GetLocation();

                if(location != null) {
                    targetNodeReference = new NodeReferenceSymbol(targetNodeSyntax.Name, location, targetNode, NodeReferenceType.Target);
                }
            }

            // Edge
            EdgeModeSymbol edgeMode = null;
            var edgeSyntax = exitTransitionDefinitionSyntax.Edge;
            if (edgeSyntax != null) {

                var location = edgeSyntax.GetLocation();

                if(location != null) {
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
            if(exitTransition.Source != null) {
                // Source in Exit Transition muss immer ein Task sein
                if (exitTransition.Source.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                        exitTransition.Source.Location, 
                        DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0, 
                        exitTransition.Source.Name));

                } else {
                    var sourceNode = (TaskNodeSymbol) exitTransition.Source.Declaration;
                    sourceNode.Outgoings.Add(exitTransition);
                    sourceNode.References.Add(exitTransition.Source);
                }
            }

            //==============================
            // ConnectionPoint Errors
            //==============================
            if (exitTransition.ConnectionPoint != null) {

                if (exitTransition.ConnectionPoint.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                        exitTransition.ConnectionPoint.Location,
                        DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0,
                        exitTransition.ConnectionPoint.Name));

                } else if(exitTransition.ConnectionPoint.Declaration.Kind !=ConnectionPointKind.Exit) {
                    _diagnostics.Add(new Diagnostic(
                        exitTransition.ConnectionPoint.Location,
                        DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0,
                        exitTransition.ConnectionPoint.Name));
                }
            }

            //==============================
            // Target Errors
            //==============================
            if (exitTransition.Target != null) {

                if (exitTransition.Target.Declaration == null) {
                    _diagnostics.Add(new Diagnostic(
                        exitTransition.Target.Location, 
                        DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0, 
                        exitTransition.Target.Name));

                } else if (exitTransition.Target.Declaration is InitNodeSymbol) {
                    _diagnostics.Add(new Diagnostic(
                        exitTransition.Target.Location,
                        DiagnosticDescriptors.Semantic.Nav0103InitNodeMustNotContainIncomingEdges));

                } else if (exitTransition.Target.Declaration is EndNodeSymbol) {
                    var node = (EndNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);

                } else if (exitTransition.Target.Declaration is ExitNodeSymbol) {
                    var node = (ExitNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);

                } else if (exitTransition.Target.Declaration is DialogNodeSymbol) {
                    var node = (DialogNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);

                } else if (exitTransition.Target.Declaration is ViewNodeSymbol) {
                    var node = (ViewNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);

                } else if (exitTransition.Target.Declaration is ChoiceNodeSymbol) {
                    var node = (ChoiceNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);

                } else if (exitTransition.Target.Declaration is TaskNodeSymbol) {
                    var node = (TaskNodeSymbol)exitTransition.Target.Declaration;
                    node.Incomings.Add(exitTransition);
                    node.References.Add(exitTransition.Target);
                }
            }

            //==============================
            // Edge Errors
            //==============================
            if (exitTransition.EdgeMode != null)
            {

                if (exitTransition.EdgeMode.EdgeMode != EdgeMode.Goto)
                {

                    if (exitTransition.Target?.Declaration is ChoiceNodeSymbol)
                    {
                        _diagnostics.Add(new Diagnostic(
                            exitTransition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo,
                            exitTransition.Target.Name));
                    }

                    if (exitTransition.Target?.Declaration is ExitNodeSymbol)
                    {
                        _diagnostics.Add(new Diagnostic(
                            exitTransition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo,
                            exitTransition.Target.Name));
                    }

                    if (exitTransition.Target?.Declaration is EndNodeSymbol)
                    {
                        _diagnostics.Add(new Diagnostic(
                            exitTransition.EdgeMode.Location,
                            DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo,
                            exitTransition.Target.Name));
                    }
                }
            }

            //==============================
            // Condition Clause Errors
            //==============================
            if (exitTransition.Syntax.ConditionClause != null) {
                
                if(!(exitTransition.Syntax.ConditionClause is IfConditionClauseSyntax)) {

                    _diagnostics.Add(new Diagnostic(
                               exitTransition.Syntax.ConditionClause.GetLocation(),
                               DiagnosticDescriptors.Semantic.Nav0221OnlyIfConditionsAllowedInExitTransitions));
                }
                
            }

            _taskDefinition.ExitTransitions.Add(exitTransition);
        }

        #endregion

        void VerifyTaskDefinition() {

            //==============================
            //  Init Node Errors
            //==============================
            foreach(var initNode in _taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
                if (!initNode.Outgoings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        initNode.Alias?.Location ??  initNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0109InitNode0HasNoOutgoingEdges,
                        initNode.Name));
                }
            }

            //==============================
            //  Exit Node Errors
            //==============================
            foreach (var exitNode in _taskDefinition.NodeDeclarations.OfType<IExitNodeSymbol>()) {
                if (!exitNode.Incomings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        exitNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0107ExitNode0HasNoIncomingEdges,
                        exitNode.Name));
                }
            }

            //==============================
            //  End Node Errors
            //==============================
            foreach (var endNode in _taskDefinition.NodeDeclarations.OfType<IEndNodeSymbol>()) {
                if (!endNode.Incomings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                         endNode.Location,
                         DiagnosticDescriptors.Semantic.Nav0108EndNodeHasNoIncomingEdges,
                         endNode.Name));
                }
            }            

            //==============================
            //  Choice Node Errors
            //==============================
            foreach (var choiceNode in _taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if(!choiceNode.References.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        choiceNode.Syntax.GetLocation(),
                        DiagnosticDescriptors.DeadCode.Nav1009ChoiceNode0NotRequired,
                        choiceNode.Name));

                } else if(!choiceNode.Incomings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        choiceNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0111ChoiceNode0HasNoIncomingEdges,
                        choiceNode.Name));

                    if(choiceNode.Outgoings.Any()) {

                        _diagnostics.Add(new Diagnostic(
                            choiceNode.Outgoings.First().Location,
                            choiceNode.Outgoings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1007ChoiceNode0HasNoIncomingEdges,
                            choiceNode.Name));
                    }

                } else if(!choiceNode.Outgoings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        choiceNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0112ChoiceNode0HasNoOutgoingEdges,
                        choiceNode.Name));

                    if(choiceNode.Incomings.Any()) {
                        _diagnostics.Add(new Diagnostic(
                            choiceNode.Incomings.First().Location,
                            choiceNode.Incomings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1008ChoiceNode0HasNoOutgoingEdges,
                            choiceNode.Name));
                    }  
                }
            }

            //==============================
            //  Task Node Errors
            //==============================
            foreach (var taskNode in _taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if(!taskNode.References.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        taskNode.Syntax.GetLocation(),
                        DiagnosticDescriptors.DeadCode.Nav1012TaskNode0NotRequired,
                        taskNode.Name));

                } else {

                    if(!taskNode.Incomings.Any()) {

                        _diagnostics.Add(new Diagnostic(
                            taskNode.Location,
                            DiagnosticDescriptors.Semantic.Nav0113TaskNode0HasNoIncomingEdges,
                            taskNode.Name));

                        if (taskNode.Outgoings.Any()) {
                            _diagnostics.Add(new Diagnostic(
                                taskNode.Outgoings.First().Location,
                                taskNode.Outgoings.Select(edge => edge.Location).Skip(1),
                                DiagnosticDescriptors.DeadCode.Nav1010TaskNode0HasNoIncomingEdges,
                                taskNode.Name));
                        }                        
                    }

                    //==============================
                    // Exit Errors
                    //==============================
                    if (taskNode.Declaration == null) {
                        continue;
                    }

                    var expectedExits = taskNode.Declaration.Exits().OrderBy(cp => cp.Name);
                    var actualExits   = taskNode.Outgoings
                                                .Select(et => et.ConnectionPoint)
                                                .Where(cp => cp != null)
                                                .ToList();

                    foreach (var expectedExit in expectedExits) {

                        if (!actualExits.Exists(cpRef => cpRef.Declaration == expectedExit)) {

                            _diagnostics.Add(new Diagnostic(
                                taskNode.Location,
                                taskNode.Incomings
                                        .Select(edge => edge.Target)
                                        .Where(nodeReference  => nodeReference != null)
                                        .Select(nodeReference => nodeReference.Location),
                                DiagnosticDescriptors.Semantic.Nav0025NoOutgoingEdgeForExit0Declared,
                                expectedExit.Name));                  
                        }
                    }

                    foreach (var duplicate in actualExits.GroupBy(e => e.Name).Where(g => g.Count() > 1)) {
                        // TODO Additional Locations
                        foreach (var exit in duplicate) {
                            _diagnostics.Add(new Diagnostic(
                                exit.Location,
                                DiagnosticDescriptors.Semantic.Nav0024OutgoingEdgeForExit0AlreadyDeclared,
                                exit.Name));
                        }
                    }
                }
            }

            //==============================
            //  Dialog Node Errors
            //==============================
            foreach (var dialogNode in _taskDefinition.NodeDeclarations.OfType<IDialogNodeSymbol>()) {

                if (!dialogNode.References.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        dialogNode.Syntax.GetLocation(),
                        DiagnosticDescriptors.DeadCode.Nav1014DialogNode0NotRequired,
                        dialogNode.Name));

                } else if (!dialogNode.Incomings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        dialogNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0114DialogNode0HasNoIncomingEdges,
                        dialogNode.Name));

                    if (dialogNode.Outgoings.Any()) {
                        _diagnostics.Add(new Diagnostic(
                            dialogNode.Outgoings.First().Location,
                            dialogNode.Outgoings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1015DialogNode0HasNoIncomingEdges,
                            dialogNode.Name));
                    }                    
                } else if (!dialogNode.Outgoings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        dialogNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0115DialogNode0HasNoOutgoingEdges,
                        dialogNode.Name));

                    if (dialogNode.Incomings.Any()) {
                        _diagnostics.Add(new Diagnostic(
                            dialogNode.Incomings.First().Location,
                            dialogNode.Incomings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1016DialogNode0HasNoOutgoingEdges,
                            dialogNode.Name));
                    }                    
                }
            }

            //==============================
            //  View Node Errors
            //==============================
            foreach (var viewNode in _taskDefinition.NodeDeclarations.OfType<IViewNodeSymbol>()) {

                if (!viewNode.References.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        viewNode.Syntax.GetLocation(),
                        DiagnosticDescriptors.DeadCode.Nav1017ViewNode0NotRequired,
                        viewNode.Name));

                } else if (!viewNode.Incomings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        viewNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0116ViewNode0HasNoIncomingEdges,
                        viewNode.Name));

                    if (viewNode.Outgoings.Any()) {
                        _diagnostics.Add(new Diagnostic(
                            viewNode.Outgoings.First().Location,
                            viewNode.Outgoings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1018ViewNode0HasNoIncomingEdges,
                            viewNode.Name));
                    }                    
                } else if (!viewNode.Outgoings.Any()) {

                    _diagnostics.Add(new Diagnostic(
                        viewNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0117ViewNode0HasNoOutgoingEdges,
                        viewNode.Name));

                    if (viewNode.Incomings.Any()) {
                        _diagnostics.Add(new Diagnostic(
                            viewNode.Incomings.First().Location,
                            viewNode.Incomings.Select(edge => edge.Location).Skip(1),
                            DiagnosticDescriptors.DeadCode.Nav1019ViewNode0HasNoOutgoingEdges,
                            viewNode.Name));
                    }                    
                }
            }

            //==============================
            // Trigger Errors
            //==============================
            var triggerMap = new Dictionary<INodeSymbol, ITriggerSymbol>();
            foreach(var trans in _taskDefinition.Transitions) {
                // Nicht deklarierte Sourcenodes interessieren uns nicht
                var nodeSymbol = trans.Source?.Declaration;
                if (nodeSymbol == null) {
                    continue;
                }

                triggerMap.TryGetValue(nodeSymbol, out var existing);

                foreach(var trigger in trans.Triggers) {

                    if(existing!=null && trigger.Name== existing.Name) {

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
            foreach(var initNode in _taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {

                // Eigentlich darf eine Init-Node nur genau eine initTransition haben...
                foreach(var initTransition in initNode.Outgoings) {
                    foreach(var reachableCall in initTransition.GetReachableCalls()
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
            // TODO Nodes reachable by different Edges!
        }
        
        public static TaskDefinitionBuilderResult Build(TaskDefinitionSyntax taskDefinitionSyntax, IReadOnlySymbolCollection<TaskDeclarationSymbol> taskDeklarations) {
            var builder = new TaskDefinitionSymbolBuilder(taskDeklarations);
            builder.Visit(taskDefinitionSyntax);
            return new TaskDefinitionBuilderResult(builder._taskDefinition, builder._diagnostics);
        }
    }
}