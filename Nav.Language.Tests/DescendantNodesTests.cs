#region Using Directives

using System.Linq;

using NUnit.Framework;
using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests; 

[TestFixture]
public class DescendantNodesTests {

     const string NavContent = @"
            [namespaceprefix NamespacePrefix]
            [using My.Using.Namespace]

            taskref ""Dummy.nav"";

            task Test [base StandardWFS : IWFServiceBase]
                                [result TestResult]
            {
                init [params string message];
        
                view TestView;
    
                exit Ok;

                init --> TestView;  
    
                TestView --> Ok on Ok;
            }
        ";

    CodeGenerationUnitSyntax _codegenerationUnitSyntax;

    [SetUp]
    public void Setup() {
        _codegenerationUnitSyntax = Syntax.ParseCodeGenerationUnit(NavContent);
    }

    [Test]
    public void CountIdentifierSyntaxTest() {
        Assert.That(
            _codegenerationUnitSyntax.DescendantNodes<IdentifierSyntax>().Count(),
            Is.EqualTo(3) 
        );
    }

    [Test]
    public void CountIncludeDirectiveSyntaxTest() {
        Assert.That(
            _codegenerationUnitSyntax.DescendantNodes<IncludeDirectiveSyntax>().Count(),
            Is.EqualTo(1)
        );
    }

    [Test]
    public void CountTaskDefinitionSyntaxTest() {
        Assert.That(
            _codegenerationUnitSyntax.DescendantNodes<TaskDefinitionSyntax>().Count(),
            Is.EqualTo(1)
        );
    }

    [Test]
    public void CountSyntaxNodeTest1() {
        Assert.That(
            _codegenerationUnitSyntax.DescendantNodes<SyntaxNode>().Count(),
            Is.EqualTo(31)
        );
    }

    [Test]
    public void CountSyntaxNodeTest2() {
        Assert.That(
            _codegenerationUnitSyntax.DescendantNodes<SyntaxNode>().Count(),
            Is.EqualTo(_codegenerationUnitSyntax.DescendantNodes().Count())
        );
    }
}