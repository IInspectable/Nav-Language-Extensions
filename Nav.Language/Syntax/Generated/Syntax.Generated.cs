 
//==================================================
// HINWEIS: Diese Datei wurde am 14.05.2017 14:16:42
//			automatisch generiert!
//==================================================
namespace Pharmatechnik.Nav.Language {

    using System.Threading;

	public static class Syntax {
		
		public static DoClauseSyntax ParseDoClause(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (DoClauseSyntax)SyntaxTree.ParseTextCore(text, parser => parser.doClause(), filePath, null, cancellationToken).Root;		
		}

		public static GoToEdgeSyntax ParseGoToEdge(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (GoToEdgeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.goToEdge(), filePath, null, cancellationToken).Root;		
		}

		public static ArrayTypeSyntax ParseArrayType(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ArrayTypeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.arrayType(), filePath, null, cancellationToken).Root;		
		}

		public static ModalEdgeSyntax ParseModalEdge(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ModalEdgeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.modalEdge(), filePath, null, cancellationToken).Root;		
		}

		public static ParameterSyntax ParseParameter(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ParameterSyntax)SyntaxTree.ParseTextCore(text, parser => parser.parameter(), filePath, null, cancellationToken).Root;		
		}

		public static IdentifierSyntax ParseIdentifier(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IdentifierSyntax)SyntaxTree.ParseTextCore(text, parser => parser.identifier(), filePath, null, cancellationToken).Root;		
		}

		public static SimpleTypeSyntax ParseSimpleType(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (SimpleTypeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.simpleType(), filePath, null, cancellationToken).Root;		
		}

		public static GenericTypeSyntax ParseGenericType(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (GenericTypeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.genericType(), filePath, null, cancellationToken).Root;		
		}

		public static NonModalEdgeSyntax ParseNonModalEdge(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (NonModalEdgeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.nonModalEdge(), filePath, null, cancellationToken).Root;		
		}

		public static EndTargetNodeSyntax ParseEndTargetNode(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (EndTargetNodeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.endTargetNode(), filePath, null, cancellationToken).Root;		
		}

		public static ParameterListSyntax ParseParameterList(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ParameterListSyntax)SyntaxTree.ParseTextCore(text, parser => parser.parameterList(), filePath, null, cancellationToken).Root;		
		}

		public static SignalTriggerSyntax ParseSignalTrigger(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (SignalTriggerSyntax)SyntaxTree.ParseTextCore(text, parser => parser.signalTrigger(), filePath, null, cancellationToken).Root;		
		}

		public static StringLiteralSyntax ParseStringLiteral(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (StringLiteralSyntax)SyntaxTree.ParseTextCore(text, parser => parser.stringLiteral(), filePath, null, cancellationToken).Root;		
		}

		public static InitSourceNodeSyntax ParseInitSourceNode(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (InitSourceNodeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.initSourceNode(), filePath, null, cancellationToken).Root;		
		}

		public static TaskDefinitionSyntax ParseTaskDefinition(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (TaskDefinitionSyntax)SyntaxTree.ParseTextCore(text, parser => parser.taskDefinition(), filePath, null, cancellationToken).Root;		
		}

		public static CodeDeclarationSyntax ParseCodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static TaskDeclarationSyntax ParseTaskDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (TaskDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.taskDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static IncludeDirectiveSyntax ParseIncludeDirective(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IncludeDirectiveSyntax)SyntaxTree.ParseTextCore(text, parser => parser.includeDirective(), filePath, null, cancellationToken).Root;		
		}

		public static IfConditionClauseSyntax ParseIfConditionClause(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IfConditionClauseSyntax)SyntaxTree.ParseTextCore(text, parser => parser.ifConditionClause(), filePath, null, cancellationToken).Root;		
		}

		public static ArrayRankSpecifierSyntax ParseArrayRankSpecifier(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ArrayRankSpecifierSyntax)SyntaxTree.ParseTextCore(text, parser => parser.arrayRankSpecifier(), filePath, null, cancellationToken).Root;		
		}

		public static CodeGenerationUnitSyntax ParseCodeGenerationUnit(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeGenerationUnitSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeGenerationUnit(), filePath, null, cancellationToken).Root;		
		}

		public static EndNodeDeclarationSyntax ParseEndNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (EndNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.endNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static SpontaneousTriggerSyntax ParseSpontaneousTrigger(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (SpontaneousTriggerSyntax)SyntaxTree.ParseTextCore(text, parser => parser.spontaneousTrigger(), filePath, null, cancellationToken).Root;		
		}

		public static CodeBaseDeclarationSyntax ParseCodeBaseDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeBaseDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeBaseDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static ElseConditionClauseSyntax ParseElseConditionClause(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ElseConditionClauseSyntax)SyntaxTree.ParseTextCore(text, parser => parser.elseConditionClause(), filePath, null, cancellationToken).Root;		
		}

		public static ExitNodeDeclarationSyntax ParseExitNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ExitNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.exitNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static InitNodeDeclarationSyntax ParseInitNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (InitNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.initNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static TaskNodeDeclarationSyntax ParseTaskNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (TaskNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.taskNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static ViewNodeDeclarationSyntax ParseViewNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ViewNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.viewNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static CodeUsingDeclarationSyntax ParseCodeUsingDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeUsingDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeUsingDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static IdentifierSourceNodeSyntax ParseIdentifierSourceNode(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IdentifierSourceNodeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.identifierSourceNode(), filePath, null, cancellationToken).Root;		
		}

		public static IdentifierTargetNodeSyntax ParseIdentifierTargetNode(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IdentifierTargetNodeSyntax)SyntaxTree.ParseTextCore(text, parser => parser.identifierTargetNode(), filePath, null, cancellationToken).Root;		
		}

		public static NodeDeclarationBlockSyntax ParseNodeDeclarationBlock(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (NodeDeclarationBlockSyntax)SyntaxTree.ParseTextCore(text, parser => parser.nodeDeclarationBlock(), filePath, null, cancellationToken).Root;		
		}

		public static TransitionDefinitionSyntax ParseTransitionDefinition(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (TransitionDefinitionSyntax)SyntaxTree.ParseTextCore(text, parser => parser.transitionDefinition(), filePath, null, cancellationToken).Root;		
		}

		public static ChoiceNodeDeclarationSyntax ParseChoiceNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ChoiceNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.choiceNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static CodeParamsDeclarationSyntax ParseCodeParamsDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeParamsDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeParamsDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static CodeResultDeclarationSyntax ParseCodeResultDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeResultDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeResultDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static DialogNodeDeclarationSyntax ParseDialogNodeDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (DialogNodeDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.dialogNodeDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static ElseIfConditionClauseSyntax ParseElseIfConditionClause(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ElseIfConditionClauseSyntax)SyntaxTree.ParseTextCore(text, parser => parser.elseIfConditionClause(), filePath, null, cancellationToken).Root;		
		}

		public static IdentifierOrStringListSyntax ParseIdentifierOrStringList(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (IdentifierOrStringListSyntax)SyntaxTree.ParseTextCore(text, parser => parser.identifierOrStringList(), filePath, null, cancellationToken).Root;		
		}

		public static CodeNamespaceDeclarationSyntax ParseCodeNamespaceDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeNamespaceDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeNamespaceDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static ExitTransitionDefinitionSyntax ParseExitTransitionDefinition(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (ExitTransitionDefinitionSyntax)SyntaxTree.ParseTextCore(text, parser => parser.exitTransitionDefinition(), filePath, null, cancellationToken).Root;		
		}

		public static CodeGenerateToDeclarationSyntax ParseCodeGenerateToDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeGenerateToDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeGenerateToDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static TransitionDefinitionBlockSyntax ParseTransitionDefinitionBlock(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (TransitionDefinitionBlockSyntax)SyntaxTree.ParseTextCore(text, parser => parser.transitionDefinitionBlock(), filePath, null, cancellationToken).Root;		
		}

		public static CodeDoNotInjectDeclarationSyntax ParseCodeDoNotInjectDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeDoNotInjectDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeDoNotInjectDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static CodeAbstractMethodDeclarationSyntax ParseCodeAbstractMethodDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeAbstractMethodDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeAbstractMethodDeclaration(), filePath, null, cancellationToken).Root;		
		}

		public static CodeNotImplementedDeclarationSyntax ParseCodeNotImplementedDeclaration(string text, string filePath = null, CancellationToken cancellationToken = default(CancellationToken)) {
			return (CodeNotImplementedDeclarationSyntax)SyntaxTree.ParseTextCore(text, parser => parser.codeNotImplementedDeclaration(), filePath, null, cancellationToken).Root;		
		}

	}
}
