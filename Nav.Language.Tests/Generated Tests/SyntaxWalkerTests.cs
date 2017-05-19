 
//==================================================
// HINWEIS: Diese Datei wurde am 14.05.2017 14:17:39
//			automatisch generiert!
//==================================================
using NUnit.Framework;
using Pharmatechnik.Nav.Language;
using System.Collections.Generic;

namespace Nav.Language.Tests {

	public sealed class TestSyntaxNodeWalker: SyntaxNodeWalker {

		public readonly Dictionary<string, bool> MethodsCalled = new Dictionary<string, bool>();

		// DoClauseSyntax
		public override bool WalkDoClause(DoClauseSyntax doClauseSyntax) { 
			MethodsCalled["WalkDoClause"]=true;
			return true; 
		}
		public override void PostWalkDoClause(DoClauseSyntax doClauseSyntax) { 
			MethodsCalled["PostWalkDoClause"]=true;
		}
		// GoToEdgeSyntax
		public override bool WalkGoToEdge(GoToEdgeSyntax goToEdgeSyntax) { 
			MethodsCalled["WalkGoToEdge"]=true;
			return true; 
		}
		public override void PostWalkGoToEdge(GoToEdgeSyntax goToEdgeSyntax) { 
			MethodsCalled["PostWalkGoToEdge"]=true;
		}
		// ArrayTypeSyntax
		public override bool WalkArrayType(ArrayTypeSyntax arrayTypeSyntax) { 
			MethodsCalled["WalkArrayType"]=true;
			return true; 
		}
		public override void PostWalkArrayType(ArrayTypeSyntax arrayTypeSyntax) { 
			MethodsCalled["PostWalkArrayType"]=true;
		}
		// ModalEdgeSyntax
		public override bool WalkModalEdge(ModalEdgeSyntax modalEdgeSyntax) { 
			MethodsCalled["WalkModalEdge"]=true;
			return true; 
		}
		public override void PostWalkModalEdge(ModalEdgeSyntax modalEdgeSyntax) { 
			MethodsCalled["PostWalkModalEdge"]=true;
		}
		// ParameterSyntax
		public override bool WalkParameter(ParameterSyntax parameterSyntax) { 
			MethodsCalled["WalkParameter"]=true;
			return true; 
		}
		public override void PostWalkParameter(ParameterSyntax parameterSyntax) { 
			MethodsCalled["PostWalkParameter"]=true;
		}
		// IdentifierSyntax
		public override bool WalkIdentifier(IdentifierSyntax identifierSyntax) { 
			MethodsCalled["WalkIdentifier"]=true;
			return true; 
		}
		public override void PostWalkIdentifier(IdentifierSyntax identifierSyntax) { 
			MethodsCalled["PostWalkIdentifier"]=true;
		}
		// SimpleTypeSyntax
		public override bool WalkSimpleType(SimpleTypeSyntax simpleTypeSyntax) { 
			MethodsCalled["WalkSimpleType"]=true;
			return true; 
		}
		public override void PostWalkSimpleType(SimpleTypeSyntax simpleTypeSyntax) { 
			MethodsCalled["PostWalkSimpleType"]=true;
		}
		// GenericTypeSyntax
		public override bool WalkGenericType(GenericTypeSyntax genericTypeSyntax) { 
			MethodsCalled["WalkGenericType"]=true;
			return true; 
		}
		public override void PostWalkGenericType(GenericTypeSyntax genericTypeSyntax) { 
			MethodsCalled["PostWalkGenericType"]=true;
		}
		// NonModalEdgeSyntax
		public override bool WalkNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) { 
			MethodsCalled["WalkNonModalEdge"]=true;
			return true; 
		}
		public override void PostWalkNonModalEdge(NonModalEdgeSyntax nonModalEdgeSyntax) { 
			MethodsCalled["PostWalkNonModalEdge"]=true;
		}
		// EndTargetNodeSyntax
		public override bool WalkEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) { 
			MethodsCalled["WalkEndTargetNode"]=true;
			return true; 
		}
		public override void PostWalkEndTargetNode(EndTargetNodeSyntax endTargetNodeSyntax) { 
			MethodsCalled["PostWalkEndTargetNode"]=true;
		}
		// ParameterListSyntax
		public override bool WalkParameterList(ParameterListSyntax parameterListSyntax) { 
			MethodsCalled["WalkParameterList"]=true;
			return true; 
		}
		public override void PostWalkParameterList(ParameterListSyntax parameterListSyntax) { 
			MethodsCalled["PostWalkParameterList"]=true;
		}
		// SignalTriggerSyntax
		public override bool WalkSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) { 
			MethodsCalled["WalkSignalTrigger"]=true;
			return true; 
		}
		public override void PostWalkSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) { 
			MethodsCalled["PostWalkSignalTrigger"]=true;
		}
		// StringLiteralSyntax
		public override bool WalkStringLiteral(StringLiteralSyntax stringLiteralSyntax) { 
			MethodsCalled["WalkStringLiteral"]=true;
			return true; 
		}
		public override void PostWalkStringLiteral(StringLiteralSyntax stringLiteralSyntax) { 
			MethodsCalled["PostWalkStringLiteral"]=true;
		}
		// InitSourceNodeSyntax
		public override bool WalkInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) { 
			MethodsCalled["WalkInitSourceNode"]=true;
			return true; 
		}
		public override void PostWalkInitSourceNode(InitSourceNodeSyntax initSourceNodeSyntax) { 
			MethodsCalled["PostWalkInitSourceNode"]=true;
		}
		// TaskDefinitionSyntax
		public override bool WalkTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) { 
			MethodsCalled["WalkTaskDefinition"]=true;
			return true; 
		}
		public override void PostWalkTaskDefinition(TaskDefinitionSyntax taskDefinitionSyntax) { 
			MethodsCalled["PostWalkTaskDefinition"]=true;
		}
		// CodeDeclarationSyntax
		public override bool WalkCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) { 
			MethodsCalled["WalkCodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeDeclaration(CodeDeclarationSyntax codeDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeDeclaration"]=true;
		}
		// TaskDeclarationSyntax
		public override bool WalkTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) { 
			MethodsCalled["WalkTaskDeclaration"]=true;
			return true; 
		}
		public override void PostWalkTaskDeclaration(TaskDeclarationSyntax taskDeclarationSyntax) { 
			MethodsCalled["PostWalkTaskDeclaration"]=true;
		}
		// IncludeDirectiveSyntax
		public override bool WalkIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) { 
			MethodsCalled["WalkIncludeDirective"]=true;
			return true; 
		}
		public override void PostWalkIncludeDirective(IncludeDirectiveSyntax includeDirectiveSyntax) { 
			MethodsCalled["PostWalkIncludeDirective"]=true;
		}
		// IfConditionClauseSyntax
		public override bool WalkIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) { 
			MethodsCalled["WalkIfConditionClause"]=true;
			return true; 
		}
		public override void PostWalkIfConditionClause(IfConditionClauseSyntax ifConditionClauseSyntax) { 
			MethodsCalled["PostWalkIfConditionClause"]=true;
		}
		// ArrayRankSpecifierSyntax
		public override bool WalkArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) { 
			MethodsCalled["WalkArrayRankSpecifier"]=true;
			return true; 
		}
		public override void PostWalkArrayRankSpecifier(ArrayRankSpecifierSyntax arrayRankSpecifierSyntax) { 
			MethodsCalled["PostWalkArrayRankSpecifier"]=true;
		}
		// CodeGenerationUnitSyntax
		public override bool WalkCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) { 
			MethodsCalled["WalkCodeGenerationUnit"]=true;
			return true; 
		}
		public override void PostWalkCodeGenerationUnit(CodeGenerationUnitSyntax codeGenerationUnitSyntax) { 
			MethodsCalled["PostWalkCodeGenerationUnit"]=true;
		}
		// EndNodeDeclarationSyntax
		public override bool WalkEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) { 
			MethodsCalled["WalkEndNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkEndNodeDeclaration"]=true;
		}
		// SpontaneousTriggerSyntax
		public override bool WalkSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) { 
			MethodsCalled["WalkSpontaneousTrigger"]=true;
			return true; 
		}
		public override void PostWalkSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) { 
			MethodsCalled["PostWalkSpontaneousTrigger"]=true;
		}
		// CodeBaseDeclarationSyntax
		public override bool WalkCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) { 
			MethodsCalled["WalkCodeBaseDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeBaseDeclaration(CodeBaseDeclarationSyntax codeBaseDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeBaseDeclaration"]=true;
		}
		// ElseConditionClauseSyntax
		public override bool WalkElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) { 
			MethodsCalled["WalkElseConditionClause"]=true;
			return true; 
		}
		public override void PostWalkElseConditionClause(ElseConditionClauseSyntax elseConditionClauseSyntax) { 
			MethodsCalled["PostWalkElseConditionClause"]=true;
		}
		// ExitNodeDeclarationSyntax
		public override bool WalkExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) { 
			MethodsCalled["WalkExitNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkExitNodeDeclaration"]=true;
		}
		// InitNodeDeclarationSyntax
		public override bool WalkInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) { 
			MethodsCalled["WalkInitNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkInitNodeDeclaration"]=true;
		}
		// TaskNodeDeclarationSyntax
		public override bool WalkTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) { 
			MethodsCalled["WalkTaskNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkTaskNodeDeclaration"]=true;
		}
		// ViewNodeDeclarationSyntax
		public override bool WalkViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) { 
			MethodsCalled["WalkViewNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkViewNodeDeclaration"]=true;
		}
		// CodeUsingDeclarationSyntax
		public override bool WalkCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) { 
			MethodsCalled["WalkCodeUsingDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeUsingDeclaration(CodeUsingDeclarationSyntax codeUsingDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeUsingDeclaration"]=true;
		}
		// IdentifierSourceNodeSyntax
		public override bool WalkIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) { 
			MethodsCalled["WalkIdentifierSourceNode"]=true;
			return true; 
		}
		public override void PostWalkIdentifierSourceNode(IdentifierSourceNodeSyntax identifierSourceNodeSyntax) { 
			MethodsCalled["PostWalkIdentifierSourceNode"]=true;
		}
		// IdentifierTargetNodeSyntax
		public override bool WalkIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) { 
			MethodsCalled["WalkIdentifierTargetNode"]=true;
			return true; 
		}
		public override void PostWalkIdentifierTargetNode(IdentifierTargetNodeSyntax identifierTargetNodeSyntax) { 
			MethodsCalled["PostWalkIdentifierTargetNode"]=true;
		}
		// NodeDeclarationBlockSyntax
		public override bool WalkNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) { 
			MethodsCalled["WalkNodeDeclarationBlock"]=true;
			return true; 
		}
		public override void PostWalkNodeDeclarationBlock(NodeDeclarationBlockSyntax nodeDeclarationBlockSyntax) { 
			MethodsCalled["PostWalkNodeDeclarationBlock"]=true;
		}
		// TransitionDefinitionSyntax
		public override bool WalkTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) { 
			MethodsCalled["WalkTransitionDefinition"]=true;
			return true; 
		}
		public override void PostWalkTransitionDefinition(TransitionDefinitionSyntax transitionDefinitionSyntax) { 
			MethodsCalled["PostWalkTransitionDefinition"]=true;
		}
		// ChoiceNodeDeclarationSyntax
		public override bool WalkChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) { 
			MethodsCalled["WalkChoiceNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkChoiceNodeDeclaration"]=true;
		}
		// CodeParamsDeclarationSyntax
		public override bool WalkCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) { 
			MethodsCalled["WalkCodeParamsDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeParamsDeclaration(CodeParamsDeclarationSyntax codeParamsDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeParamsDeclaration"]=true;
		}
		// CodeResultDeclarationSyntax
		public override bool WalkCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) { 
			MethodsCalled["WalkCodeResultDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeResultDeclaration(CodeResultDeclarationSyntax codeResultDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeResultDeclaration"]=true;
		}
		// DialogNodeDeclarationSyntax
		public override bool WalkDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) { 
			MethodsCalled["WalkDialogNodeDeclaration"]=true;
			return true; 
		}
		public override void PostWalkDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) { 
			MethodsCalled["PostWalkDialogNodeDeclaration"]=true;
		}
		// ElseIfConditionClauseSyntax
		public override bool WalkElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) { 
			MethodsCalled["WalkElseIfConditionClause"]=true;
			return true; 
		}
		public override void PostWalkElseIfConditionClause(ElseIfConditionClauseSyntax elseIfConditionClauseSyntax) { 
			MethodsCalled["PostWalkElseIfConditionClause"]=true;
		}
		// IdentifierOrStringListSyntax
		public override bool WalkIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) { 
			MethodsCalled["WalkIdentifierOrStringList"]=true;
			return true; 
		}
		public override void PostWalkIdentifierOrStringList(IdentifierOrStringListSyntax identifierOrStringListSyntax) { 
			MethodsCalled["PostWalkIdentifierOrStringList"]=true;
		}
		// CodeNamespaceDeclarationSyntax
		public override bool WalkCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) { 
			MethodsCalled["WalkCodeNamespaceDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeNamespaceDeclaration(CodeNamespaceDeclarationSyntax codeNamespaceDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeNamespaceDeclaration"]=true;
		}
		// ExitTransitionDefinitionSyntax
		public override bool WalkExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) { 
			MethodsCalled["WalkExitTransitionDefinition"]=true;
			return true; 
		}
		public override void PostWalkExitTransitionDefinition(ExitTransitionDefinitionSyntax exitTransitionDefinitionSyntax) { 
			MethodsCalled["PostWalkExitTransitionDefinition"]=true;
		}
		// CodeGenerateToDeclarationSyntax
		public override bool WalkCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) { 
			MethodsCalled["WalkCodeGenerateToDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeGenerateToDeclaration(CodeGenerateToDeclarationSyntax codeGenerateToDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeGenerateToDeclaration"]=true;
		}
		// TransitionDefinitionBlockSyntax
		public override bool WalkTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) { 
			MethodsCalled["WalkTransitionDefinitionBlock"]=true;
			return true; 
		}
		public override void PostWalkTransitionDefinitionBlock(TransitionDefinitionBlockSyntax transitionDefinitionBlockSyntax) { 
			MethodsCalled["PostWalkTransitionDefinitionBlock"]=true;
		}
		// CodeDoNotInjectDeclarationSyntax
		public override bool WalkCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) { 
			MethodsCalled["WalkCodeDoNotInjectDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeDoNotInjectDeclaration(CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeDoNotInjectDeclaration"]=true;
		}
		// CodeAbstractMethodDeclarationSyntax
		public override bool WalkCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) { 
			MethodsCalled["WalkCodeAbstractMethodDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeAbstractMethodDeclaration(CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeAbstractMethodDeclaration"]=true;
		}
		// CodeNotImplementedDeclarationSyntax
		public override bool WalkCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) { 
			MethodsCalled["WalkCodeNotImplementedDeclaration"]=true;
			return true; 
		}
		public override void PostWalkCodeNotImplementedDeclaration(CodeNotImplementedDeclarationSyntax codeNotImplementedDeclarationSyntax) { 
			MethodsCalled["PostWalkCodeNotImplementedDeclaration"]=true;
		}
	}

	[TestFixture]
	public class SyntaxWalkerTests {
		
		public Dictionary<string, bool> MethodsCalled;

		[SetUp]
		public void Setup() {
			var syntaxTree = SyntaxTree.ParseText(Resources.AllRules);
			var walker     = new TestSyntaxNodeWalker();

			walker.Walk(syntaxTree.GetRoot());
			MethodsCalled=walker.MethodsCalled;
		}

		// DoClause
		[Test]
		public void TestWalkDoClause() {
			Assert.That(MethodsCalled.ContainsKey("WalkDoClause"), Is.True, "WalkDoClause not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkDoClause"), Is.True, "PostWalkDoClause not called.");
		}
		// GoToEdge
		[Test]
		public void TestWalkGoToEdge() {
			Assert.That(MethodsCalled.ContainsKey("WalkGoToEdge"), Is.True, "WalkGoToEdge not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkGoToEdge"), Is.True, "PostWalkGoToEdge not called.");
		}
		// ArrayType
		[Test]
		public void TestWalkArrayType() {
			Assert.That(MethodsCalled.ContainsKey("WalkArrayType"), Is.True, "WalkArrayType not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkArrayType"), Is.True, "PostWalkArrayType not called.");
		}
		// ModalEdge
		[Test]
		public void TestWalkModalEdge() {
			Assert.That(MethodsCalled.ContainsKey("WalkModalEdge"), Is.True, "WalkModalEdge not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkModalEdge"), Is.True, "PostWalkModalEdge not called.");
		}
		// Parameter
		[Test]
		public void TestWalkParameter() {
			Assert.That(MethodsCalled.ContainsKey("WalkParameter"), Is.True, "WalkParameter not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkParameter"), Is.True, "PostWalkParameter not called.");
		}
		// Identifier
		[Test]
		public void TestWalkIdentifier() {
			Assert.That(MethodsCalled.ContainsKey("WalkIdentifier"), Is.True, "WalkIdentifier not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIdentifier"), Is.True, "PostWalkIdentifier not called.");
		}
		// SimpleType
		[Test]
		public void TestWalkSimpleType() {
			Assert.That(MethodsCalled.ContainsKey("WalkSimpleType"), Is.True, "WalkSimpleType not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkSimpleType"), Is.True, "PostWalkSimpleType not called.");
		}
		// GenericType
		[Test]
		public void TestWalkGenericType() {
			Assert.That(MethodsCalled.ContainsKey("WalkGenericType"), Is.True, "WalkGenericType not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkGenericType"), Is.True, "PostWalkGenericType not called.");
		}
		// NonModalEdge
		[Test]
		public void TestWalkNonModalEdge() {
			Assert.That(MethodsCalled.ContainsKey("WalkNonModalEdge"), Is.True, "WalkNonModalEdge not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkNonModalEdge"), Is.True, "PostWalkNonModalEdge not called.");
		}
		// EndTargetNode
		[Test]
		public void TestWalkEndTargetNode() {
			Assert.That(MethodsCalled.ContainsKey("WalkEndTargetNode"), Is.True, "WalkEndTargetNode not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkEndTargetNode"), Is.True, "PostWalkEndTargetNode not called.");
		}
		// ParameterList
		[Test]
		public void TestWalkParameterList() {
			Assert.That(MethodsCalled.ContainsKey("WalkParameterList"), Is.True, "WalkParameterList not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkParameterList"), Is.True, "PostWalkParameterList not called.");
		}
		// SignalTrigger
		[Test]
		public void TestWalkSignalTrigger() {
			Assert.That(MethodsCalled.ContainsKey("WalkSignalTrigger"), Is.True, "WalkSignalTrigger not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkSignalTrigger"), Is.True, "PostWalkSignalTrigger not called.");
		}
		// StringLiteral
		[Test]
		public void TestWalkStringLiteral() {
			Assert.That(MethodsCalled.ContainsKey("WalkStringLiteral"), Is.True, "WalkStringLiteral not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkStringLiteral"), Is.True, "PostWalkStringLiteral not called.");
		}
		// InitSourceNode
		[Test]
		public void TestWalkInitSourceNode() {
			Assert.That(MethodsCalled.ContainsKey("WalkInitSourceNode"), Is.True, "WalkInitSourceNode not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkInitSourceNode"), Is.True, "PostWalkInitSourceNode not called.");
		}
		// TaskDefinition
		[Test]
		public void TestWalkTaskDefinition() {
			Assert.That(MethodsCalled.ContainsKey("WalkTaskDefinition"), Is.True, "WalkTaskDefinition not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkTaskDefinition"), Is.True, "PostWalkTaskDefinition not called.");
		}
		// CodeDeclaration
		[Test]
		public void TestWalkCodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeDeclaration"), Is.True, "WalkCodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeDeclaration"), Is.True, "PostWalkCodeDeclaration not called.");
		}
		// TaskDeclaration
		[Test]
		public void TestWalkTaskDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkTaskDeclaration"), Is.True, "WalkTaskDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkTaskDeclaration"), Is.True, "PostWalkTaskDeclaration not called.");
		}
		// IncludeDirective
		[Test]
		public void TestWalkIncludeDirective() {
			Assert.That(MethodsCalled.ContainsKey("WalkIncludeDirective"), Is.True, "WalkIncludeDirective not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIncludeDirective"), Is.True, "PostWalkIncludeDirective not called.");
		}
		// IfConditionClause
		[Test]
		public void TestWalkIfConditionClause() {
			Assert.That(MethodsCalled.ContainsKey("WalkIfConditionClause"), Is.True, "WalkIfConditionClause not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIfConditionClause"), Is.True, "PostWalkIfConditionClause not called.");
		}
		// ArrayRankSpecifier
		[Test]
		public void TestWalkArrayRankSpecifier() {
			Assert.That(MethodsCalled.ContainsKey("WalkArrayRankSpecifier"), Is.True, "WalkArrayRankSpecifier not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkArrayRankSpecifier"), Is.True, "PostWalkArrayRankSpecifier not called.");
		}
		// CodeGenerationUnit
		[Test]
		public void TestWalkCodeGenerationUnit() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeGenerationUnit"), Is.True, "WalkCodeGenerationUnit not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeGenerationUnit"), Is.True, "PostWalkCodeGenerationUnit not called.");
		}
		// EndNodeDeclaration
		[Test]
		public void TestWalkEndNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkEndNodeDeclaration"), Is.True, "WalkEndNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkEndNodeDeclaration"), Is.True, "PostWalkEndNodeDeclaration not called.");
		}
		// SpontaneousTrigger
		[Test]
		public void TestWalkSpontaneousTrigger() {
			Assert.That(MethodsCalled.ContainsKey("WalkSpontaneousTrigger"), Is.True, "WalkSpontaneousTrigger not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkSpontaneousTrigger"), Is.True, "PostWalkSpontaneousTrigger not called.");
		}
		// CodeBaseDeclaration
		[Test]
		public void TestWalkCodeBaseDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeBaseDeclaration"), Is.True, "WalkCodeBaseDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeBaseDeclaration"), Is.True, "PostWalkCodeBaseDeclaration not called.");
		}
		// ElseConditionClause
		[Test]
		public void TestWalkElseConditionClause() {
			Assert.That(MethodsCalled.ContainsKey("WalkElseConditionClause"), Is.True, "WalkElseConditionClause not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkElseConditionClause"), Is.True, "PostWalkElseConditionClause not called.");
		}
		// ExitNodeDeclaration
		[Test]
		public void TestWalkExitNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkExitNodeDeclaration"), Is.True, "WalkExitNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkExitNodeDeclaration"), Is.True, "PostWalkExitNodeDeclaration not called.");
		}
		// InitNodeDeclaration
		[Test]
		public void TestWalkInitNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkInitNodeDeclaration"), Is.True, "WalkInitNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkInitNodeDeclaration"), Is.True, "PostWalkInitNodeDeclaration not called.");
		}
		// TaskNodeDeclaration
		[Test]
		public void TestWalkTaskNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkTaskNodeDeclaration"), Is.True, "WalkTaskNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkTaskNodeDeclaration"), Is.True, "PostWalkTaskNodeDeclaration not called.");
		}
		// ViewNodeDeclaration
		[Test]
		public void TestWalkViewNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkViewNodeDeclaration"), Is.True, "WalkViewNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkViewNodeDeclaration"), Is.True, "PostWalkViewNodeDeclaration not called.");
		}
		// CodeUsingDeclaration
		[Test]
		public void TestWalkCodeUsingDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeUsingDeclaration"), Is.True, "WalkCodeUsingDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeUsingDeclaration"), Is.True, "PostWalkCodeUsingDeclaration not called.");
		}
		// IdentifierSourceNode
		[Test]
		public void TestWalkIdentifierSourceNode() {
			Assert.That(MethodsCalled.ContainsKey("WalkIdentifierSourceNode"), Is.True, "WalkIdentifierSourceNode not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIdentifierSourceNode"), Is.True, "PostWalkIdentifierSourceNode not called.");
		}
		// IdentifierTargetNode
		[Test]
		public void TestWalkIdentifierTargetNode() {
			Assert.That(MethodsCalled.ContainsKey("WalkIdentifierTargetNode"), Is.True, "WalkIdentifierTargetNode not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIdentifierTargetNode"), Is.True, "PostWalkIdentifierTargetNode not called.");
		}
		// NodeDeclarationBlock
		[Test]
		public void TestWalkNodeDeclarationBlock() {
			Assert.That(MethodsCalled.ContainsKey("WalkNodeDeclarationBlock"), Is.True, "WalkNodeDeclarationBlock not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkNodeDeclarationBlock"), Is.True, "PostWalkNodeDeclarationBlock not called.");
		}
		// TransitionDefinition
		[Test]
		public void TestWalkTransitionDefinition() {
			Assert.That(MethodsCalled.ContainsKey("WalkTransitionDefinition"), Is.True, "WalkTransitionDefinition not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkTransitionDefinition"), Is.True, "PostWalkTransitionDefinition not called.");
		}
		// ChoiceNodeDeclaration
		[Test]
		public void TestWalkChoiceNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkChoiceNodeDeclaration"), Is.True, "WalkChoiceNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkChoiceNodeDeclaration"), Is.True, "PostWalkChoiceNodeDeclaration not called.");
		}
		// CodeParamsDeclaration
		[Test]
		public void TestWalkCodeParamsDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeParamsDeclaration"), Is.True, "WalkCodeParamsDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeParamsDeclaration"), Is.True, "PostWalkCodeParamsDeclaration not called.");
		}
		// CodeResultDeclaration
		[Test]
		public void TestWalkCodeResultDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeResultDeclaration"), Is.True, "WalkCodeResultDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeResultDeclaration"), Is.True, "PostWalkCodeResultDeclaration not called.");
		}
		// DialogNodeDeclaration
		[Test]
		public void TestWalkDialogNodeDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkDialogNodeDeclaration"), Is.True, "WalkDialogNodeDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkDialogNodeDeclaration"), Is.True, "PostWalkDialogNodeDeclaration not called.");
		}
		// ElseIfConditionClause
		[Test]
		public void TestWalkElseIfConditionClause() {
			Assert.That(MethodsCalled.ContainsKey("WalkElseIfConditionClause"), Is.True, "WalkElseIfConditionClause not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkElseIfConditionClause"), Is.True, "PostWalkElseIfConditionClause not called.");
		}
		// IdentifierOrStringList
		[Test]
		public void TestWalkIdentifierOrStringList() {
			Assert.That(MethodsCalled.ContainsKey("WalkIdentifierOrStringList"), Is.True, "WalkIdentifierOrStringList not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkIdentifierOrStringList"), Is.True, "PostWalkIdentifierOrStringList not called.");
		}
		// CodeNamespaceDeclaration
		[Test]
		public void TestWalkCodeNamespaceDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeNamespaceDeclaration"), Is.True, "WalkCodeNamespaceDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeNamespaceDeclaration"), Is.True, "PostWalkCodeNamespaceDeclaration not called.");
		}
		// ExitTransitionDefinition
		[Test]
		public void TestWalkExitTransitionDefinition() {
			Assert.That(MethodsCalled.ContainsKey("WalkExitTransitionDefinition"), Is.True, "WalkExitTransitionDefinition not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkExitTransitionDefinition"), Is.True, "PostWalkExitTransitionDefinition not called.");
		}
		// CodeGenerateToDeclaration
		[Test]
		public void TestWalkCodeGenerateToDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeGenerateToDeclaration"), Is.True, "WalkCodeGenerateToDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeGenerateToDeclaration"), Is.True, "PostWalkCodeGenerateToDeclaration not called.");
		}
		// TransitionDefinitionBlock
		[Test]
		public void TestWalkTransitionDefinitionBlock() {
			Assert.That(MethodsCalled.ContainsKey("WalkTransitionDefinitionBlock"), Is.True, "WalkTransitionDefinitionBlock not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkTransitionDefinitionBlock"), Is.True, "PostWalkTransitionDefinitionBlock not called.");
		}
		// CodeDoNotInjectDeclaration
		[Test]
		public void TestWalkCodeDoNotInjectDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeDoNotInjectDeclaration"), Is.True, "WalkCodeDoNotInjectDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeDoNotInjectDeclaration"), Is.True, "PostWalkCodeDoNotInjectDeclaration not called.");
		}
		// CodeAbstractMethodDeclaration
		[Test]
		public void TestWalkCodeAbstractMethodDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeAbstractMethodDeclaration"), Is.True, "WalkCodeAbstractMethodDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeAbstractMethodDeclaration"), Is.True, "PostWalkCodeAbstractMethodDeclaration not called.");
		}
		// CodeNotImplementedDeclaration
		[Test]
		public void TestWalkCodeNotImplementedDeclaration() {
			Assert.That(MethodsCalled.ContainsKey("WalkCodeNotImplementedDeclaration"), Is.True, "WalkCodeNotImplementedDeclaration not called.");	
			Assert.That(MethodsCalled.ContainsKey("PostWalkCodeNotImplementedDeclaration"), Is.True, "PostWalkCodeNotImplementedDeclaration not called.");
		}
	}
}
