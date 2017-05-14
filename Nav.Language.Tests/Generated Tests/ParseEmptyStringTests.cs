 
//==================================================
// HINWEIS: Diese Datei wurde am 14.05.2017 14:17:39
//			automatisch generiert!
//==================================================
using NUnit.Framework;
using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

	[TestFixture]
	public class ParseEmptyStringTests {
		
        // DoClauseSyntax
		[Test]
		public void TestDoClauseSyntax() {
			
        var doClauseSyntax=Syntax.ParseDoClause("");
            // DoKeyword
            Assert.That(doClauseSyntax.DoKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", doClauseSyntax.DoKeyword);
            Assert.That(doClauseSyntax.DoKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", doClauseSyntax.DoKeyword);
		}

        // GoToEdgeSyntax
		[Test]
		public void TestGoToEdgeSyntax() {
			
        var goToEdgeSyntax=Syntax.ParseGoToEdge("");
            // Keyword
            Assert.That(goToEdgeSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", goToEdgeSyntax.Keyword);
            Assert.That(goToEdgeSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", goToEdgeSyntax.Keyword);
            // GoToEdgeKeyword
            Assert.That(goToEdgeSyntax.GoToEdgeKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", goToEdgeSyntax.GoToEdgeKeyword);
            Assert.That(goToEdgeSyntax.GoToEdgeKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", goToEdgeSyntax.GoToEdgeKeyword);
		}

        // ArrayTypeSyntax
		[Test]
		public void TestArrayTypeSyntax() {
			
		}

        // ModalEdgeSyntax
		[Test]
		public void TestModalEdgeSyntax() {
			
        var modalEdgeSyntax=Syntax.ParseModalEdge("");
            // Keyword
            Assert.That(modalEdgeSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", modalEdgeSyntax.Keyword);
            Assert.That(modalEdgeSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", modalEdgeSyntax.Keyword);
            // ModalEdgeKeyword
            Assert.That(modalEdgeSyntax.ModalEdgeKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", modalEdgeSyntax.ModalEdgeKeyword);
            Assert.That(modalEdgeSyntax.ModalEdgeKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", modalEdgeSyntax.ModalEdgeKeyword);
		}

        // ParameterSyntax
		[Test]
		public void TestParameterSyntax() {
			
        var parameterSyntax=Syntax.ParseParameter("");
            // Identifier
            Assert.That(parameterSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", parameterSyntax.Identifier);
            Assert.That(parameterSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", parameterSyntax.Identifier);
		}

        // IdentifierSyntax
		[Test]
		public void TestIdentifierSyntax() {
			
        var identifierSyntax=Syntax.ParseIdentifier("");
            // Identifier
            Assert.That(identifierSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierSyntax.Identifier);
            Assert.That(identifierSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierSyntax.Identifier);
		}

        // SimpleTypeSyntax
		[Test]
		public void TestSimpleTypeSyntax() {
			
        var simpleTypeSyntax=Syntax.ParseSimpleType("");
            // Identifier
            Assert.That(simpleTypeSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", simpleTypeSyntax.Identifier);
            Assert.That(simpleTypeSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", simpleTypeSyntax.Identifier);
		}

        // GenericTypeSyntax
		[Test]
		public void TestGenericTypeSyntax() {
			
        var genericTypeSyntax=Syntax.ParseGenericType("");
            // Identifier
            Assert.That(genericTypeSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", genericTypeSyntax.Identifier);
            Assert.That(genericTypeSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", genericTypeSyntax.Identifier);
		}

        // NonModalEdgeSyntax
		[Test]
		public void TestNonModalEdgeSyntax() {
			
        var nonModalEdgeSyntax=Syntax.ParseNonModalEdge("");
            // Keyword
            Assert.That(nonModalEdgeSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", nonModalEdgeSyntax.Keyword);
            Assert.That(nonModalEdgeSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", nonModalEdgeSyntax.Keyword);
            // NonModalEdgeKeyword
            Assert.That(nonModalEdgeSyntax.NonModalEdgeKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", nonModalEdgeSyntax.NonModalEdgeKeyword);
            Assert.That(nonModalEdgeSyntax.NonModalEdgeKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", nonModalEdgeSyntax.NonModalEdgeKeyword);
		}

        // EndTargetNodeSyntax
		[Test]
		public void TestEndTargetNodeSyntax() {
			
        var endTargetNodeSyntax=Syntax.ParseEndTargetNode("");
            // EndKeyword
            Assert.That(endTargetNodeSyntax.EndKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endTargetNodeSyntax.EndKeyword);
            Assert.That(endTargetNodeSyntax.EndKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endTargetNodeSyntax.EndKeyword);
		}

        // ParameterListSyntax
		[Test]
		public void TestParameterListSyntax() {
			
		}

        // SignalTriggerSyntax
		[Test]
		public void TestSignalTriggerSyntax() {
			
        var signalTriggerSyntax=Syntax.ParseSignalTrigger("");
            // OnKeyword
            Assert.That(signalTriggerSyntax.OnKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", signalTriggerSyntax.OnKeyword);
            Assert.That(signalTriggerSyntax.OnKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", signalTriggerSyntax.OnKeyword);
		}

        // StringLiteralSyntax
		[Test]
		public void TestStringLiteralSyntax() {
			
        var stringLiteralSyntax=Syntax.ParseStringLiteral("");
            // StringLiteral
            Assert.That(stringLiteralSyntax.StringLiteral.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", stringLiteralSyntax.StringLiteral);
            Assert.That(stringLiteralSyntax.StringLiteral.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", stringLiteralSyntax.StringLiteral);
		}

        // InitSourceNodeSyntax
		[Test]
		public void TestInitSourceNodeSyntax() {
			
        var initSourceNodeSyntax=Syntax.ParseInitSourceNode("");
            // InitKeyword
            Assert.That(initSourceNodeSyntax.InitKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initSourceNodeSyntax.InitKeyword);
            Assert.That(initSourceNodeSyntax.InitKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initSourceNodeSyntax.InitKeyword);
		}

        // TaskDefinitionSyntax
		[Test]
		public void TestTaskDefinitionSyntax() {
			
        var taskDefinitionSyntax=Syntax.ParseTaskDefinition("");
            // TaskKeyword
            Assert.That(taskDefinitionSyntax.TaskKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.TaskKeyword);
            Assert.That(taskDefinitionSyntax.TaskKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.TaskKeyword);
            // Identifier
            Assert.That(taskDefinitionSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.Identifier);
            Assert.That(taskDefinitionSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.Identifier);
            // OpenBrace
            Assert.That(taskDefinitionSyntax.OpenBrace.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.OpenBrace);
            Assert.That(taskDefinitionSyntax.OpenBrace.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.OpenBrace);
            // CloseBrace
            Assert.That(taskDefinitionSyntax.CloseBrace.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.CloseBrace);
            Assert.That(taskDefinitionSyntax.CloseBrace.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDefinitionSyntax.CloseBrace);
		}

        // CodeDeclarationSyntax
		[Test]
		public void TestCodeDeclarationSyntax() {
			
        var codeDeclarationSyntax=Syntax.ParseCodeDeclaration("");
            // CodeKeyword
            Assert.That(codeDeclarationSyntax.CodeKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.CodeKeyword);
            Assert.That(codeDeclarationSyntax.CodeKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.CodeKeyword);
            // StringLiteral
            Assert.That(codeDeclarationSyntax.StringLiteral.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.StringLiteral);
            Assert.That(codeDeclarationSyntax.StringLiteral.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.StringLiteral);
            // OpenBracket
            Assert.That(codeDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.OpenBracket);
            Assert.That(codeDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.Keyword);
            Assert.That(codeDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.CloseBracket);
            Assert.That(codeDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDeclarationSyntax.CloseBracket);
		}

        // TaskDeclarationSyntax
		[Test]
		public void TestTaskDeclarationSyntax() {
			
        var taskDeclarationSyntax=Syntax.ParseTaskDeclaration("");
            // TaskrefKeyword
            Assert.That(taskDeclarationSyntax.TaskrefKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.TaskrefKeyword);
            Assert.That(taskDeclarationSyntax.TaskrefKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.TaskrefKeyword);
            // Identifier
            Assert.That(taskDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.Identifier);
            Assert.That(taskDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.Identifier);
            // OpenBrace
            Assert.That(taskDeclarationSyntax.OpenBrace.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.OpenBrace);
            Assert.That(taskDeclarationSyntax.OpenBrace.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.OpenBrace);
            // CloseBrace
            Assert.That(taskDeclarationSyntax.CloseBrace.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.CloseBrace);
            Assert.That(taskDeclarationSyntax.CloseBrace.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskDeclarationSyntax.CloseBrace);
		}

        // IncludeDirectiveSyntax
		[Test]
		public void TestIncludeDirectiveSyntax() {
			
        var includeDirectiveSyntax=Syntax.ParseIncludeDirective("");
            // TaskrefKeyword
            Assert.That(includeDirectiveSyntax.TaskrefKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.TaskrefKeyword);
            Assert.That(includeDirectiveSyntax.TaskrefKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.TaskrefKeyword);
            // StringLiteral
            Assert.That(includeDirectiveSyntax.StringLiteral.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.StringLiteral);
            Assert.That(includeDirectiveSyntax.StringLiteral.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.StringLiteral);
            // Semicolon
            Assert.That(includeDirectiveSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.Semicolon);
            Assert.That(includeDirectiveSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", includeDirectiveSyntax.Semicolon);
		}

        // IfConditionClauseSyntax
		[Test]
		public void TestIfConditionClauseSyntax() {
			
        var ifConditionClauseSyntax=Syntax.ParseIfConditionClause("");
            // IfKeyword
            Assert.That(ifConditionClauseSyntax.IfKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", ifConditionClauseSyntax.IfKeyword);
            Assert.That(ifConditionClauseSyntax.IfKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", ifConditionClauseSyntax.IfKeyword);
		}

        // ArrayRankSpecifierSyntax
		[Test]
		public void TestArrayRankSpecifierSyntax() {
			
        var arrayRankSpecifierSyntax=Syntax.ParseArrayRankSpecifier("");
            // OpenBracket
            Assert.That(arrayRankSpecifierSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", arrayRankSpecifierSyntax.OpenBracket);
            Assert.That(arrayRankSpecifierSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", arrayRankSpecifierSyntax.OpenBracket);
            // CloseBracket
            Assert.That(arrayRankSpecifierSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", arrayRankSpecifierSyntax.CloseBracket);
            Assert.That(arrayRankSpecifierSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", arrayRankSpecifierSyntax.CloseBracket);
		}

        // CodeGenerationUnitSyntax
		[Test]
		public void TestCodeGenerationUnitSyntax() {
			
		}

        // EndNodeDeclarationSyntax
		[Test]
		public void TestEndNodeDeclarationSyntax() {
			
        var endNodeDeclarationSyntax=Syntax.ParseEndNodeDeclaration("");
            // EndKeyword
            Assert.That(endNodeDeclarationSyntax.EndKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endNodeDeclarationSyntax.EndKeyword);
            Assert.That(endNodeDeclarationSyntax.EndKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endNodeDeclarationSyntax.EndKeyword);
            // Semicolon
            Assert.That(endNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endNodeDeclarationSyntax.Semicolon);
            Assert.That(endNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", endNodeDeclarationSyntax.Semicolon);
		}

        // SpontaneousTriggerSyntax
		[Test]
		public void TestSpontaneousTriggerSyntax() {
			
        var spontaneousTriggerSyntax=Syntax.ParseSpontaneousTrigger("");
            // SpontaneousKeyword
            Assert.That(spontaneousTriggerSyntax.SpontaneousKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", spontaneousTriggerSyntax.SpontaneousKeyword);
            Assert.That(spontaneousTriggerSyntax.SpontaneousKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", spontaneousTriggerSyntax.SpontaneousKeyword);
		}

        // CodeBaseDeclarationSyntax
		[Test]
		public void TestCodeBaseDeclarationSyntax() {
			
        var codeBaseDeclarationSyntax=Syntax.ParseCodeBaseDeclaration("");
            // BaseKeyword
            Assert.That(codeBaseDeclarationSyntax.BaseKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.BaseKeyword);
            Assert.That(codeBaseDeclarationSyntax.BaseKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.BaseKeyword);
            // OpenBracket
            Assert.That(codeBaseDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.OpenBracket);
            Assert.That(codeBaseDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeBaseDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.Keyword);
            Assert.That(codeBaseDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeBaseDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.CloseBracket);
            Assert.That(codeBaseDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeBaseDeclarationSyntax.CloseBracket);
		}

        // ElseConditionClauseSyntax
		[Test]
		public void TestElseConditionClauseSyntax() {
			
        var elseConditionClauseSyntax=Syntax.ParseElseConditionClause("");
            // ElseKeyword
            Assert.That(elseConditionClauseSyntax.ElseKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", elseConditionClauseSyntax.ElseKeyword);
            Assert.That(elseConditionClauseSyntax.ElseKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", elseConditionClauseSyntax.ElseKeyword);
		}

        // ExitNodeDeclarationSyntax
		[Test]
		public void TestExitNodeDeclarationSyntax() {
			
        var exitNodeDeclarationSyntax=Syntax.ParseExitNodeDeclaration("");
            // ExitKeyword
            Assert.That(exitNodeDeclarationSyntax.ExitKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.ExitKeyword);
            Assert.That(exitNodeDeclarationSyntax.ExitKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.ExitKeyword);
            // Identifier
            Assert.That(exitNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.Identifier);
            Assert.That(exitNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.Identifier);
            // Semicolon
            Assert.That(exitNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.Semicolon);
            Assert.That(exitNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitNodeDeclarationSyntax.Semicolon);
		}

        // InitNodeDeclarationSyntax
		[Test]
		public void TestInitNodeDeclarationSyntax() {
			
        var initNodeDeclarationSyntax=Syntax.ParseInitNodeDeclaration("");
            // InitKeyword
            Assert.That(initNodeDeclarationSyntax.InitKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.InitKeyword);
            Assert.That(initNodeDeclarationSyntax.InitKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.InitKeyword);
            // Identifier
            Assert.That(initNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.Identifier);
            Assert.That(initNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.Identifier);
            // Semicolon
            Assert.That(initNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.Semicolon);
            Assert.That(initNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", initNodeDeclarationSyntax.Semicolon);
		}

        // TaskNodeDeclarationSyntax
		[Test]
		public void TestTaskNodeDeclarationSyntax() {
			
        var taskNodeDeclarationSyntax=Syntax.ParseTaskNodeDeclaration("");
            // TaskKeyword
            Assert.That(taskNodeDeclarationSyntax.TaskKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.TaskKeyword);
            Assert.That(taskNodeDeclarationSyntax.TaskKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.TaskKeyword);
            // Identifier
            Assert.That(taskNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.Identifier);
            Assert.That(taskNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.Identifier);
            // IdentifierAlias
            Assert.That(taskNodeDeclarationSyntax.IdentifierAlias.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.IdentifierAlias);
            Assert.That(taskNodeDeclarationSyntax.IdentifierAlias.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.IdentifierAlias);
            // Semicolon
            Assert.That(taskNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.Semicolon);
            Assert.That(taskNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", taskNodeDeclarationSyntax.Semicolon);
		}

        // ViewNodeDeclarationSyntax
		[Test]
		public void TestViewNodeDeclarationSyntax() {
			
        var viewNodeDeclarationSyntax=Syntax.ParseViewNodeDeclaration("");
            // ViewKeyword
            Assert.That(viewNodeDeclarationSyntax.ViewKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.ViewKeyword);
            Assert.That(viewNodeDeclarationSyntax.ViewKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.ViewKeyword);
            // Identifier
            Assert.That(viewNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.Identifier);
            Assert.That(viewNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.Identifier);
            // Semicolon
            Assert.That(viewNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.Semicolon);
            Assert.That(viewNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", viewNodeDeclarationSyntax.Semicolon);
		}

        // CodeUsingDeclarationSyntax
		[Test]
		public void TestCodeUsingDeclarationSyntax() {
			
        var codeUsingDeclarationSyntax=Syntax.ParseCodeUsingDeclaration("");
            // UsingKeyword
            Assert.That(codeUsingDeclarationSyntax.UsingKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.UsingKeyword);
            Assert.That(codeUsingDeclarationSyntax.UsingKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.UsingKeyword);
            // OpenBracket
            Assert.That(codeUsingDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.OpenBracket);
            Assert.That(codeUsingDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeUsingDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.Keyword);
            Assert.That(codeUsingDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeUsingDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.CloseBracket);
            Assert.That(codeUsingDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeUsingDeclarationSyntax.CloseBracket);
		}

        // IdentifierSourceNodeSyntax
		[Test]
		public void TestIdentifierSourceNodeSyntax() {
			
        var identifierSourceNodeSyntax=Syntax.ParseIdentifierSourceNode("");
            // Identifier
            Assert.That(identifierSourceNodeSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierSourceNodeSyntax.Identifier);
            Assert.That(identifierSourceNodeSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierSourceNodeSyntax.Identifier);
		}

        // IdentifierTargetNodeSyntax
		[Test]
		public void TestIdentifierTargetNodeSyntax() {
			
        var identifierTargetNodeSyntax=Syntax.ParseIdentifierTargetNode("");
            // Identifier
            Assert.That(identifierTargetNodeSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierTargetNodeSyntax.Identifier);
            Assert.That(identifierTargetNodeSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", identifierTargetNodeSyntax.Identifier);
		}

        // NodeDeclarationBlockSyntax
		[Test]
		public void TestNodeDeclarationBlockSyntax() {
			
		}

        // TransitionDefinitionSyntax
		[Test]
		public void TestTransitionDefinitionSyntax() {
			
        var transitionDefinitionSyntax=Syntax.ParseTransitionDefinition("");
            // Semicolon
            Assert.That(transitionDefinitionSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", transitionDefinitionSyntax.Semicolon);
            Assert.That(transitionDefinitionSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", transitionDefinitionSyntax.Semicolon);
		}

        // ChoiceNodeDeclarationSyntax
		[Test]
		public void TestChoiceNodeDeclarationSyntax() {
			
        var choiceNodeDeclarationSyntax=Syntax.ParseChoiceNodeDeclaration("");
            // ChoiceKeyword
            Assert.That(choiceNodeDeclarationSyntax.ChoiceKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.ChoiceKeyword);
            Assert.That(choiceNodeDeclarationSyntax.ChoiceKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.ChoiceKeyword);
            // Identifier
            Assert.That(choiceNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.Identifier);
            Assert.That(choiceNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.Identifier);
            // Semicolon
            Assert.That(choiceNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.Semicolon);
            Assert.That(choiceNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", choiceNodeDeclarationSyntax.Semicolon);
		}

        // CodeParamsDeclarationSyntax
		[Test]
		public void TestCodeParamsDeclarationSyntax() {
			
        var codeParamsDeclarationSyntax=Syntax.ParseCodeParamsDeclaration("");
            // ParamsKeyword
            Assert.That(codeParamsDeclarationSyntax.ParamsKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.ParamsKeyword);
            Assert.That(codeParamsDeclarationSyntax.ParamsKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.ParamsKeyword);
            // OpenBracket
            Assert.That(codeParamsDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.OpenBracket);
            Assert.That(codeParamsDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeParamsDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.Keyword);
            Assert.That(codeParamsDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeParamsDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.CloseBracket);
            Assert.That(codeParamsDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeParamsDeclarationSyntax.CloseBracket);
		}

        // CodeResultDeclarationSyntax
		[Test]
		public void TestCodeResultDeclarationSyntax() {
			
        var codeResultDeclarationSyntax=Syntax.ParseCodeResultDeclaration("");
            // ResultKeyword
            Assert.That(codeResultDeclarationSyntax.ResultKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.ResultKeyword);
            Assert.That(codeResultDeclarationSyntax.ResultKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.ResultKeyword);
            // OpenBracket
            Assert.That(codeResultDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.OpenBracket);
            Assert.That(codeResultDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeResultDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.Keyword);
            Assert.That(codeResultDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeResultDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.CloseBracket);
            Assert.That(codeResultDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeResultDeclarationSyntax.CloseBracket);
		}

        // DialogNodeDeclarationSyntax
		[Test]
		public void TestDialogNodeDeclarationSyntax() {
			
        var dialogNodeDeclarationSyntax=Syntax.ParseDialogNodeDeclaration("");
            // DialogKeyword
            Assert.That(dialogNodeDeclarationSyntax.DialogKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.DialogKeyword);
            Assert.That(dialogNodeDeclarationSyntax.DialogKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.DialogKeyword);
            // Identifier
            Assert.That(dialogNodeDeclarationSyntax.Identifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.Identifier);
            Assert.That(dialogNodeDeclarationSyntax.Identifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.Identifier);
            // Semicolon
            Assert.That(dialogNodeDeclarationSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.Semicolon);
            Assert.That(dialogNodeDeclarationSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", dialogNodeDeclarationSyntax.Semicolon);
		}

        // ElseIfConditionClauseSyntax
		[Test]
		public void TestElseIfConditionClauseSyntax() {
			
		}

        // IdentifierOrStringListSyntax
		[Test]
		public void TestIdentifierOrStringListSyntax() {
			
		}

        // CodeNamespaceDeclarationSyntax
		[Test]
		public void TestCodeNamespaceDeclarationSyntax() {
			
        var codeNamespaceDeclarationSyntax=Syntax.ParseCodeNamespaceDeclaration("");
            // NamespaceprefixKeyword
            Assert.That(codeNamespaceDeclarationSyntax.NamespaceprefixKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.NamespaceprefixKeyword);
            Assert.That(codeNamespaceDeclarationSyntax.NamespaceprefixKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.NamespaceprefixKeyword);
            // OpenBracket
            Assert.That(codeNamespaceDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.OpenBracket);
            Assert.That(codeNamespaceDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeNamespaceDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.Keyword);
            Assert.That(codeNamespaceDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeNamespaceDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.CloseBracket);
            Assert.That(codeNamespaceDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNamespaceDeclarationSyntax.CloseBracket);
		}

        // ExitTransitionDefinitionSyntax
		[Test]
		public void TestExitTransitionDefinitionSyntax() {
			
        var exitTransitionDefinitionSyntax=Syntax.ParseExitTransitionDefinition("");
            // Colon
            Assert.That(exitTransitionDefinitionSyntax.Colon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.Colon);
            Assert.That(exitTransitionDefinitionSyntax.Colon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.Colon);
            // ExitIdentifier
            Assert.That(exitTransitionDefinitionSyntax.ExitIdentifier.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.ExitIdentifier);
            Assert.That(exitTransitionDefinitionSyntax.ExitIdentifier.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.ExitIdentifier);
            // Semicolon
            Assert.That(exitTransitionDefinitionSyntax.Semicolon.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.Semicolon);
            Assert.That(exitTransitionDefinitionSyntax.Semicolon.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", exitTransitionDefinitionSyntax.Semicolon);
		}

        // CodeGenerateToDeclarationSyntax
		[Test]
		public void TestCodeGenerateToDeclarationSyntax() {
			
        var codeGenerateToDeclarationSyntax=Syntax.ParseCodeGenerateToDeclaration("");
            // GeneratetoKeyword
            Assert.That(codeGenerateToDeclarationSyntax.GeneratetoKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.GeneratetoKeyword);
            Assert.That(codeGenerateToDeclarationSyntax.GeneratetoKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.GeneratetoKeyword);
            // StringLiteral
            Assert.That(codeGenerateToDeclarationSyntax.StringLiteral.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.StringLiteral);
            Assert.That(codeGenerateToDeclarationSyntax.StringLiteral.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.StringLiteral);
            // OpenBracket
            Assert.That(codeGenerateToDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.OpenBracket);
            Assert.That(codeGenerateToDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeGenerateToDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.Keyword);
            Assert.That(codeGenerateToDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeGenerateToDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.CloseBracket);
            Assert.That(codeGenerateToDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeGenerateToDeclarationSyntax.CloseBracket);
		}

        // TransitionDefinitionBlockSyntax
		[Test]
		public void TestTransitionDefinitionBlockSyntax() {
			
		}

        // CodeDoNotInjectDeclarationSyntax
		[Test]
		public void TestCodeDoNotInjectDeclarationSyntax() {
			
        var codeDoNotInjectDeclarationSyntax=Syntax.ParseCodeDoNotInjectDeclaration("");
            // DonotinjectKeyword
            Assert.That(codeDoNotInjectDeclarationSyntax.DonotinjectKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.DonotinjectKeyword);
            Assert.That(codeDoNotInjectDeclarationSyntax.DonotinjectKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.DonotinjectKeyword);
            // OpenBracket
            Assert.That(codeDoNotInjectDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.OpenBracket);
            Assert.That(codeDoNotInjectDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeDoNotInjectDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.Keyword);
            Assert.That(codeDoNotInjectDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeDoNotInjectDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.CloseBracket);
            Assert.That(codeDoNotInjectDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeDoNotInjectDeclarationSyntax.CloseBracket);
		}

        // CodeAbstractMethodDeclarationSyntax
		[Test]
		public void TestCodeAbstractMethodDeclarationSyntax() {
			
        var codeAbstractMethodDeclarationSyntax=Syntax.ParseCodeAbstractMethodDeclaration("");
            // AbstractmethodKeyword
            Assert.That(codeAbstractMethodDeclarationSyntax.AbstractmethodKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.AbstractmethodKeyword);
            Assert.That(codeAbstractMethodDeclarationSyntax.AbstractmethodKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.AbstractmethodKeyword);
            // OpenBracket
            Assert.That(codeAbstractMethodDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.OpenBracket);
            Assert.That(codeAbstractMethodDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeAbstractMethodDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.Keyword);
            Assert.That(codeAbstractMethodDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeAbstractMethodDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.CloseBracket);
            Assert.That(codeAbstractMethodDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeAbstractMethodDeclarationSyntax.CloseBracket);
		}

        // CodeNotImplementedDeclarationSyntax
		[Test]
		public void TestCodeNotImplementedDeclarationSyntax() {
			
        var codeNotImplementedDeclarationSyntax=Syntax.ParseCodeNotImplementedDeclaration("");
            // NotimplementedKeyword
            Assert.That(codeNotImplementedDeclarationSyntax.NotimplementedKeyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.NotimplementedKeyword);
            Assert.That(codeNotImplementedDeclarationSyntax.NotimplementedKeyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.NotimplementedKeyword);
            // OpenBracket
            Assert.That(codeNotImplementedDeclarationSyntax.OpenBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.OpenBracket);
            Assert.That(codeNotImplementedDeclarationSyntax.OpenBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.OpenBracket);
            // Keyword
            Assert.That(codeNotImplementedDeclarationSyntax.Keyword.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.Keyword);
            Assert.That(codeNotImplementedDeclarationSyntax.Keyword.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.Keyword);
            // CloseBracket
            Assert.That(codeNotImplementedDeclarationSyntax.CloseBracket.IsMissing, Is.True, "Das Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.CloseBracket);
            Assert.That(codeNotImplementedDeclarationSyntax.CloseBracket.Extent.IsMissing, Is.True, "Extent des Token '{0}' sollte als 'fehlend' gekennzeichnet sein.", codeNotImplementedDeclarationSyntax.CloseBracket);
		}

	}
}
