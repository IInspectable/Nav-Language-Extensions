 
//==================================================
// HINWEIS: Diese Datei wurde am 14.05.2017 14:17:39
//			automatisch generiert!
//==================================================
using NUnit.Framework;
using Pharmatechnik.Nav.Language;
using System;
using System.Linq;
using System.Reflection;
using Pharmatechnik.Nav.Language.Internal;
namespace Nav.Language.Tests; 

[TestFixture]
[Category("Tests noch nicht fertig.")]
public class TokenPropertyNameTests {
		
    // DoClauseSyntax
    [Test]
    public void TestDoClauseSyntax() {
		
        var tokenProps = typeof(DoClauseSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseDoClause(SampleSyntax.Of<DoClauseSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // GoToEdgeSyntax
    [Test]
    public void TestGoToEdgeSyntax() {
		
        var tokenProps = typeof(GoToEdgeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseGoToEdge(SampleSyntax.Of<GoToEdgeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ArrayTypeSyntax
    [Test]
    public void TestArrayTypeSyntax() {
		
        var tokenProps = typeof(ArrayTypeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseArrayType(SampleSyntax.Of<ArrayTypeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ModalEdgeSyntax
    [Test]
    public void TestModalEdgeSyntax() {
		
        var tokenProps = typeof(ModalEdgeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseModalEdge(SampleSyntax.Of<ModalEdgeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ParameterSyntax
    [Test]
    public void TestParameterSyntax() {
		
        var tokenProps = typeof(ParameterSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseParameter(SampleSyntax.Of<ParameterSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IdentifierSyntax
    [Test]
    public void TestIdentifierSyntax() {
		
        var tokenProps = typeof(IdentifierSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIdentifier(SampleSyntax.Of<IdentifierSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // SimpleTypeSyntax
    [Test]
    public void TestSimpleTypeSyntax() {
		
        var tokenProps = typeof(SimpleTypeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseSimpleType(SampleSyntax.Of<SimpleTypeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // GenericTypeSyntax
    [Test]
    public void TestGenericTypeSyntax() {
		
        var tokenProps = typeof(GenericTypeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                  .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseGenericType(SampleSyntax.Of<GenericTypeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // NonModalEdgeSyntax
    [Test]
    public void TestNonModalEdgeSyntax() {
		
        var tokenProps = typeof(NonModalEdgeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                   .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseNonModalEdge(SampleSyntax.Of<NonModalEdgeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // EndTargetNodeSyntax
    [Test]
    public void TestEndTargetNodeSyntax() {
		
        var tokenProps = typeof(EndTargetNodeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseEndTargetNode(SampleSyntax.Of<EndTargetNodeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ParameterListSyntax
    [Test]
    public void TestParameterListSyntax() {
		
        var tokenProps = typeof(ParameterListSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseParameterList(SampleSyntax.Of<ParameterListSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // SignalTriggerSyntax
    [Test]
    public void TestSignalTriggerSyntax() {
		
        var tokenProps = typeof(SignalTriggerSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseSignalTrigger(SampleSyntax.Of<SignalTriggerSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // StringLiteralSyntax
    [Test]
    public void TestStringLiteralSyntax() {
		
        var tokenProps = typeof(StringLiteralSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseStringLiteral(SampleSyntax.Of<StringLiteralSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // InitSourceNodeSyntax
    [Test]
    public void TestInitSourceNodeSyntax() {
		
        var tokenProps = typeof(InitSourceNodeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                     .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseInitSourceNode(SampleSyntax.Of<InitSourceNodeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // TaskDefinitionSyntax
    [Test]
    public void TestTaskDefinitionSyntax() {
		
        var tokenProps = typeof(TaskDefinitionSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                     .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseTaskDefinition(SampleSyntax.Of<TaskDefinitionSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeDeclarationSyntax
    [Test]
    public void TestCodeDeclarationSyntax() {
		
        var tokenProps = typeof(CodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeDeclaration(SampleSyntax.Of<CodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // TaskDeclarationSyntax
    [Test]
    public void TestTaskDeclarationSyntax() {
		
        var tokenProps = typeof(TaskDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseTaskDeclaration(SampleSyntax.Of<TaskDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IncludeDirectiveSyntax
    [Test]
    public void TestIncludeDirectiveSyntax() {
		
        var tokenProps = typeof(IncludeDirectiveSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                       .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIncludeDirective(SampleSyntax.Of<IncludeDirectiveSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IfConditionClauseSyntax
    [Test]
    public void TestIfConditionClauseSyntax() {
		
        var tokenProps = typeof(IfConditionClauseSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIfConditionClause(SampleSyntax.Of<IfConditionClauseSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ArrayRankSpecifierSyntax
    [Test]
    public void TestArrayRankSpecifierSyntax() {
		
        var tokenProps = typeof(ArrayRankSpecifierSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                         .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseArrayRankSpecifier(SampleSyntax.Of<ArrayRankSpecifierSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeGenerationUnitSyntax
    [Test]
    public void TestCodeGenerationUnitSyntax() {
		
        var tokenProps = typeof(CodeGenerationUnitSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                         .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeGenerationUnit(SampleSyntax.Of<CodeGenerationUnitSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // EndNodeDeclarationSyntax
    [Test]
    public void TestEndNodeDeclarationSyntax() {
		
        var tokenProps = typeof(EndNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                         .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseEndNodeDeclaration(SampleSyntax.Of<EndNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // SpontaneousTriggerSyntax
    [Test]
    public void TestSpontaneousTriggerSyntax() {
		
        var tokenProps = typeof(SpontaneousTriggerSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                         .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseSpontaneousTrigger(SampleSyntax.Of<SpontaneousTriggerSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeBaseDeclarationSyntax
    [Test]
    public void TestCodeBaseDeclarationSyntax() {
		
        var tokenProps = typeof(CodeBaseDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeBaseDeclaration(SampleSyntax.Of<CodeBaseDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ElseConditionClauseSyntax
    [Test]
    public void TestElseConditionClauseSyntax() {
		
        var tokenProps = typeof(ElseConditionClauseSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseElseConditionClause(SampleSyntax.Of<ElseConditionClauseSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ExitNodeDeclarationSyntax
    [Test]
    public void TestExitNodeDeclarationSyntax() {
		
        var tokenProps = typeof(ExitNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseExitNodeDeclaration(SampleSyntax.Of<ExitNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // InitNodeDeclarationSyntax
    [Test]
    public void TestInitNodeDeclarationSyntax() {
		
        var tokenProps = typeof(InitNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseInitNodeDeclaration(SampleSyntax.Of<InitNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // TaskNodeDeclarationSyntax
    [Test]
    public void TestTaskNodeDeclarationSyntax() {
		
        var tokenProps = typeof(TaskNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseTaskNodeDeclaration(SampleSyntax.Of<TaskNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ViewNodeDeclarationSyntax
    [Test]
    public void TestViewNodeDeclarationSyntax() {
		
        var tokenProps = typeof(ViewNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseViewNodeDeclaration(SampleSyntax.Of<ViewNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeUsingDeclarationSyntax
    [Test]
    public void TestCodeUsingDeclarationSyntax() {
		
        var tokenProps = typeof(CodeUsingDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                           .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeUsingDeclaration(SampleSyntax.Of<CodeUsingDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IdentifierSourceNodeSyntax
    [Test]
    public void TestIdentifierSourceNodeSyntax() {
		
        var tokenProps = typeof(IdentifierSourceNodeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                           .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIdentifierSourceNode(SampleSyntax.Of<IdentifierSourceNodeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IdentifierTargetNodeSyntax
    [Test]
    public void TestIdentifierTargetNodeSyntax() {
		
        var tokenProps = typeof(IdentifierTargetNodeSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                           .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIdentifierTargetNode(SampleSyntax.Of<IdentifierTargetNodeSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // NodeDeclarationBlockSyntax
    [Test]
    public void TestNodeDeclarationBlockSyntax() {
		
        var tokenProps = typeof(NodeDeclarationBlockSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                           .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseNodeDeclarationBlock(SampleSyntax.Of<NodeDeclarationBlockSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // TransitionDefinitionSyntax
    [Test]
    public void TestTransitionDefinitionSyntax() {
		
        var tokenProps = typeof(TransitionDefinitionSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                           .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseTransitionDefinition(SampleSyntax.Of<TransitionDefinitionSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ChoiceNodeDeclarationSyntax
    [Test]
    public void TestChoiceNodeDeclarationSyntax() {
		
        var tokenProps = typeof(ChoiceNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseChoiceNodeDeclaration(SampleSyntax.Of<ChoiceNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeParamsDeclarationSyntax
    [Test]
    public void TestCodeParamsDeclarationSyntax() {
		
        var tokenProps = typeof(CodeParamsDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeParamsDeclaration(SampleSyntax.Of<CodeParamsDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeResultDeclarationSyntax
    [Test]
    public void TestCodeResultDeclarationSyntax() {
		
        var tokenProps = typeof(CodeResultDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeResultDeclaration(SampleSyntax.Of<CodeResultDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // DialogNodeDeclarationSyntax
    [Test]
    public void TestDialogNodeDeclarationSyntax() {
		
        var tokenProps = typeof(DialogNodeDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseDialogNodeDeclaration(SampleSyntax.Of<DialogNodeDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ElseIfConditionClauseSyntax
    [Test]
    public void TestElseIfConditionClauseSyntax() {
		
        var tokenProps = typeof(ElseIfConditionClauseSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseElseIfConditionClause(SampleSyntax.Of<ElseIfConditionClauseSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // IdentifierOrStringListSyntax
    [Test]
    public void TestIdentifierOrStringListSyntax() {
		
        var tokenProps = typeof(IdentifierOrStringListSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                             .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseIdentifierOrStringList(SampleSyntax.Of<IdentifierOrStringListSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeNamespaceDeclarationSyntax
    [Test]
    public void TestCodeNamespaceDeclarationSyntax() {
		
        var tokenProps = typeof(CodeNamespaceDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                               .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeNamespaceDeclaration(SampleSyntax.Of<CodeNamespaceDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // ExitTransitionDefinitionSyntax
    [Test]
    public void TestExitTransitionDefinitionSyntax() {
		
        var tokenProps = typeof(ExitTransitionDefinitionSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                               .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseExitTransitionDefinition(SampleSyntax.Of<ExitTransitionDefinitionSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeGenerateToDeclarationSyntax
    [Test]
    public void TestCodeGenerateToDeclarationSyntax() {
		
        var tokenProps = typeof(CodeGenerateToDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeGenerateToDeclaration(SampleSyntax.Of<CodeGenerateToDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // TransitionDefinitionBlockSyntax
    [Test]
    public void TestTransitionDefinitionBlockSyntax() {
		
        var tokenProps = typeof(TransitionDefinitionBlockSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseTransitionDefinitionBlock(SampleSyntax.Of<TransitionDefinitionBlockSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeDoNotInjectDeclarationSyntax
    [Test]
    public void TestCodeDoNotInjectDeclarationSyntax() {
		
        var tokenProps = typeof(CodeDoNotInjectDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                 .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeDoNotInjectDeclaration(SampleSyntax.Of<CodeDoNotInjectDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeAbstractMethodDeclarationSyntax
    [Test]
    public void TestCodeAbstractMethodDeclarationSyntax() {
		
        var tokenProps = typeof(CodeAbstractMethodDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeAbstractMethodDeclaration(SampleSyntax.Of<CodeAbstractMethodDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

    // CodeNotImplementedDeclarationSyntax
    [Test]
    public void TestCodeNotImplementedDeclarationSyntax() {
		
        var tokenProps = typeof(CodeNotImplementedDeclarationSyntax).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                    .Where(m => m.PropertyType ==typeof(SyntaxToken)).ToList();	
        if(!tokenProps.Any()) {
            return;
        }		
        var syntax =Syntax.ParseCodeNotImplementedDeclaration(SampleSyntax.Of<CodeNotImplementedDeclarationSyntax>());
        foreach (var prop in tokenProps) {    
            if(Attribute.IsDefined(prop, typeof(SuppressCodeSanityCheckAttribute))) {
                continue;
            }
            var tokenType = ((SyntaxToken)prop.GetValue(syntax, null)).Type;
            Assert.That(prop.Name, Is.EqualTo(tokenType.ToString()), "Der Name der Eigenschaft '{0}' sollte '{1}' lauten", prop.Name, tokenType);
        }
    }

}