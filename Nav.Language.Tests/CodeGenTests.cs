#region Using Directives

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeGen.Templates;

using NUnit.Framework;

#endregion

namespace Nav.Language.Tests {
    [TestFixture]
    public class CodeGenTests {

        [Test]
        public void TestResources() {
            var n = Resources.IBeginWfsTemplate;
            Assert.That(n, Is.Not.Empty);
        }

        [Test]
        public void SimpleCodegenTest() {

            string navCode = @"[namespaceprefix NS]

[using Pharmatechnik.Apotheke.XTplus.Framework.Core.WFL]
[using Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL] 


task TaskA [base StandardWFS : ILegacyMessageBoxWFS]
                  [result MessageBoxResult]
{
    init I1 [params string message];
    init I2 [params string message, MessageBoxImage messageBoxImage];
        
    view MessageBoxOK;
    
    exit Ok;

    I1 --> MessageBoxOK;  
    I2 --> MessageBoxOK; 
    
    MessageBoxOK --> Ok on Ok;
    MessageBoxOK --> Ok on OnFoo;
}";
            var codeGenerationUnitSyntax= Syntax.ParseCodeGenerationUnit(navCode, @"c:\TaskA.nav");
            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax);

            var options        = GenerationOptions.Default;
            var modelGenerator = new CodeModelGenerator(options);
            var codeGenerator  = new CodeGenerator(options);

            var results = modelGenerator.Generate(codeGenerationUnit);

            Assert.That(results.Count, Is.EqualTo(1));

            var codeGenResult = codeGenerator.Generate(results[0]);

            Assert.That(codeGenResult.IBeginWfsCode, Is.Not.Empty);
            Assert.That(codeGenResult.IWfsCode     , Is.Not.Empty);
            Assert.That(codeGenResult.WfsBaseCode  , Is.Not.Empty);
            Assert.That(codeGenResult.WfsCode      , Is.Not.Empty);
        }
    }
}
