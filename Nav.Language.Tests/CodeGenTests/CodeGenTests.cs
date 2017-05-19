#region Using Directives

using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeGen;

using NUnit.Framework;

using Diagnostic         = Microsoft.CodeAnalysis.Diagnostic;
using SyntaxTree         = Microsoft.CodeAnalysis.SyntaxTree;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;

#endregion

namespace Nav.Language.Tests.CodeGenTests {
    [TestFixture]
    public class CodeGenTests {

        [Test]
        public void SimpleCodegenTest() {

            var codeGenerationUnitSyntax= Syntax.ParseCodeGenerationUnit(Resources.TaskANav, filePath: MkFilename("TaskA.nav"));
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

            string navCode = Resources.TaskANav;
            var codeGenerationUnitSyntax = Syntax.ParseCodeGenerationUnit(navCode, filePath: MkFilename("TaskA.nav"));
            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax);

            var options = GenerationOptions.Default;
            var modelGenerator = new CodeModelGenerator(options);
            var codeGenerator = new CodeGenerator(options);

            var results = modelGenerator.Generate(codeGenerationUnit);

            Assert.That(results.Count, Is.EqualTo(1));

            var codeGenResult = codeGenerator.Generate(results[0]);

            var syntaxTrees = new[] {
                CSharpSyntaxTree.ParseText(codeGenResult.IBeginWfsCode, path: codeGenResult.PathProvider.IBeginWfsFileName),
                CSharpSyntaxTree.ParseText(codeGenResult.IWfsCode     , path: codeGenResult.PathProvider.IWfsFileName),
                CSharpSyntaxTree.ParseText(codeGenResult.WfsBaseCode  , path: codeGenResult.PathProvider.WfsBaseFileName),
                CSharpSyntaxTree.ParseText(codeGenResult.WfsCode      , path: codeGenResult.PathProvider.WfsFileName),
                GetFrameworkStubCode()
            };

            foreach (var syntaxTree in syntaxTrees) {
                AssertDiagnosticErrors(syntaxTree.GetDiagnostics());
            }     
            
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
                }
            }
        }
        
        SyntaxTree GetFrameworkStubCode() {
            return CSharpSyntaxTree.ParseText(Resources.FrameworkStubsCode, path: MkFilename("FrameworkStubCode.cs"));
        }

        void AssertDiagnosticErrors(IEnumerable<Diagnostic> diagnostics) {
            var errors = diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.That(errors.Any(), Is.False, errors.Aggregate(
                "", 
                (s, d)=> s+= $"{d.Id}: {d.Location} {d.GetMessage()}\r\n"));
        }

        static string MkFilename(string fileName) {
            return Path.Combine(@"c:\UnitTest", fileName);
        }
    }  
}
