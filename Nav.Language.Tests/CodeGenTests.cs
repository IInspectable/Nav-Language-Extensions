﻿#region Using Directives

using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeGen;

using NUnit.Framework;

using RoslynDiagnostic         = Microsoft.CodeAnalysis.Diagnostic;
using RoslynSyntaxTree         = Microsoft.CodeAnalysis.SyntaxTree;
using RoslynDiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;

using Diagnostic               = Pharmatechnik.Nav.Language.Diagnostic;
using DiagnosticSeverity       = Pharmatechnik.Nav.Language.DiagnosticSeverity;

#endregion

namespace Nav.Language.Tests {
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

        public static TestCaseData[] CompileTestCases = {

            new TestCaseData(
                new TestCase {
                    NavFiles = {
                        new TestCaseFile {FilePath = MkFilename("TaskA.nav"), Content = Resources.TaskANav}
                    }
                }
            ) {
                TestName = "TaskA should be compilable"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename("TaskB.nav"), Content = Resources.TaskBNav}
                }
            }){
                TestName = "TaskB should be compilable"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename("TaskA.nav"), Content = Resources.TaskANav},
                    new TestCaseFile {FilePath = MkFilename("TaskB.nav"), Content = Resources.TaskBNav}
                }
            }){
                TestName = "TaskA and TaskB should be compilable at the same time"
            },
        };

        [Test, TestCaseSource(nameof(CompileTestCases))]
        public void SimpleCodegenCompileTest(TestCase testCase) {

            var syntaxTrees = new List<RoslynSyntaxTree>();

            foreach (var file in testCase.NavFiles) {

                var codeGenerationUnitSyntax = Syntax.ParseCodeGenerationUnit(file.Content, filePath: file.FilePath);
                AssertNoDiagnosticErrors(codeGenerationUnitSyntax.SyntaxTree.Diagnostics);

                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax);
                AssertNoDiagnosticErrors(codeGenerationUnit.Diagnostics);

                var options = GenerationOptions.Default;
                var modelGenerator = new CodeModelGenerator(options);
                var codeGenerator = new CodeGenerator(options);

                var results = modelGenerator.Generate(codeGenerationUnit);


                foreach (var codeModelResult in results) {

                    var codeGenResult = codeGenerator.Generate(codeModelResult);

                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenResult.IBeginWfsCode, path: codeGenResult.PathProvider.IBeginWfsFileName));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenResult.IWfsCode     , path: codeGenResult.PathProvider.IWfsFileName));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenResult.WfsBaseCode  , path: codeGenResult.PathProvider.WfsBaseFileName));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenResult.WfsCode      , path: codeGenResult.PathProvider.WfsFileName));
                }                
            }

            syntaxTrees.Add(GetFrameworkStubCode());

            foreach (var syntaxTree in syntaxTrees) {
                AssertNoDiagnosticErrors(syntaxTree.GetDiagnostics());
            }     
            
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references : references,
                options    : new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream()) {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success) {
                    AssertNoDiagnosticErrors(result.Diagnostics);
                }
            }
        }
        
        RoslynSyntaxTree GetFrameworkStubCode() {
            return CSharpSyntaxTree.ParseText(Resources.FrameworkStubsCode, path: MkFilename("FrameworkStubCode.cs"));
        }

        void AssertNoDiagnosticErrors(IEnumerable<RoslynDiagnostic> diagnostics) {
            var errors = diagnostics.Where(d => d.IsWarningAsError || d.Severity == RoslynDiagnosticSeverity.Error).ToList();
            Assert.That(errors.Any(), Is.False, FormatDiagnostics(errors));
        }

        string FormatDiagnostics(IEnumerable<RoslynDiagnostic> diagnostics) {
            return diagnostics.Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine(FormatDiagnostic(d)), sb => sb.ToString());
        }

        static string FormatDiagnostic(RoslynDiagnostic diagnostic) {
            return $"{diagnostic.Id}: {diagnostic.Location} {diagnostic.GetMessage()}";
        }

        void AssertNoDiagnosticErrors(IEnumerable<Diagnostic> diagnostics) {
            var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.That(errors.Any(), Is.False, FormatDiagnostics(errors));
        }

        string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics) {
            return diagnostics.Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine(FormatDiagnostic(d)), sb => sb.ToString());
        }

        static string FormatDiagnostic(Diagnostic diagnostic) {
            return $"{diagnostic.Descriptor.Id}: {diagnostic.Location} {diagnostic.Message}";
        }

        static string MkFilename(string fileName) {
            return Path.Combine(@"c:\UnitTest", fileName);
        }

        public class TestCaseFile {
            public string Content { get; set; }
            public string FilePath { get; set; }
        }
        public class TestCase {
            public List<TestCaseFile> NavFiles { get; } = new List<TestCaseFile>();
        }
    }  
}
