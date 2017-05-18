#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.CodeGen.Templates;

using NUnit.Framework;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using SyntaxTree = Microsoft.CodeAnalysis.SyntaxTree;

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

        [Test]
        public void SimpleCodegenCompileTest() {

            string navCode = @"
[namespaceprefix Test]

[using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL]

task TaskA [code ""public enum MessageBoxResult{ Ok, Abbrechen}""]
           [base StandardWFS : IWFService]
                  [result MessageBoxResult]
{
    init I1 [params string message];
        
    view TestView;
    
    exit Ok;

    I1    --> TestView;  
    
    TestView --> Ok on Ok;
    TestView --> Ok on OnFoo;
}
";
            var codeGenerationUnitSyntax = Syntax.ParseCodeGenerationUnit(navCode, @"c:\UnitTests\TaskA.nav");
            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax);

            var options = GenerationOptions.Default;
            var modelGenerator = new CodeModelGenerator(options);
            var codeGenerator = new CodeGenerator(options);

            var results = modelGenerator.Generate(codeGenerationUnit);

            Assert.That(results.Count, Is.EqualTo(1));

            var codeGenResult = codeGenerator.Generate(results[0]);
            
            var beginWfsSyntaxTree  = CSharpSyntaxTree.ParseText(codeGenResult.IBeginWfsCode, path: codeGenResult.PathProvider.IBeginWfsFileName);
            var iWfsCodeSyntaxTree  = CSharpSyntaxTree.ParseText(codeGenResult.IWfsCode     , path: codeGenResult.PathProvider.IWfsFileName);
            var wfsBaseSyntaxTree   = CSharpSyntaxTree.ParseText(codeGenResult.WfsBaseCode  , path: codeGenResult.PathProvider.WfsBaseFileName);
            var wfsSyntaxTree       = CSharpSyntaxTree.ParseText(codeGenResult.WfsCode      , path: codeGenResult.PathProvider.WfsFileName);
            var frameworkSyntaxTree = GetFrameworkStubCode();

            var syntaxTrees = new[] {
                beginWfsSyntaxTree,
                iWfsCodeSyntaxTree,
                wfsBaseSyntaxTree,
                wfsSyntaxTree,
                frameworkSyntaxTree
            };

            AssertDiagnosticErrors(frameworkSyntaxTree.GetDiagnostics());
            AssertDiagnosticErrors(beginWfsSyntaxTree.GetDiagnostics());
            AssertDiagnosticErrors(iWfsCodeSyntaxTree.GetDiagnostics());
            AssertDiagnosticErrors(wfsBaseSyntaxTree.GetDiagnostics());
            AssertDiagnosticErrors(wfsSyntaxTree.GetDiagnostics());
            
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream()) {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success) {

                    AssertDiagnosticErrors(result.Diagnostics);
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures) {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
               }
        }

        // TODO "Framework Code" checken
        SyntaxTree GetFrameworkStubCode() {
            return CSharpSyntaxTree.ParseText(@"

namespace Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL
{
    public interface IWFService { }

    public class TestViewTO { }

    public interface INavCommand { }
    public interface IClientSideWFS { }
}

namespace Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL {

    public interface INavCommandBody {

    }

    public interface IBeginWFService {

    }

    public class IINIT_TASK {

    }

    public class StandardWFS {

        protected INavCommandBody InternalTaskResult(object o) {
            return null;
        }

    }
}

namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.WFL {

}

namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL {

}
                ", path: @"c:\UnitTests\FrameworkStubCode.cs");
        }

        void AssertDiagnosticErrors(IEnumerable<Diagnostic> diagnostics) {
            var errors = diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.That(errors.Any(), Is.False, errors.Aggregate(
                "", 
                (s, d)=> s+= $"{d.Id}: {d.Location} {d.GetMessage()}\r\n"));
        }
    }  
}
