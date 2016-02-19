#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

using Pharmatechnik.Nav.Language.Generated;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    sealed class NavGrammarVisitor : NavGrammarBaseVisitor<SyntaxNode> {

        readonly List<SyntaxToken> _tokens;

        public NavGrammarVisitor(int expectedTokenCount) {
            _tokens = new List<SyntaxToken>(capacity: expectedTokenCount);
        }
        
        public List<SyntaxToken> Tokens {
            get { return _tokens; }
        }

        #region CodeGenerationUnit

        public override SyntaxNode VisitCodeGenerationUnit([NotNull] NavGrammarParser.CodeGenerationUnitContext context) {

            var node = new CodeGenerationUnitSyntax(
                CreateExtent(context),
                codeNamespaceDeclaration: 
                context.codeNamespaceDeclaration()
                       .Optional(VisitCodeNamespaceDeclaration)
                       .OfSyntaxType<CodeNamespaceDeclarationSyntax>(),
                codeUsingDeclarations: 
                context.codeUsingDeclaration()
                       .ZeroOrMore(VisitCodeUsingDeclaration)
                       .OfSyntaxType<CodeUsingDeclarationSyntax>()
                       .ToList(),
                memberDeclarations:                
                context.memberDeclaration()
                       .ZeroOrMore(VisitMemberDeclaration)
                       .OfSyntaxType<MemberDeclarationSyntax>()
                       .ToList()
              );

            CreateToken(node, context.Eof(), SyntaxTokenClassification.Whitespace);

            return node;
        }

        public override SyntaxNode VisitMemberDeclaration(NavGrammarParser.MemberDeclarationContext context) {
            if (context.includeDirective() != null) {
                return VisitIncludeDirective(context.includeDirective());
            }
            if (context.taskDeclaration() != null) {
                return VisitTaskDeclaration(context.taskDeclaration());
            }
            if (context.taskDefinition() != null) {
                return VisitTaskDefinition(context.taskDefinition());
            }
            return null;
        }

        #endregion

        #region CodeNamespaceDeclaration

        public override SyntaxNode VisitCodeNamespaceDeclaration([NotNull] NavGrammarParser.CodeNamespaceDeclarationContext context) {
            
            var node = new CodeNamespaceDeclarationSyntax(
                extent:CreateExtent(context),
                namespaceSyntax:
                context.identifierOrString()
                       .Optional(VisitIdentifierOrString)
                       .OfSyntaxType<IdentifierOrStringSyntax>()
                       );

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.NamespaceprefixKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeUsingDeclaration

        public override SyntaxNode VisitCodeUsingDeclaration(NavGrammarParser.CodeUsingDeclarationContext context) {
            
            var node = new CodeUsingDeclarationSyntax(
                extent:CreateExtent(context),
                namespaceSyntax:
                context.identifierOrString()
                       .Optional(VisitIdentifierOrString)
                       .OfSyntaxType<IdentifierOrStringSyntax>()
                       );

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.UsingKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region IncludeDirective

        public override SyntaxNode VisitIncludeDirective(NavGrammarParser.IncludeDirectiveContext context) {
            
            var node = new IncludeDirectiveSyntax(
                CreateExtent(context));

            CreateToken(node, context.TaskrefKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.StringLiteral(), SyntaxTokenClassification.StringLiteral);
            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region TaskDeclaration

        public override SyntaxNode VisitTaskDeclaration(NavGrammarParser.TaskDeclarationContext context) {
            
            var node = new TaskDeclarationSyntax(
                CreateExtent(context),
                codeNamespaceDeclaration: 
                context.codeNamespaceDeclaration()
                       .Optional(VisitCodeNamespaceDeclaration)
                       .OfSyntaxType<CodeNamespaceDeclarationSyntax>(),
                codeNotImplementedDeclaration: 
                context.codeNotImplementedDeclaration()
                       .Optional(VisitCodeNotImplementedDeclaration)
                       .OfSyntaxType<CodeNotImplementedDeclarationSyntax>(),
                codeResultDeclaration: 
                context.codeResultDeclaration()
                       .Optional(VisitCodeResultDeclaration)
                       .OfSyntaxType<CodeResultDeclarationSyntax>(),
                connectionPointNodeDeclarations: 
                context.connectionPointNodeDeclaration()
                       .ZeroOrMore(VisitConnectionPointNodeDeclaration)
                       .OfSyntaxType<ConnectionPointNodeSyntax>()
                       .ToList()
                );

            CreateToken(node, context.TaskrefKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier(),     SyntaxTokenClassification.TaskName);
            CreateToken(node, context.OpenBrace(),      SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CloseBrace(),     SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitConnectionPointNodeDeclaration(NavGrammarParser.ConnectionPointNodeDeclarationContext context) {
            if (context.initNodeDeclaration() != null) {
                return VisitInitNodeDeclaration(context.initNodeDeclaration());
            }
            if (context.exitNodeDeclaration() != null) {
                return VisitExitNodeDeclaration(context.exitNodeDeclaration());
            }
            if (context.endNodeDeclaration() != null) {
                return VisitEndNodeDeclaration(context.endNodeDeclaration());
            }
            return null;
        }

        #endregion

        #region TaskDefinition

        public override SyntaxNode VisitTaskDefinition(NavGrammarParser.TaskDefinitionContext context) {
            
            var node = new TaskDefinitionSyntax(
                CreateExtent(context),
                codeDeclaration: 
                context.codeDeclaration()
                       .Optional(VisitCodeDeclaration)
                       .OfSyntaxType<CodeDeclarationSyntax>(),
                codeBaseDeclaration: 
                context.codeBaseDeclaration()
                       .Optional(VisitCodeBaseDeclaration)
                       .OfSyntaxType<CodeBaseDeclarationSyntax>(),
                codeGenerateToDeclaration:
                context.codeGenerateToDeclaration()
                       .Optional(VisitCodeGenerateToDeclaration)
                       .OfSyntaxType<CodeGenerateToDeclarationSyntax>(),
                codeParamsDeclaration: 
                context.codeParamsDeclaration()
                       .Optional(VisitCodeParamsDeclaration)
                       .OfSyntaxType<CodeParamsDeclarationSyntax>(),
                codeResultDeclaration: 
                context.codeResultDeclaration()
                       .Optional(VisitCodeResultDeclaration)
                       .OfSyntaxType<CodeResultDeclarationSyntax>(),
                nodeDeclarationBlock:
                context.nodeDeclarationBlock()
                       .Optional(VisitNodeDeclarationBlock)
                       .OfSyntaxType<NodeDeclarationBlockSyntax>(),
                transitionDefinitionBlock:
                context.transitionDefinitionBlock()
                       .Optional(VisitTransitionDefinitionBlock)
                       .OfSyntaxType<TransitionDefinitionBlockSyntax>()
                );

            CreateToken(node, context.TaskKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier() , SyntaxTokenClassification.TaskName);
            CreateToken(node, context.OpenBrace()  , SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CloseBrace() , SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion
        
        #region Node Declarations

        public override SyntaxNode VisitNodeDeclarationBlock(NavGrammarParser.NodeDeclarationBlockContext context) {
            
            var node= new NodeDeclarationBlockSyntax(
                CreateExtent(context),
                nodeDeclarations: 
                context.nodeDeclaration()
                       .ZeroOrMore(VisitNodeDeclaration)
                       .OfSyntaxType<NodeDeclarationSyntax>()
                       .ToList()
                );

            return node;
        }

        public override SyntaxNode VisitNodeDeclaration(NavGrammarParser.NodeDeclarationContext context) {
            if (context.connectionPointNodeDeclaration() != null) {
                return VisitConnectionPointNodeDeclaration(context.connectionPointNodeDeclaration());
            }
            if (context.taskNodeDeclaration() != null) {
                return VisitTaskNodeDeclaration(context.taskNodeDeclaration());
            }
            if (context.choiceNodeDeclaration() != null) {
                return VisitChoiceNodeDeclaration(context.choiceNodeDeclaration());
            }
            if (context.dialogNodeDeclaration() != null) {
                return VisitDialogNodeDeclaration(context.dialogNodeDeclaration());
            }
            if (context.viewNodeDeclaration() != null) {
                return VisitViewNodeDeclaration(context.viewNodeDeclaration());
            }
            return null;
        }

        public override SyntaxNode VisitInitNodeDeclaration(NavGrammarParser.InitNodeDeclarationContext context) {

            var node = new InitNodeDeclarationSyntax(CreateExtent(context),  
                    codeAbstractMethodDeclaration: 
                    context.codeAbstractMethodDeclaration()
                           .Optional(VisitCodeAbstractMethodDeclaration)
                           .OfSyntaxType<CodeAbstractMethodDeclarationSyntax>(),
                    codeParamsDeclaration: 
                    context.codeParamsDeclaration()
                           .Optional(VisitCodeParamsDeclaration)
                           .OfSyntaxType<CodeParamsDeclarationSyntax>(),
                    doClause:
                    context.doClause()
                           .Optional(VisitDoClause)
                           .OfSyntaxType<DoClauseSyntax>()
                );

            CreateToken(node, context.InitKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier); // Name der Initfunktion
            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitExitNodeDeclaration(NavGrammarParser.ExitNodeDeclarationContext context) {
            
            var node = new ExitNodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.ExitKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);
            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitEndNodeDeclaration(NavGrammarParser.EndNodeDeclarationContext context) {
            
            var node = new EndNodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.EndKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitTaskNodeDeclaration(NavGrammarParser.TaskNodeDeclarationContext context) {
            
            var node = new TaskNodeDeclarationSyntax(CreateExtent(context), 
                    codeDoNotInjectDeclaration: 
                    context.codeDoNotInjectDeclaration()
                           .Optional(VisitCodeDoNotInjectDeclaration)
                           .OfSyntaxType<CodeDoNotInjectDeclarationSyntax>(),
                    codeAbstractMethodDeclaration: 
                    context.codeAbstractMethodDeclaration()
                           .Optional(VisitCodeAbstractMethodDeclaration)
                           .OfSyntaxType<CodeAbstractMethodDeclarationSyntax>()
                );

            CreateToken(node, context.TaskKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier(0), SyntaxTokenClassification.TaskName);
            CreateToken(node, context.Identifier(1), SyntaxTokenClassification.Identifier);
            CreateToken(node, context.Semicolon()  , SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitChoiceNodeDeclaration(NavGrammarParser.ChoiceNodeDeclarationContext context) {
            
            var node = new ChoiceNodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.ChoiceKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);
            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitDialogNodeDeclaration(NavGrammarParser.DialogNodeDeclarationContext context) {
            
            var node = new DialogNodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.DialogKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier()   , SyntaxTokenClassification.FormName);
            CreateToken(node, context.Semicolon()    , SyntaxTokenClassification.Punctuation);
            
            return node;
        }

        public override SyntaxNode VisitViewNodeDeclaration(NavGrammarParser.ViewNodeDeclarationContext context) {

            var node = new ViewNodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.ViewKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Identifier() , SyntaxTokenClassification.FormName);
            CreateToken(node, context.Semicolon()  , SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region TransitionDefinitionBlock

        public override SyntaxNode VisitTransitionDefinitionBlock(NavGrammarParser.TransitionDefinitionBlockContext context) {
            var node = new TransitionDefinitionBlockSyntax(
                    extent: 
                    CreateExtent(context),
                    transitionDefinitions:
                            context.transitionDefinition()
                                    .ZeroOrMore(VisitTransitionDefinition)
                                    .OfSyntaxType<TransitionDefinitionSyntax>()
                                    .ToList(),
                    exitTransitionDefinitions:
                    context.exitTransitionDefinition()
                                    .ZeroOrMore(VisitExitTransitionDefinition)
                                    .OfSyntaxType<ExitTransitionDefinitionSyntax>()
                                    .ToList()
                    );

            return node;
        }

        public override SyntaxNode VisitTransitionDefinition(NavGrammarParser.TransitionDefinitionContext context) {
            
            var node = new TransitionDefinitionSyntax(
                    extent:
                    CreateExtent(context),
                    sourceNode:
                        context.sourceNode()
                               .Optional(VisitSourceNode)
                               .OfSyntaxType<SourceNodeSyntax>(),
                    edgeSyntax: 
                        context.edge()
                               .Optional(VisitEdge)
                               .OfSyntaxType<EdgeSyntax>(),
                    targetNode:
                        context.targetNode()
                               .Optional(VisitTargetNode)
                               .OfSyntaxType<TargetNodeSyntax>(),
                    trigger:
                        context.trigger()
                               .Optional(VisitTrigger)
                               .OfSyntaxType<TriggerSyntax>(),
                    conditionClause:
                        context.conditionClause()
                               .Optional(VisitConditionClause)
                               .OfSyntaxType<ConditionClauseSyntax>(),
                    doClause:
                        context.doClause()
                               .Optional(VisitDoClause)
                               .OfSyntaxType<DoClauseSyntax>()
                    );

            CreateToken(node, context.Semicolon(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitSourceNode(NavGrammarParser.SourceNodeContext context) {

            if (context.initSourceNode() != null) {
                return VisitInitSourceNode(context.initSourceNode());
            }

            if (context.identifierSourceNode() != null) {
                return VisitIdentifierSourceNode(context.identifierSourceNode());
            }
            return null;
        }

        public override SyntaxNode VisitInitSourceNode(NavGrammarParser.InitSourceNodeContext context) {
            var node = new InitSourceNodeSyntax(CreateExtent(context));
            CreateToken(node, context.InitKeyword(), SyntaxTokenClassification.Keyword);
            return node;
        }

        public override SyntaxNode VisitIdentifierSourceNode(NavGrammarParser.IdentifierSourceNodeContext context) {
            var node = new IdentifierSourceNodeSyntax(CreateExtent(context));
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);
            return node;
        }
        
        public override SyntaxNode VisitTargetNode(NavGrammarParser.TargetNodeContext context) {

            if (context.endTargetNode() != null) {
                return VisitEndTargetNode(context.endTargetNode());
            }
            if (context.identifierTargetNode() != null) {
                return VisitIdentifierTargetNode(context.identifierTargetNode());
            }
            return null;
        }

        public override SyntaxNode VisitEndTargetNode(NavGrammarParser.EndTargetNodeContext context) {
            var node = new EndTargetNodeSyntax(extent:CreateExtent(context));
            CreateToken(node, context.EndKeyword(), SyntaxTokenClassification.Keyword);
            return node;
        }

        public override SyntaxNode VisitIdentifierTargetNode(NavGrammarParser.IdentifierTargetNodeContext context) {
            var node = new IdentifierTargetNodeSyntax(
                 extent:
                 CreateExtent(context),
                 identifierOrStringList:
                    context.identifierOrStringList()
                           .Optional(VisitIdentifierOrStringList)
                           .OfSyntaxType<IdentifierOrStringListSyntax>()
            );

            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);
            CreateToken(node, context.OpenParen(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CloseParen(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitTrigger(NavGrammarParser.TriggerContext context) {
            if (context.signalTrigger() != null) {
                return VisitSignalTrigger(context.signalTrigger());
            }
            if (context.spontaneousTrigger() != null) {
                return VisitSpontaneousTrigger(context.spontaneousTrigger());
            }
            return null;
        }

        public override SyntaxNode VisitSpontaneousTrigger(NavGrammarParser.SpontaneousTriggerContext context) {
            var node = new SpontaneousTriggerSyntax(
                     extent:
                     CreateExtent(context)                    
                 );
            
            CreateToken(node, context.SpontKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.SpontaneousKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        public override SyntaxNode VisitSignalTrigger(NavGrammarParser.SignalTriggerContext context) {
            
            var node = new SignalTriggerSyntax(
                    extent:
                    CreateExtent(context),
                    identifierOrStringList:
                    context.identifierOrStringList()
                               .Optional(VisitIdentifierOrStringList)
                               .OfSyntaxType<IdentifierOrStringListSyntax>()
                );

            CreateToken(node, context.OnKeyword(), SyntaxTokenClassification.Keyword);
            
            return node;
        }

        public override SyntaxNode VisitConditionClause([NotNull] NavGrammarParser.ConditionClauseContext context) {

            if (context.ifConditionClause() != null) {
                return VisitIfConditionClause(context.ifConditionClause());
            }
            if (context.elseIfConditionClause() != null) {
                return VisitElseIfConditionClause(context.elseIfConditionClause());
            }
            if (context.elseConditionClause() != null) {
                return VisitElseConditionClause(context.elseConditionClause());
            }
            return null;         
        }

        public override SyntaxNode VisitIfConditionClause(NavGrammarParser.IfConditionClauseContext context) {
            var node = new IfConditionClauseSyntax(
                    extent:CreateExtent(context),
                    identifierOrString:
                    context.identifierOrString()
                               .Optional(VisitIdentifierOrString)
                               .OfSyntaxType<IdentifierOrStringSyntax>());

            CreateToken(node, context.IfKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        public override SyntaxNode VisitElseConditionClause(NavGrammarParser.ElseConditionClauseContext context) {
            var node = new ElseConditionClauseSyntax(extent: CreateExtent(context));

            CreateToken(node, context.ElseKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        public override SyntaxNode VisitElseIfConditionClause(NavGrammarParser.ElseIfConditionClauseContext context) {
            var node = new ElseIfConditionClauseSyntax(
                    extent: CreateExtent(context),
                    elseCondition:
                    context.elseConditionClause()
                           .Optional(VisitElseConditionClause)
                           .OfSyntaxType<ElseConditionClauseSyntax>(),
                    ifCondition:
                    context.ifConditionClause()
                           .Optional(VisitIfConditionClause)
                           .OfSyntaxType<IfConditionClauseSyntax>());

            return node;
        }

        public override SyntaxNode VisitDoClause(NavGrammarParser.DoClauseContext context) {
            
            var node = new DoClauseSyntax(
                    extent:
                    CreateExtent(context),
                    identifierOrString:
                    context.identifierOrString()
                               .Optional(VisitIdentifierOrString)
                               .OfSyntaxType<IdentifierOrStringSyntax>()
                    );

            CreateToken(node, context.DoKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }
        
        public override SyntaxNode VisitExitTransitionDefinition(NavGrammarParser.ExitTransitionDefinitionContext context) {

            var node = new ExitTransitionDefinitionSyntax(
                extent:
                    CreateExtent(context),
                sourceNode:
                    context.identifierSourceNode()
                           .Optional(VisitIdentifierSourceNode)
                           .OfSyntaxType<IdentifierSourceNodeSyntax>(),
                edge:
                    context.edge()
                           .Optional(VisitEdge)
                           .OfSyntaxType<EdgeSyntax>(),
                targetNode:
                    context.targetNode()
                           .Optional(VisitTargetNode)
                           .OfSyntaxType<TargetNodeSyntax>(),
                conditionClause:
                    context.conditionClause()
                           .Optional(VisitConditionClause)
                           .OfSyntaxType<ConditionClauseSyntax>(),
                doClause:
                    context.doClause()
                           .Optional(VisitDoClause)
                           .OfSyntaxType<DoClauseSyntax>()
                );
            
            CreateToken(node, context.Colon(),      SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier); // ExitNode:Exit
            CreateToken(node, context.Semicolon(),  SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion   

        #region Edge

        public override SyntaxNode VisitEdge(NavGrammarParser.EdgeContext context) {

            if (context.nonModalEdge() != null) {
                return VisitNonModalEdge(context.nonModalEdge());
            }
            if (context.goToEdge() != null) {
                return VisitGoToEdge(context.goToEdge());
            }
            if (context.modalEdge() != null) {
                return VisitModalEdge(context.modalEdge());
            }
            return null;
        }

        #region Overrides of NavGrammarBaseVisitor<SyntaxNode>

        public override SyntaxNode VisitModalEdge(NavGrammarParser.ModalEdgeContext context) {

            var node = new ModalEdgeSyntax(CreateExtent(context));

            CreateToken(node, context.ModalEdgeKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        public override SyntaxNode VisitGoToEdge(NavGrammarParser.GoToEdgeContext context) {

            var node = new GoToEdgeSyntax(CreateExtent(context));

            CreateToken(node, context.GoToEdgeKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        public override SyntaxNode VisitNonModalEdge(NavGrammarParser.NonModalEdgeContext context) {

            var node = new NonModalEdgeSyntax(CreateExtent(context));

            CreateToken(node, context.NonModalEdgeKeyword(), SyntaxTokenClassification.Keyword);

            return node;
        }

        #endregion

        #endregion

        #region CodeNotImplementedDeclaration

        public override SyntaxNode VisitCodeNotImplementedDeclaration(NavGrammarParser.CodeNotImplementedDeclarationContext context) {
            
            var node= new CodeNotImplementedDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.NotimplementedKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeDoNotInjectDeclaration

        public override SyntaxNode VisitCodeDoNotInjectDeclaration(NavGrammarParser.CodeDoNotInjectDeclarationContext context) {
            
            var node= new CodeDoNotInjectDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.DonotinjectKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeAbstractMethodDeclaration

        public override SyntaxNode VisitCodeAbstractMethodDeclaration(NavGrammarParser.CodeAbstractMethodDeclarationContext context) {
            
            var node = new CodeAbstractMethodDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.AbstractmethodKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeDeclaration

        public override SyntaxNode VisitCodeDeclaration(NavGrammarParser.CodeDeclarationContext context) {

            var node = new CodeDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CodeKeyword(), SyntaxTokenClassification.Keyword);
            CreateTokens(node, context.StringLiteral(), SyntaxTokenClassification.StringLiteral);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeBaseDeclaration

        public override SyntaxNode VisitCodeBaseDeclaration(NavGrammarParser.CodeBaseDeclarationContext context) {

            var node = new CodeBaseDeclarationSyntax(CreateExtent(context),
                context.codeType()
                    .ZeroOrMore(VisitCodeType)
                    .OfSyntaxType<CodeTypeSyntax>()
                    .ToList()
                );

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.BaseKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.Colon(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.Comma(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeGenerateToDeclaration

        public override SyntaxNode VisitCodeGenerateToDeclaration(NavGrammarParser.CodeGenerateToDeclarationContext context) {
            var node= new CodeGenerateToDeclarationSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.GeneratetoKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.StringLiteral(), SyntaxTokenClassification.StringLiteral);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeParamsDeclaration

        public override SyntaxNode VisitCodeParamsDeclaration(NavGrammarParser.CodeParamsDeclarationContext context) {

            var node = new CodeParamsDeclarationSyntax(CreateExtent(context),
                 parameterList:
                    context.parameterList()
                        .Optional(VisitParameterList)
                        .OfSyntaxType<ParameterListSyntax>()
                    );

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.ParamsKeyword(), SyntaxTokenClassification.Keyword);
            
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region ParameterList

        public override SyntaxNode VisitParameterList(NavGrammarParser.ParameterListContext context) {

            var node = new ParameterListSyntax(CreateExtent(context), 
                parameters: 
                context.parameter()
                       .ZeroOrMore(VisitParameter)
                       .OfSyntaxType<ParameterSyntax>()
                       .ToList()
                );

            CreateTokens(node, context.Comma(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region Parameter

        public override SyntaxNode VisitParameter([NotNull] NavGrammarParser.ParameterContext context) {
            var node = new ParameterSyntax(
                CreateExtent(context),
                type:
                    context.codeType()
                        .Optional(VisitCodeType)
                        .OfSyntaxType<CodeTypeSyntax>()
                );

            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);

            return node;
        }

        #endregion

        #region CodeResultDeclaration

        public override SyntaxNode VisitCodeResultDeclaration(NavGrammarParser.CodeResultDeclarationContext context) {

            var node = new CodeResultDeclarationSyntax(CreateExtent(context),
                result: context.parameter()
                    .Optional(VisitParameter)
                    .OfSyntaxType<ParameterSyntax>());

            CreateToken(node, context.OpenBracket(), SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.ResultKeyword(), SyntaxTokenClassification.Keyword);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region CodeType

        public override SyntaxNode VisitCodeType(NavGrammarParser.CodeTypeContext context) {


            if (context.simpleType() != null) {
                return VisitSimpleType(context.simpleType());
            }
            if (context.genericType() != null) {
                return VisitGenericType(context.genericType());
            }
            if (context.arrayType() != null) {
                return VisitArrayType(context.arrayType());
            }

            return null;
        }


        public override SyntaxNode VisitSimpleType(NavGrammarParser.SimpleTypeContext context) {

            var node = new SimpleTypeSyntax(CreateExtent(context));

            CreateToken(node, context.Identifier(), SyntaxTokenClassification.TypeName);

            return node;
        }

        public override SyntaxNode VisitArrayType(NavGrammarParser.ArrayTypeContext context) {

            CodeTypeSyntax type = null;
            if (context.simpleType() != null) {
                type = (CodeTypeSyntax)VisitSimpleType(context.simpleType());
            }
            if (context.genericType() != null) {
                type = (CodeTypeSyntax)VisitGenericType(context.genericType());
            }

            var node = new ArrayTypeSyntax(CreateExtent(context), type,
                context.arrayRankSpecifier()
                    .ZeroOrMore(VisitArrayRankSpecifier)
                    .OfSyntaxType<ArrayRankSpecifierSyntax>()
                    .ToList());

            return node;
        }

        #region Overrides of NavGrammarBaseVisitor<SyntaxNode>

        public override SyntaxNode VisitArrayRankSpecifier(NavGrammarParser.ArrayRankSpecifierContext context) {
            var node = new ArrayRankSpecifierSyntax(CreateExtent(context));

            CreateToken(node, context.OpenBracket(),  SyntaxTokenClassification.Punctuation);
            CreateToken(node, context.CloseBracket(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        public override SyntaxNode VisitGenericType(NavGrammarParser.GenericTypeContext context) {

            var node = new GenericTypeSyntax(CreateExtent(context), 
                genericArguments: 
                context.codeType()
                       .ZeroOrMore(VisitCodeType)
                       .OfSyntaxType<CodeTypeSyntax>()
                       .ToList()
                );

            CreateToken(node , context.Identifier() , SyntaxTokenClassification.TypeName);
            CreateToken(node , context.LessThan()   , SyntaxTokenClassification.Punctuation);
            CreateTokens(node, context.Comma()      , SyntaxTokenClassification.Punctuation);
            CreateToken(node , context.GreaterThan(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        #endregion

        #region IdentifierOrString

        public override SyntaxNode VisitIdentifierOrStringList(NavGrammarParser.IdentifierOrStringListContext context) {

            var node = new IdentifierOrStringListSyntax(CreateExtent(context),
                identifierOrStrings:
                context.identifierOrString()
                       .ZeroOrMore(VisitIdentifierOrString)
                       .OfSyntaxType<IdentifierOrStringSyntax>()
                       .ToList()
                );

            CreateTokens(node, context.Comma(), SyntaxTokenClassification.Punctuation);

            return node;
        }

        public override SyntaxNode VisitIdentifierOrString(NavGrammarParser.IdentifierOrStringContext context) {
            if (context.identifier() != null) {     
                return VisitIdentifier(context.identifier());
            }
            if (context.stringLiteral() != null) {
                return VisitStringLiteral(context.stringLiteral());
            }
            return null;
        }

        public override SyntaxNode VisitIdentifier(NavGrammarParser.IdentifierContext context) {
            var node = new IdentifierSyntax(CreateExtent(context));
            CreateToken(node, context.Identifier(), SyntaxTokenClassification.Identifier);
            return node;
        }

        public override SyntaxNode VisitStringLiteral(NavGrammarParser.StringLiteralContext context) {
            var node = new StringLiteralSyntax(CreateExtent(context));
            CreateToken(node, context.StringLiteral(), SyntaxTokenClassification.StringLiteral);
            return node;
        }
        
        #endregion

        #region Helper

        TextExtent CreateExtent(ParserRuleContext context) {
            return TextExtentFactory.CreateExtent(context);
        }

        void CreateTokens(SyntaxNode parent, IReadOnlyList<ITerminalNode> nodes, SyntaxTokenClassification classification) {
            foreach (var node in nodes) {
                CreateToken(parent, node, classification);
            }
        }

        void CreateToken(SyntaxNode parent, ITerminalNode node, SyntaxTokenClassification classification) {
            if (node == null) {
                return;
            }
            var token = SyntaxTokenFactory.CreateToken(node, classification, parent);
            if (!token.IsMissing) {
                _tokens.Add(token);
            }
        }

        #endregion
    }
}