 
//==================================================
// HINWEIS: Diese Datei wurde am 25.06.2016 12:33:27
//			automatisch generiert!
//==================================================
namespace Pharmatechnik.Nav.Language {

	partial class SyntaxNode {
		internal abstract void Accept(ISyntaxNodeVisitor visitor);
		internal abstract T Accept<T>(ISyntaxNodeVisitor<T> visitor);
	}

	partial class ArrayRankSpecifierSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitArrayRankSpecifier(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitArrayRankSpecifier(this);
		}
	}

	partial class ArrayTypeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitArrayType(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitArrayType(this);
		}
	}

	partial class ChoiceNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitChoiceNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitChoiceNodeDeclaration(this);
		}
	}

	partial class CodeAbstractMethodDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeAbstractMethodDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeAbstractMethodDeclaration(this);
		}
	}

	partial class CodeBaseDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeBaseDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeBaseDeclaration(this);
		}
	}

	partial class CodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeDeclaration(this);
		}
	}

	partial class CodeDoNotInjectDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeDoNotInjectDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeDoNotInjectDeclaration(this);
		}
	}

	partial class CodeGenerateToDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeGenerateToDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeGenerateToDeclaration(this);
		}
	}

	partial class CodeGenerationUnitSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeGenerationUnit(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeGenerationUnit(this);
		}
	}

	partial class CodeNamespaceDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeNamespaceDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeNamespaceDeclaration(this);
		}
	}

	partial class CodeNotImplementedDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeNotImplementedDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeNotImplementedDeclaration(this);
		}
	}

	partial class CodeParamsDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeParamsDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeParamsDeclaration(this);
		}
	}

	partial class CodeResultDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeResultDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeResultDeclaration(this);
		}
	}

	partial class CodeUsingDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitCodeUsingDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitCodeUsingDeclaration(this);
		}
	}

	partial class DialogNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitDialogNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitDialogNodeDeclaration(this);
		}
	}

	partial class DoClauseSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitDoClause(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitDoClause(this);
		}
	}

	partial class ElseConditionClauseSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitElseConditionClause(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitElseConditionClause(this);
		}
	}

	partial class ElseIfConditionClauseSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitElseIfConditionClause(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitElseIfConditionClause(this);
		}
	}

	partial class EndNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitEndNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitEndNodeDeclaration(this);
		}
	}

	partial class EndTargetNodeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitEndTargetNode(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitEndTargetNode(this);
		}
	}

	partial class ExitNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitExitNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitExitNodeDeclaration(this);
		}
	}

	partial class ExitTransitionDefinitionSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitExitTransitionDefinition(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitExitTransitionDefinition(this);
		}
	}

	partial class GenericTypeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitGenericType(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitGenericType(this);
		}
	}

	partial class GoToEdgeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitGoToEdge(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitGoToEdge(this);
		}
	}

	partial class IdentifierOrStringListSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIdentifierOrStringList(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIdentifierOrStringList(this);
		}
	}

	partial class IdentifierSourceNodeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIdentifierSourceNode(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIdentifierSourceNode(this);
		}
	}

	partial class IdentifierSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIdentifier(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIdentifier(this);
		}
	}

	partial class IdentifierTargetNodeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIdentifierTargetNode(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIdentifierTargetNode(this);
		}
	}

	partial class IfConditionClauseSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIfConditionClause(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIfConditionClause(this);
		}
	}

	partial class IncludeDirectiveSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitIncludeDirective(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitIncludeDirective(this);
		}
	}

	partial class InitNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitInitNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitInitNodeDeclaration(this);
		}
	}

	partial class InitSourceNodeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitInitSourceNode(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitInitSourceNode(this);
		}
	}

	partial class ModalEdgeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitModalEdge(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitModalEdge(this);
		}
	}

	partial class NodeDeclarationBlockSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitNodeDeclarationBlock(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitNodeDeclarationBlock(this);
		}
	}

	partial class NonModalEdgeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitNonModalEdge(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitNonModalEdge(this);
		}
	}

	partial class ParameterListSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitParameterList(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitParameterList(this);
		}
	}

	partial class ParameterSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitParameter(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitParameter(this);
		}
	}

	partial class SignalTriggerSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitSignalTrigger(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitSignalTrigger(this);
		}
	}

	partial class SimpleTypeSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitSimpleType(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitSimpleType(this);
		}
	}

	partial class SpontaneousTriggerSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitSpontaneousTrigger(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitSpontaneousTrigger(this);
		}
	}

	partial class StringLiteralSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitStringLiteral(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitStringLiteral(this);
		}
	}

	partial class TaskDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitTaskDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitTaskDeclaration(this);
		}
	}

	partial class TaskDefinitionSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitTaskDefinition(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitTaskDefinition(this);
		}
	}

	partial class TaskNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitTaskNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitTaskNodeDeclaration(this);
		}
	}

	partial class TransitionDefinitionBlockSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitTransitionDefinitionBlock(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitTransitionDefinitionBlock(this);
		}
	}

	partial class TransitionDefinitionSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitTransitionDefinition(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitTransitionDefinition(this);
		}
	}

	partial class ViewNodeDeclarationSyntax {
		internal override void Accept(ISyntaxNodeVisitor visitor) {
			visitor.VisitViewNodeDeclaration(this);
		}
		internal override T Accept<T>(ISyntaxNodeVisitor<T> visitor) {
			return visitor.VisitViewNodeDeclaration(this);
		}
	}

	public interface ISyntaxNodeVisitor {
		void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax); 
		void VisitArrayType(ArrayTypeSyntax arrayTypeSyntax); 
		void VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax); 
		void VisitCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax); 
		void VisitCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax); 
		void VisitCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax); 
		void VisitCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax); 
		void VisitCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax); 
		void VisitCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax); 
		void VisitCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax); 
		void VisitCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax); 
		void VisitCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax); 
		void VisitCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax); 
		void VisitCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax); 
		void VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax); 
		void VisitDoClause(DoClauseSyntax doClauseSyntax); 
		void VisitElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax); 
		void VisitElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax); 
		void VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax); 
		void VisitEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax); 
		void VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax); 
		void VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax); 
		void VisitGenericType(GenericTypeSyntax genericTypeSyntax); 
		void VisitGoToEdge(GoToEdgeSyntax goToEdgeSyntax); 
		void VisitIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax); 
		void VisitIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax); 
		void VisitIdentifier(IdentifierSyntax identifierSyntax); 
		void VisitIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax); 
		void VisitIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax); 
		void VisitIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax); 
		void VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax); 
		void VisitInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax); 
		void VisitModalEdge(ModalEdgeSyntax modalEdgeSyntax); 
		void VisitNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax); 
		void VisitNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax); 
		void VisitParameterList(ParameterListSyntax parameterListSyntax); 
		void VisitParameter(ParameterSyntax parameterSyntax); 
		void VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax); 
		void VisitSimpleType(SimpleTypeSyntax simpleTypeSyntax); 
		void VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax); 
		void VisitStringLiteral(StringLiteralSyntax stringLiteralSyntax); 
		void VisitTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax); 
		void VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax); 
		void VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax); 
		void VisitTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax); 
		void VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax); 
		void VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax); 
	}

	public abstract class SyntaxNodeVisitor: ISyntaxNodeVisitor {

		public void Visit(SyntaxNode node){
			node.Accept(this);
		}

		protected virtual void DefaultVisit(SyntaxNode node) {			
		}

		public virtual void VisitDoClause(DoClauseSyntax doClauseSyntax) {
			DefaultVisit(doClauseSyntax);
		}

		public virtual void VisitGoToEdge(GoToEdgeSyntax goToEdgeSyntax) {
			DefaultVisit(goToEdgeSyntax);
		}

		public virtual void VisitArrayType(ArrayTypeSyntax arrayTypeSyntax) {
			DefaultVisit(arrayTypeSyntax);
		}

		public virtual void VisitModalEdge(ModalEdgeSyntax modalEdgeSyntax) {
			DefaultVisit(modalEdgeSyntax);
		}

		public virtual void VisitParameter(ParameterSyntax parameterSyntax) {
			DefaultVisit(parameterSyntax);
		}

		public virtual void VisitIdentifier(IdentifierSyntax identifierSyntax) {
			DefaultVisit(identifierSyntax);
		}

		public virtual void VisitSimpleType(SimpleTypeSyntax simpleTypeSyntax) {
			DefaultVisit(simpleTypeSyntax);
		}

		public virtual void VisitGenericType(GenericTypeSyntax genericTypeSyntax) {
			DefaultVisit(genericTypeSyntax);
		}

		public virtual void VisitNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) {
			DefaultVisit(nonModalEdgeSyntax);
		}

		public virtual void VisitEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) {
			DefaultVisit(endTargetNodeSyntax);
		}

		public virtual void VisitParameterList(ParameterListSyntax parameterListSyntax) {
			DefaultVisit(parameterListSyntax);
		}

		public virtual void VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) {
			DefaultVisit(signalTriggerSyntax);
		}

		public virtual void VisitStringLiteral(StringLiteralSyntax stringLiteralSyntax) {
			DefaultVisit(stringLiteralSyntax);
		}

		public virtual void VisitInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) {
			DefaultVisit(initSourceNodeSyntax);
		}

		public virtual void VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {
			DefaultVisit(taskDefinitionSyntax);
		}

		public virtual void VisitCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) {
			DefaultVisit(codeDeclarationSyntax);
		}

		public virtual void VisitTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) {
			DefaultVisit(taskDeclarationSyntax);
		}

		public virtual void VisitIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) {
			DefaultVisit(includeDirectiveSyntax);
		}

		public virtual void VisitIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) {
			DefaultVisit(ifConditionClauseSyntax);
		}

		public virtual void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) {
			DefaultVisit(arrayRankSpecifierSyntax);
		}

		public virtual void VisitCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) {
			DefaultVisit(codeGenerationUnitSyntax);
		}

		public virtual void VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) {
			DefaultVisit(endNodeDeclarationSyntax);
		}

		public virtual void VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) {
			DefaultVisit(spontaneousTriggerSyntax);
		}

		public virtual void VisitCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) {
			DefaultVisit(codeBaseDeclarationSyntax);
		}

		public virtual void VisitElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) {
			DefaultVisit(elseConditionClauseSyntax);
		}

		public virtual void VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) {
			DefaultVisit(exitNodeDeclarationSyntax);
		}

		public virtual void VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) {
			DefaultVisit(initNodeDeclarationSyntax);
		}

		public virtual void VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) {
			DefaultVisit(taskNodeDeclarationSyntax);
		}

		public virtual void VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) {
			DefaultVisit(viewNodeDeclarationSyntax);
		}

		public virtual void VisitCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) {
			DefaultVisit(codeUsingDeclarationSyntax);
		}

		public virtual void VisitIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) {
			DefaultVisit(identifierSourceNodeSyntax);
		}

		public virtual void VisitIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) {
			DefaultVisit(identifierTargetNodeSyntax);
		}

		public virtual void VisitNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) {
			DefaultVisit(nodeDeclarationBlockSyntax);
		}

		public virtual void VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) {
			DefaultVisit(transitionDefinitionSyntax);
		}

		public virtual void VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) {
			DefaultVisit(choiceNodeDeclarationSyntax);
		}

		public virtual void VisitCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) {
			DefaultVisit(codeParamsDeclarationSyntax);
		}

		public virtual void VisitCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) {
			DefaultVisit(codeResultDeclarationSyntax);
		}

		public virtual void VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) {
			DefaultVisit(dialogNodeDeclarationSyntax);
		}

		public virtual void VisitElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) {
			DefaultVisit(elseIfConditionClauseSyntax);
		}

		public virtual void VisitIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) {
			DefaultVisit(identifierOrStringListSyntax);
		}

		public virtual void VisitCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) {
			DefaultVisit(codeNamespaceDeclarationSyntax);
		}

		public virtual void VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) {
			DefaultVisit(exitTransitionDefinitionSyntax);
		}

		public virtual void VisitCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) {
			DefaultVisit(codeGenerateToDeclarationSyntax);
		}

		public virtual void VisitTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) {
			DefaultVisit(transitionDefinitionBlockSyntax);
		}

		public virtual void VisitCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) {
			DefaultVisit(codeDoNotInjectDeclarationSyntax);
		}

		public virtual void VisitCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) {
			DefaultVisit(codeAbstractMethodDeclarationSyntax);
		}

		public virtual void VisitCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) {
			DefaultVisit(codeNotImplementedDeclarationSyntax);
		}

	}

	public interface ISyntaxNodeVisitor<T> {
		T VisitDoClause(DoClauseSyntax doClauseSyntax); 
		T VisitGoToEdge(GoToEdgeSyntax goToEdgeSyntax); 
		T VisitArrayType(ArrayTypeSyntax arrayTypeSyntax); 
		T VisitModalEdge(ModalEdgeSyntax modalEdgeSyntax); 
		T VisitParameter(ParameterSyntax parameterSyntax); 
		T VisitIdentifier(IdentifierSyntax identifierSyntax); 
		T VisitSimpleType(SimpleTypeSyntax simpleTypeSyntax); 
		T VisitGenericType(GenericTypeSyntax genericTypeSyntax); 
		T VisitNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax); 
		T VisitEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax); 
		T VisitParameterList(ParameterListSyntax parameterListSyntax); 
		T VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax); 
		T VisitStringLiteral(StringLiteralSyntax stringLiteralSyntax); 
		T VisitInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax); 
		T VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax); 
		T VisitCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax); 
		T VisitTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax); 
		T VisitIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax); 
		T VisitIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax); 
		T VisitArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax); 
		T VisitCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax); 
		T VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax); 
		T VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax); 
		T VisitCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax); 
		T VisitElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax); 
		T VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax); 
		T VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax); 
		T VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax); 
		T VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax); 
		T VisitCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax); 
		T VisitIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax); 
		T VisitIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax); 
		T VisitNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax); 
		T VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax); 
		T VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax); 
		T VisitCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax); 
		T VisitCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax); 
		T VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax); 
		T VisitElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax); 
		T VisitIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax); 
		T VisitCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax); 
		T VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax); 
		T VisitCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax); 
		T VisitTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax); 
		T VisitCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax); 
		T VisitCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax); 
		T VisitCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax); 
	}

	public abstract class SyntaxNodeVisitor<T>: ISyntaxNodeVisitor<T> {

		public T Visit(SyntaxNode node){
			return node.Accept(this);
		}

		protected virtual T DefaultVisit(SyntaxNode node) {
			return default(T);
		}

		public virtual T VisitDoClause(DoClauseSyntax doClauseSyntax) {
			return DefaultVisit(doClauseSyntax);
		}

		public virtual T VisitGoToEdge(GoToEdgeSyntax goToEdgeSyntax) {
			return DefaultVisit(goToEdgeSyntax);
		}

		public virtual T VisitArrayType(ArrayTypeSyntax arrayTypeSyntax) {
			return DefaultVisit(arrayTypeSyntax);
		}

		public virtual T VisitModalEdge(ModalEdgeSyntax modalEdgeSyntax) {
			return DefaultVisit(modalEdgeSyntax);
		}

		public virtual T VisitParameter(ParameterSyntax parameterSyntax) {
			return DefaultVisit(parameterSyntax);
		}

		public virtual T VisitIdentifier(IdentifierSyntax identifierSyntax) {
			return DefaultVisit(identifierSyntax);
		}

		public virtual T VisitSimpleType(SimpleTypeSyntax simpleTypeSyntax) {
			return DefaultVisit(simpleTypeSyntax);
		}

		public virtual T VisitGenericType(GenericTypeSyntax genericTypeSyntax) {
			return DefaultVisit(genericTypeSyntax);
		}

		public virtual T VisitNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) {
			return DefaultVisit(nonModalEdgeSyntax);
		}

		public virtual T VisitEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) {
			return DefaultVisit(endTargetNodeSyntax);
		}

		public virtual T VisitParameterList(ParameterListSyntax parameterListSyntax) {
			return DefaultVisit(parameterListSyntax);
		}

		public virtual T VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) {
			return DefaultVisit(signalTriggerSyntax);
		}

		public virtual T VisitStringLiteral(StringLiteralSyntax stringLiteralSyntax) {
			return DefaultVisit(stringLiteralSyntax);
		}

		public virtual T VisitInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) {
			return DefaultVisit(initSourceNodeSyntax);
		}

		public virtual T VisitTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) {
			return DefaultVisit(taskDefinitionSyntax);
		}

		public virtual T VisitCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) {
			return DefaultVisit(codeDeclarationSyntax);
		}

		public virtual T VisitTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) {
			return DefaultVisit(taskDeclarationSyntax);
		}

		public virtual T VisitIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) {
			return DefaultVisit(includeDirectiveSyntax);
		}

		public virtual T VisitIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) {
			return DefaultVisit(ifConditionClauseSyntax);
		}

		public virtual T VisitArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) {
			return DefaultVisit(arrayRankSpecifierSyntax);
		}

		public virtual T VisitCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) {
			return DefaultVisit(codeGenerationUnitSyntax);
		}

		public virtual T VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) {
			return DefaultVisit(endNodeDeclarationSyntax);
		}

		public virtual T VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) {
			return DefaultVisit(spontaneousTriggerSyntax);
		}

		public virtual T VisitCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) {
			return DefaultVisit(codeBaseDeclarationSyntax);
		}

		public virtual T VisitElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) {
			return DefaultVisit(elseConditionClauseSyntax);
		}

		public virtual T VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) {
			return DefaultVisit(exitNodeDeclarationSyntax);
		}

		public virtual T VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) {
			return DefaultVisit(initNodeDeclarationSyntax);
		}

		public virtual T VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) {
			return DefaultVisit(taskNodeDeclarationSyntax);
		}

		public virtual T VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) {
			return DefaultVisit(viewNodeDeclarationSyntax);
		}

		public virtual T VisitCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) {
			return DefaultVisit(codeUsingDeclarationSyntax);
		}

		public virtual T VisitIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) {
			return DefaultVisit(identifierSourceNodeSyntax);
		}

		public virtual T VisitIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) {
			return DefaultVisit(identifierTargetNodeSyntax);
		}

		public virtual T VisitNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) {
			return DefaultVisit(nodeDeclarationBlockSyntax);
		}

		public virtual T VisitTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) {
			return DefaultVisit(transitionDefinitionSyntax);
		}

		public virtual T VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) {
			return DefaultVisit(choiceNodeDeclarationSyntax);
		}

		public virtual T VisitCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) {
			return DefaultVisit(codeParamsDeclarationSyntax);
		}

		public virtual T VisitCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) {
			return DefaultVisit(codeResultDeclarationSyntax);
		}

		public virtual T VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) {
			return DefaultVisit(dialogNodeDeclarationSyntax);
		}

		public virtual T VisitElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) {
			return DefaultVisit(elseIfConditionClauseSyntax);
		}

		public virtual T VisitIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) {
			return DefaultVisit(identifierOrStringListSyntax);
		}

		public virtual T VisitCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) {
			return DefaultVisit(codeNamespaceDeclarationSyntax);
		}

		public virtual T VisitExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) {
			return DefaultVisit(exitTransitionDefinitionSyntax);
		}

		public virtual T VisitCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) {
			return DefaultVisit(codeGenerateToDeclarationSyntax);
		}

		public virtual T VisitTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) {
			return DefaultVisit(transitionDefinitionBlockSyntax);
		}

		public virtual T VisitCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) {
			return DefaultVisit(codeDoNotInjectDeclarationSyntax);
		}

		public virtual T VisitCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) {
			return DefaultVisit(codeAbstractMethodDeclarationSyntax);
		}

		public virtual T VisitCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) {
			return DefaultVisit(codeNotImplementedDeclarationSyntax);
		}

	}
}
