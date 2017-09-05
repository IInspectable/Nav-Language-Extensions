 
//==================================================
// HINWEIS: Diese Datei wurde am 14.05.2017 14:16:42
//			automatisch generiert!
//==================================================
namespace Pharmatechnik.Nav.Language {

	partial class SyntaxNode {
		public abstract void Walk(SyntaxNodeWalker walker);
	}

	#region SyntaxNode Implementation

	partial class ArrayRankSpecifierSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkArrayRankSpecifier(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkArrayRankSpecifier(this);
		}
	}

	partial class ArrayTypeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkArrayType(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkArrayType(this);
		}
	}

	partial class ChoiceNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkChoiceNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkChoiceNodeDeclaration(this);
		}
	}

	partial class CodeAbstractMethodDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeAbstractMethodDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeAbstractMethodDeclaration(this);
		}
	}

	partial class CodeBaseDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeBaseDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeBaseDeclaration(this);
		}
	}

	partial class CodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeDeclaration(this);
		}
	}

	partial class CodeDoNotInjectDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeDoNotInjectDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeDoNotInjectDeclaration(this);
		}
	}

	partial class CodeGenerateToDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeGenerateToDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeGenerateToDeclaration(this);
		}
	}

	partial class CodeGenerationUnitSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeGenerationUnit(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeGenerationUnit(this);
		}
	}

	partial class CodeNamespaceDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeNamespaceDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeNamespaceDeclaration(this);
		}
	}

	partial class CodeNotImplementedDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeNotImplementedDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeNotImplementedDeclaration(this);
		}
	}

	partial class CodeParamsDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeParamsDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeParamsDeclaration(this);
		}
	}

	partial class CodeResultDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeResultDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeResultDeclaration(this);
		}
	}

	partial class CodeUsingDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkCodeUsingDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkCodeUsingDeclaration(this);
		}
	}

	partial class DialogNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkDialogNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkDialogNodeDeclaration(this);
		}
	}

	partial class DoClauseSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkDoClause(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkDoClause(this);
		}
	}

	partial class ElseConditionClauseSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkElseConditionClause(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkElseConditionClause(this);
		}
	}

	partial class ElseIfConditionClauseSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkElseIfConditionClause(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkElseIfConditionClause(this);
		}
	}

	partial class EndNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkEndNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkEndNodeDeclaration(this);
		}
	}

	partial class EndTargetNodeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkEndTargetNode(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkEndTargetNode(this);
		}
	}

	partial class ExitNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkExitNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkExitNodeDeclaration(this);
		}
	}

	partial class ExitTransitionDefinitionSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkExitTransitionDefinition(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkExitTransitionDefinition(this);
		}
	}

	partial class GenericTypeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkGenericType(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkGenericType(this);
		}
	}

	partial class GoToEdgeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkGoToEdge(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkGoToEdge(this);
		}
	}

	partial class IdentifierOrStringListSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIdentifierOrStringList(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIdentifierOrStringList(this);
		}
	}

	partial class IdentifierSourceNodeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIdentifierSourceNode(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIdentifierSourceNode(this);
		}
	}

	partial class IdentifierSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIdentifier(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIdentifier(this);
		}
	}

	partial class IdentifierTargetNodeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIdentifierTargetNode(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIdentifierTargetNode(this);
		}
	}

	partial class IfConditionClauseSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIfConditionClause(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIfConditionClause(this);
		}
	}

	partial class IncludeDirectiveSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkIncludeDirective(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkIncludeDirective(this);
		}
	}

	partial class InitNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkInitNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkInitNodeDeclaration(this);
		}
	}

	partial class InitSourceNodeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkInitSourceNode(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkInitSourceNode(this);
		}
	}

	partial class ModalEdgeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkModalEdge(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkModalEdge(this);
		}
	}

	partial class NodeDeclarationBlockSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkNodeDeclarationBlock(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkNodeDeclarationBlock(this);
		}
	}

	partial class NonModalEdgeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkNonModalEdge(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkNonModalEdge(this);
		}
	}

	partial class ParameterListSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkParameterList(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkParameterList(this);
		}
	}

	partial class ParameterSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkParameter(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkParameter(this);
		}
	}

	partial class SignalTriggerSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkSignalTrigger(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkSignalTrigger(this);
		}
	}

	partial class SimpleTypeSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkSimpleType(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkSimpleType(this);
		}
	}

	partial class SpontaneousTriggerSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkSpontaneousTrigger(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkSpontaneousTrigger(this);
		}
	}

	partial class StringLiteralSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkStringLiteral(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkStringLiteral(this);
		}
	}

	partial class TaskDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkTaskDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkTaskDeclaration(this);
		}
	}

	partial class TaskDefinitionSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkTaskDefinition(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkTaskDefinition(this);
		}
	}

	partial class TaskNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkTaskNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkTaskNodeDeclaration(this);
		}
	}

	partial class TransitionDefinitionBlockSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkTransitionDefinitionBlock(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkTransitionDefinitionBlock(this);
		}
	}

	partial class TransitionDefinitionSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkTransitionDefinition(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkTransitionDefinition(this);
		}
	}

	partial class ViewNodeDeclarationSyntax {
		public override void Walk(SyntaxNodeWalker walker) {
			if(!walker.WalkViewNodeDeclaration(this)) {
				return;
			}
			foreach(var child in ChildNodes()) {
				child.Walk(walker);
			}
			walker.PostWalkViewNodeDeclaration(this);
		}
	}

	#endregion

	public abstract class SyntaxNodeWalker {

		public void Walk(SyntaxNode node) {
			node.Walk(this);
		}

		public virtual bool DefaultWalk(SyntaxNode node) {
			return true;
		}

		// DoClauseSyntax
		public virtual bool WalkDoClause(DoClauseSyntax doClauseSyntax) { return DefaultWalk(doClauseSyntax); }
		public virtual void PostWalkDoClause(DoClauseSyntax doClauseSyntax) { }

		// GoToEdgeSyntax
		public virtual bool WalkGoToEdge(GoToEdgeSyntax goToEdgeSyntax) { return DefaultWalk(goToEdgeSyntax); }
		public virtual void PostWalkGoToEdge(GoToEdgeSyntax goToEdgeSyntax) { }

		// ArrayTypeSyntax
		public virtual bool WalkArrayType(ArrayTypeSyntax arrayTypeSyntax) { return DefaultWalk(arrayTypeSyntax); }
		public virtual void PostWalkArrayType(ArrayTypeSyntax arrayTypeSyntax) { }

		// ModalEdgeSyntax
		public virtual bool WalkModalEdge(ModalEdgeSyntax modalEdgeSyntax) { return DefaultWalk(modalEdgeSyntax); }
		public virtual void PostWalkModalEdge(ModalEdgeSyntax modalEdgeSyntax) { }

		// ParameterSyntax
		public virtual bool WalkParameter(ParameterSyntax parameterSyntax) { return DefaultWalk(parameterSyntax); }
		public virtual void PostWalkParameter(ParameterSyntax parameterSyntax) { }

		// IdentifierSyntax
		public virtual bool WalkIdentifier(IdentifierSyntax identifierSyntax) { return DefaultWalk(identifierSyntax); }
		public virtual void PostWalkIdentifier(IdentifierSyntax identifierSyntax) { }

		// SimpleTypeSyntax
		public virtual bool WalkSimpleType(SimpleTypeSyntax simpleTypeSyntax) { return DefaultWalk(simpleTypeSyntax); }
		public virtual void PostWalkSimpleType(SimpleTypeSyntax simpleTypeSyntax) { }

		// GenericTypeSyntax
		public virtual bool WalkGenericType(GenericTypeSyntax genericTypeSyntax) { return DefaultWalk(genericTypeSyntax); }
		public virtual void PostWalkGenericType(GenericTypeSyntax genericTypeSyntax) { }

		// NonModalEdgeSyntax
		public virtual bool WalkNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) { return DefaultWalk(nonModalEdgeSyntax); }
		public virtual void PostWalkNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) { }

		// EndTargetNodeSyntax
		public virtual bool WalkEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) { return DefaultWalk(endTargetNodeSyntax); }
		public virtual void PostWalkEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) { }

		// ParameterListSyntax
		public virtual bool WalkParameterList(ParameterListSyntax parameterListSyntax) { return DefaultWalk(parameterListSyntax); }
		public virtual void PostWalkParameterList(ParameterListSyntax parameterListSyntax) { }

		// SignalTriggerSyntax
		public virtual bool WalkSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) { return DefaultWalk(signalTriggerSyntax); }
		public virtual void PostWalkSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) { }

		// StringLiteralSyntax
		public virtual bool WalkStringLiteral(StringLiteralSyntax stringLiteralSyntax) { return DefaultWalk(stringLiteralSyntax); }
		public virtual void PostWalkStringLiteral(StringLiteralSyntax stringLiteralSyntax) { }

		// InitSourceNodeSyntax
		public virtual bool WalkInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) { return DefaultWalk(initSourceNodeSyntax); }
		public virtual void PostWalkInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) { }

		// TaskDefinitionSyntax
		public virtual bool WalkTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) { return DefaultWalk(taskDefinitionSyntax); }
		public virtual void PostWalkTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) { }

		// CodeDeclarationSyntax
		public virtual bool WalkCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) { return DefaultWalk(codeDeclarationSyntax); }
		public virtual void PostWalkCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) { }

		// TaskDeclarationSyntax
		public virtual bool WalkTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) { return DefaultWalk(taskDeclarationSyntax); }
		public virtual void PostWalkTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) { }

		// IncludeDirectiveSyntax
		public virtual bool WalkIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) { return DefaultWalk(includeDirectiveSyntax); }
		public virtual void PostWalkIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) { }

		// IfConditionClauseSyntax
		public virtual bool WalkIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) { return DefaultWalk(ifConditionClauseSyntax); }
		public virtual void PostWalkIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) { }

		// ArrayRankSpecifierSyntax
		public virtual bool WalkArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) { return DefaultWalk(arrayRankSpecifierSyntax); }
		public virtual void PostWalkArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) { }

		// CodeGenerationUnitSyntax
		public virtual bool WalkCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) { return DefaultWalk(codeGenerationUnitSyntax); }
		public virtual void PostWalkCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) { }

		// EndNodeDeclarationSyntax
		public virtual bool WalkEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) { return DefaultWalk(endNodeDeclarationSyntax); }
		public virtual void PostWalkEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) { }

		// SpontaneousTriggerSyntax
		public virtual bool WalkSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) { return DefaultWalk(spontaneousTriggerSyntax); }
		public virtual void PostWalkSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) { }

		// CodeBaseDeclarationSyntax
		public virtual bool WalkCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) { return DefaultWalk(codeBaseDeclarationSyntax); }
		public virtual void PostWalkCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) { }

		// ElseConditionClauseSyntax
		public virtual bool WalkElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) { return DefaultWalk(elseConditionClauseSyntax); }
		public virtual void PostWalkElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) { }

		// ExitNodeDeclarationSyntax
		public virtual bool WalkExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) { return DefaultWalk(exitNodeDeclarationSyntax); }
		public virtual void PostWalkExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) { }

		// InitNodeDeclarationSyntax
		public virtual bool WalkInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) { return DefaultWalk(initNodeDeclarationSyntax); }
		public virtual void PostWalkInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) { }

		// TaskNodeDeclarationSyntax
		public virtual bool WalkTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) { return DefaultWalk(taskNodeDeclarationSyntax); }
		public virtual void PostWalkTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) { }

		// ViewNodeDeclarationSyntax
		public virtual bool WalkViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) { return DefaultWalk(viewNodeDeclarationSyntax); }
		public virtual void PostWalkViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) { }

		// CodeUsingDeclarationSyntax
		public virtual bool WalkCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) { return DefaultWalk(codeUsingDeclarationSyntax); }
		public virtual void PostWalkCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) { }

		// IdentifierSourceNodeSyntax
		public virtual bool WalkIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) { return DefaultWalk(identifierSourceNodeSyntax); }
		public virtual void PostWalkIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) { }

		// IdentifierTargetNodeSyntax
		public virtual bool WalkIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) { return DefaultWalk(identifierTargetNodeSyntax); }
		public virtual void PostWalkIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) { }

		// NodeDeclarationBlockSyntax
		public virtual bool WalkNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) { return DefaultWalk(nodeDeclarationBlockSyntax); }
		public virtual void PostWalkNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) { }

		// TransitionDefinitionSyntax
		public virtual bool WalkTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) { return DefaultWalk(transitionDefinitionSyntax); }
		public virtual void PostWalkTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) { }

		// ChoiceNodeDeclarationSyntax
		public virtual bool WalkChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) { return DefaultWalk(choiceNodeDeclarationSyntax); }
		public virtual void PostWalkChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) { }

		// CodeParamsDeclarationSyntax
		public virtual bool WalkCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) { return DefaultWalk(codeParamsDeclarationSyntax); }
		public virtual void PostWalkCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) { }

		// CodeResultDeclarationSyntax
		public virtual bool WalkCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) { return DefaultWalk(codeResultDeclarationSyntax); }
		public virtual void PostWalkCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) { }

		// DialogNodeDeclarationSyntax
		public virtual bool WalkDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) { return DefaultWalk(dialogNodeDeclarationSyntax); }
		public virtual void PostWalkDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) { }

		// ElseIfConditionClauseSyntax
		public virtual bool WalkElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) { return DefaultWalk(elseIfConditionClauseSyntax); }
		public virtual void PostWalkElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) { }

		// IdentifierOrStringListSyntax
		public virtual bool WalkIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) { return DefaultWalk(identifierOrStringListSyntax); }
		public virtual void PostWalkIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) { }

		// CodeNamespaceDeclarationSyntax
		public virtual bool WalkCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) { return DefaultWalk(codeNamespaceDeclarationSyntax); }
		public virtual void PostWalkCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) { }

		// ExitTransitionDefinitionSyntax
		public virtual bool WalkExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) { return DefaultWalk(exitTransitionDefinitionSyntax); }
		public virtual void PostWalkExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) { }

		// CodeGenerateToDeclarationSyntax
		public virtual bool WalkCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) { return DefaultWalk(codeGenerateToDeclarationSyntax); }
		public virtual void PostWalkCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) { }

		// TransitionDefinitionBlockSyntax
		public virtual bool WalkTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) { return DefaultWalk(transitionDefinitionBlockSyntax); }
		public virtual void PostWalkTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) { }

		// CodeDoNotInjectDeclarationSyntax
		public virtual bool WalkCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) { return DefaultWalk(codeDoNotInjectDeclarationSyntax); }
		public virtual void PostWalkCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) { }

		// CodeAbstractMethodDeclarationSyntax
		public virtual bool WalkCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) { return DefaultWalk(codeAbstractMethodDeclarationSyntax); }
		public virtual void PostWalkCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) { }

		// CodeNotImplementedDeclarationSyntax
		public virtual bool WalkCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) { return DefaultWalk(codeNotImplementedDeclarationSyntax); }
		public virtual void PostWalkCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) { }

	}
}
