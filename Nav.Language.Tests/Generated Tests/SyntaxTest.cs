 
//==================================================
// HINWEIS: Diese Datei wurde am 19.02.2016 21:28:51
//			automatisch generiert!
//==================================================
using NUnit.Framework;
using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

	[TestFixture]
	public class SyntaxTests {
		
		[Test]
		[Description("Syntax: 'do \"instruction\"'\r\n")]
		public void TestDoClauseSyntax() {
			var doClauseSyntax=Syntax.ParseDoClause(SampleSyntax.Of<DoClauseSyntax>());
			
			foreach(var diagnostic in doClauseSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(doClauseSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in doClauseSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '-->'\r\n")]
		public void TestGoToEdgeSyntax() {
			var goToEdgeSyntax=Syntax.ParseGoToEdge(SampleSyntax.Of<GoToEdgeSyntax>());
			
			foreach(var diagnostic in goToEdgeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(goToEdgeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in goToEdgeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Type[]'\r\n")]
		public void TestArrayTypeSyntax() {
			var arrayTypeSyntax=Syntax.ParseArrayType(SampleSyntax.Of<ArrayTypeSyntax>());
			
			foreach(var diagnostic in arrayTypeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(arrayTypeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in arrayTypeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'o->'\r\n")]
		public void TestModalEdgeSyntax() {
			var modalEdgeSyntax=Syntax.ParseModalEdge(SampleSyntax.Of<ModalEdgeSyntax>());
			
			foreach(var diagnostic in modalEdgeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(modalEdgeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in modalEdgeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Type param'\r\n")]
		public void TestParameterSyntax() {
			var parameterSyntax=Syntax.ParseParameter(SampleSyntax.Of<ParameterSyntax>());
			
			foreach(var diagnostic in parameterSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(parameterSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in parameterSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Identifier'\r\n")]
		public void TestIdentifierSyntax() {
			var identifierSyntax=Syntax.ParseIdentifier(SampleSyntax.Of<IdentifierSyntax>());
			
			foreach(var diagnostic in identifierSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(identifierSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in identifierSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Identifier'\r\n")]
		public void TestSimpleTypeSyntax() {
			var simpleTypeSyntax=Syntax.ParseSimpleType(SampleSyntax.Of<SimpleTypeSyntax>());
			
			foreach(var diagnostic in simpleTypeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(simpleTypeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in simpleTypeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Type<T1, T2<T3, T4>>'\r\n")]
		public void TestGenericTypeSyntax() {
			var genericTypeSyntax=Syntax.ParseGenericType(SampleSyntax.Of<GenericTypeSyntax>());
			
			foreach(var diagnostic in genericTypeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(genericTypeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in genericTypeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '==>'\r\n")]
		public void TestNonModalEdgeSyntax() {
			var nonModalEdgeSyntax=Syntax.ParseNonModalEdge(SampleSyntax.Of<NonModalEdgeSyntax>());
			
			foreach(var diagnostic in nonModalEdgeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(nonModalEdgeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in nonModalEdgeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'end'\r\n")]
		public void TestEndTargetNodeSyntax() {
			var endTargetNodeSyntax=Syntax.ParseEndTargetNode(SampleSyntax.Of<EndTargetNodeSyntax>());
			
			foreach(var diagnostic in endTargetNodeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(endTargetNodeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in endTargetNodeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'T1 param1, T2 param2'\r\n")]
		public void TestParameterListSyntax() {
			var parameterListSyntax=Syntax.ParseParameterList(SampleSyntax.Of<ParameterListSyntax>());
			
			foreach(var diagnostic in parameterListSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(parameterListSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in parameterListSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'on Trigger'\r\n")]
		public void TestSignalTriggerSyntax() {
			var signalTriggerSyntax=Syntax.ParseSignalTrigger(SampleSyntax.Of<SignalTriggerSyntax>());
			
			foreach(var diagnostic in signalTriggerSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(signalTriggerSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in signalTriggerSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '\"StringLiteral\"'\r\n")]
		public void TestStringLiteralSyntax() {
			var stringLiteralSyntax=Syntax.ParseStringLiteral(SampleSyntax.Of<StringLiteralSyntax>());
			
			foreach(var diagnostic in stringLiteralSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(stringLiteralSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in stringLiteralSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'init'\r\n")]
		public void TestInitSourceNodeSyntax() {
			var initSourceNodeSyntax=Syntax.ParseInitSourceNode(SampleSyntax.Of<InitSourceNodeSyntax>());
			
			foreach(var diagnostic in initSourceNodeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(initSourceNodeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in initSourceNodeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'task Task { };'\r\n")]
		public void TestTaskDefinitionSyntax() {
			var taskDefinitionSyntax=Syntax.ParseTaskDefinition(SampleSyntax.Of<TaskDefinitionSyntax>());
			
			foreach(var diagnostic in taskDefinitionSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(taskDefinitionSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in taskDefinitionSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[code \"code goes here\"]'\r\n")]
		public void TestCodeDeclarationSyntax() {
			var codeDeclarationSyntax=Syntax.ParseCodeDeclaration(SampleSyntax.Of<CodeDeclarationSyntax>());
			
			foreach(var diagnostic in codeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'taskref Task { };'\r\n")]
		public void TestTaskDeclarationSyntax() {
			var taskDeclarationSyntax=Syntax.ParseTaskDeclaration(SampleSyntax.Of<TaskDeclarationSyntax>());
			
			foreach(var diagnostic in taskDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(taskDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in taskDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'taskref \"file.nav\";'\r\n")]
		public void TestIncludeDirectiveSyntax() {
			var includeDirectiveSyntax=Syntax.ParseIncludeDirective(SampleSyntax.Of<IncludeDirectiveSyntax>());
			
			foreach(var diagnostic in includeDirectiveSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(includeDirectiveSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in includeDirectiveSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'if Condition'\r\n")]
		public void TestIfConditionClauseSyntax() {
			var ifConditionClauseSyntax=Syntax.ParseIfConditionClause(SampleSyntax.Of<IfConditionClauseSyntax>());
			
			foreach(var diagnostic in ifConditionClauseSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(ifConditionClauseSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in ifConditionClauseSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[]'\r\n")]
		public void TestArrayRankSpecifierSyntax() {
			var arrayRankSpecifierSyntax=Syntax.ParseArrayRankSpecifier(SampleSyntax.Of<ArrayRankSpecifierSyntax>());
			
			foreach(var diagnostic in arrayRankSpecifierSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(arrayRankSpecifierSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in arrayRankSpecifierSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: ''\r\n")]
		public void TestCodeGenerationUnitSyntax() {
			var codeGenerationUnitSyntax=Syntax.ParseCodeGenerationUnit(SampleSyntax.Of<CodeGenerationUnitSyntax>());
			
			foreach(var diagnostic in codeGenerationUnitSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeGenerationUnitSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeGenerationUnitSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'end;'\r\n")]
		public void TestEndNodeDeclarationSyntax() {
			var endNodeDeclarationSyntax=Syntax.ParseEndNodeDeclaration(SampleSyntax.Of<EndNodeDeclarationSyntax>());
			
			foreach(var diagnostic in endNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(endNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in endNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'spontaneous'\r\n")]
		public void TestSpontaneousTriggerSyntax() {
			var spontaneousTriggerSyntax=Syntax.ParseSpontaneousTrigger(SampleSyntax.Of<SpontaneousTriggerSyntax>());
			
			foreach(var diagnostic in spontaneousTriggerSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(spontaneousTriggerSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in spontaneousTriggerSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[base StandardWFS<TSType> : IWFServiceBase, IBeginWFSType]'\r\n")]
		public void TestCodeBaseDeclarationSyntax() {
			var codeBaseDeclarationSyntax=Syntax.ParseCodeBaseDeclaration(SampleSyntax.Of<CodeBaseDeclarationSyntax>());
			
			foreach(var diagnostic in codeBaseDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeBaseDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeBaseDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'else'\r\n")]
		public void TestElseConditionClauseSyntax() {
			var elseConditionClauseSyntax=Syntax.ParseElseConditionClause(SampleSyntax.Of<ElseConditionClauseSyntax>());
			
			foreach(var diagnostic in elseConditionClauseSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(elseConditionClauseSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in elseConditionClauseSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'exit Identifier;'\r\n")]
		public void TestExitNodeDeclarationSyntax() {
			var exitNodeDeclarationSyntax=Syntax.ParseExitNodeDeclaration(SampleSyntax.Of<ExitNodeDeclarationSyntax>());
			
			foreach(var diagnostic in exitNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(exitNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in exitNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'init Identifier [abstractmethod] [params T1 param1, T2<T3, T4<T5>> param2, T6[][] param3] do Instruction;'\r\n")]
		public void TestInitNodeDeclarationSyntax() {
			var initNodeDeclarationSyntax=Syntax.ParseInitNodeDeclaration(SampleSyntax.Of<InitNodeDeclarationSyntax>());
			
			foreach(var diagnostic in initNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(initNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in initNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'task Identifier Alias [donotinject] [abstractmethod];'\r\n")]
		public void TestTaskNodeDeclarationSyntax() {
			var taskNodeDeclarationSyntax=Syntax.ParseTaskNodeDeclaration(SampleSyntax.Of<TaskNodeDeclarationSyntax>());
			
			foreach(var diagnostic in taskNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(taskNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in taskNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'view Identifier;'\r\n")]
		public void TestViewNodeDeclarationSyntax() {
			var viewNodeDeclarationSyntax=Syntax.ParseViewNodeDeclaration(SampleSyntax.Of<ViewNodeDeclarationSyntax>());
			
			foreach(var diagnostic in viewNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(viewNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in viewNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[using Namespace]'\r\n")]
		public void TestCodeUsingDeclarationSyntax() {
			var codeUsingDeclarationSyntax=Syntax.ParseCodeUsingDeclaration(SampleSyntax.Of<CodeUsingDeclarationSyntax>());
			
			foreach(var diagnostic in codeUsingDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeUsingDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeUsingDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Identifier'\r\n")]
		public void TestIdentifierSourceNodeSyntax() {
			var identifierSourceNodeSyntax=Syntax.ParseIdentifierSourceNode(SampleSyntax.Of<IdentifierSourceNodeSyntax>());
			
			foreach(var diagnostic in identifierSourceNodeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(identifierSourceNodeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in identifierSourceNodeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Identifier (identifierOrStringList)'\r\n")]
		public void TestIdentifierTargetNodeSyntax() {
			var identifierTargetNodeSyntax=Syntax.ParseIdentifierTargetNode(SampleSyntax.Of<IdentifierTargetNodeSyntax>());
			
			foreach(var diagnostic in identifierTargetNodeSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(identifierTargetNodeSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in identifierTargetNodeSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: ''\r\n")]
		public void TestNodeDeclarationBlockSyntax() {
			var nodeDeclarationBlockSyntax=Syntax.ParseNodeDeclarationBlock(SampleSyntax.Of<NodeDeclarationBlockSyntax>());
			
			foreach(var diagnostic in nodeDeclarationBlockSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(nodeDeclarationBlockSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in nodeDeclarationBlockSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Node --> Target on Trigger if Condition do Instruction;'\r\n")]
		public void TestTransitionDefinitionSyntax() {
			var transitionDefinitionSyntax=Syntax.ParseTransitionDefinition(SampleSyntax.Of<TransitionDefinitionSyntax>());
			
			foreach(var diagnostic in transitionDefinitionSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(transitionDefinitionSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in transitionDefinitionSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'choice ChoiceName;'\r\n")]
		public void TestChoiceNodeDeclarationSyntax() {
			var choiceNodeDeclarationSyntax=Syntax.ParseChoiceNodeDeclaration(SampleSyntax.Of<ChoiceNodeDeclarationSyntax>());
			
			foreach(var diagnostic in choiceNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(choiceNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in choiceNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[params Type1 p1, Type2 p2]'\r\n")]
		public void TestCodeParamsDeclarationSyntax() {
			var codeParamsDeclarationSyntax=Syntax.ParseCodeParamsDeclaration(SampleSyntax.Of<CodeParamsDeclarationSyntax>());
			
			foreach(var diagnostic in codeParamsDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeParamsDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeParamsDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[result Type p]'\r\n")]
		public void TestCodeResultDeclarationSyntax() {
			var codeResultDeclarationSyntax=Syntax.ParseCodeResultDeclaration(SampleSyntax.Of<CodeResultDeclarationSyntax>());
			
			foreach(var diagnostic in codeResultDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeResultDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeResultDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'dialog Identifier;'\r\n")]
		public void TestDialogNodeDeclarationSyntax() {
			var dialogNodeDeclarationSyntax=Syntax.ParseDialogNodeDeclaration(SampleSyntax.Of<DialogNodeDeclarationSyntax>());
			
			foreach(var diagnostic in dialogNodeDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(dialogNodeDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in dialogNodeDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'else if Condition'\r\n")]
		public void TestElseIfConditionClauseSyntax() {
			var elseIfConditionClauseSyntax=Syntax.ParseElseIfConditionClause(SampleSyntax.Of<ElseIfConditionClauseSyntax>());
			
			foreach(var diagnostic in elseIfConditionClauseSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(elseIfConditionClauseSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in elseIfConditionClauseSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'Identifier, \"StringLiteral\"'\r\n")]
		public void TestIdentifierOrStringListSyntax() {
			var identifierOrStringListSyntax=Syntax.ParseIdentifierOrStringList(SampleSyntax.Of<IdentifierOrStringListSyntax>());
			
			foreach(var diagnostic in identifierOrStringListSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(identifierOrStringListSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in identifierOrStringListSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[namespaceprefix Namespace]'\r\n")]
		public void TestCodeNamespaceDeclarationSyntax() {
			var codeNamespaceDeclarationSyntax=Syntax.ParseCodeNamespaceDeclaration(SampleSyntax.Of<CodeNamespaceDeclarationSyntax>());
			
			foreach(var diagnostic in codeNamespaceDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeNamespaceDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeNamespaceDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: 'SourceNode:ExitIdentifier --> TargetNode if Condition do Instruction;'\r\n")]
		public void TestExitTransitionDefinitionSyntax() {
			var exitTransitionDefinitionSyntax=Syntax.ParseExitTransitionDefinition(SampleSyntax.Of<ExitTransitionDefinitionSyntax>());
			
			foreach(var diagnostic in exitTransitionDefinitionSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(exitTransitionDefinitionSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in exitTransitionDefinitionSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[generateto \"StringLiteral\"]'\r\n")]
		public void TestCodeGenerateToDeclarationSyntax() {
			var codeGenerateToDeclarationSyntax=Syntax.ParseCodeGenerateToDeclaration(SampleSyntax.Of<CodeGenerateToDeclarationSyntax>());
			
			foreach(var diagnostic in codeGenerateToDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeGenerateToDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeGenerateToDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: ''\r\n")]
		public void TestTransitionDefinitionBlockSyntax() {
			var transitionDefinitionBlockSyntax=Syntax.ParseTransitionDefinitionBlock(SampleSyntax.Of<TransitionDefinitionBlockSyntax>());
			
			foreach(var diagnostic in transitionDefinitionBlockSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(transitionDefinitionBlockSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in transitionDefinitionBlockSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[donotinject]'\r\n")]
		public void TestCodeDoNotInjectDeclarationSyntax() {
			var codeDoNotInjectDeclarationSyntax=Syntax.ParseCodeDoNotInjectDeclaration(SampleSyntax.Of<CodeDoNotInjectDeclarationSyntax>());
			
			foreach(var diagnostic in codeDoNotInjectDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeDoNotInjectDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeDoNotInjectDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[abstractmethod]'\r\n")]
		public void TestCodeAbstractMethodDeclarationSyntax() {
			var codeAbstractMethodDeclarationSyntax=Syntax.ParseCodeAbstractMethodDeclaration(SampleSyntax.Of<CodeAbstractMethodDeclarationSyntax>());
			
			foreach(var diagnostic in codeAbstractMethodDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeAbstractMethodDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeAbstractMethodDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

		[Test]
		[Description("Syntax: '[notimplemented]'\r\n")]
		public void TestCodeNotImplementedDeclarationSyntax() {
			var codeNotImplementedDeclarationSyntax=Syntax.ParseCodeNotImplementedDeclaration(SampleSyntax.Of<CodeNotImplementedDeclarationSyntax>());
			
			foreach(var diagnostic in codeNotImplementedDeclarationSyntax.SyntaxTree.Diagnostics) {
				Assert.Fail("Die Beispiels-Syntax führt zu Syntaxfehlern:\r\n{0}", diagnostic);
			}
			Assert.That(codeNotImplementedDeclarationSyntax.SyntaxTree.Diagnostics.Count, Is.EqualTo(0));
			foreach (var token in codeNotImplementedDeclarationSyntax.ChildTokens()) {
		        Assert.That(token.IsMissing, Is.False, "Ein Token ist als 'fehlend' gekennzeichnet:\r\n{0}", token);
		    }
		}

	}
}
