#region Using Directives

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

using RoslynDiagnostic = Microsoft.CodeAnalysis.Diagnostic;
using RoslynSyntaxTree = Microsoft.CodeAnalysis.SyntaxTree;
using RoslynDiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;

using Diagnostic = Pharmatechnik.Nav.Language.Diagnostic;
using DiagnosticSeverity = Pharmatechnik.Nav.Language.DiagnosticSeverity;
using System.Threading;
using SyntaxTree = Pharmatechnik.Nav.Language.SyntaxTree;

#endregion

namespace Nav.Language.Tests {
    [TestFixture]
    public class CodeGenTests {

        [Test]
        public void SimpleCodegenTest() {

            var codeGenerationUnitSyntax= Syntax.ParseCodeGenerationUnit(Resources.TaskA, filePath: MkFilename("TaskA.nav"));
            var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax);

            var options       = GenerationOptions.Default;
            var codeGenerator = new CodeGenerator(options);

            var results = codeGenerator.Generate(codeGenerationUnit);

            Assert.That(results.Count, Is.EqualTo(1));

            var codeGenResult = results[0];

            Assert.That(codeGenResult.IBeginWfsCodeSpec.Content, Is.Not.Empty);
            Assert.That(codeGenResult.IWfsCodeSpec.Content     , Is.Not.Empty);
            Assert.That(codeGenResult.WfsBaseCodeSpec.Content  , Is.Not.Empty);
            Assert.That(codeGenResult.WfsCodeSpec.Content      , Is.Not.Empty);
        }

        public static TestCaseData[] CompileTestCases = {

            new TestCaseData(
                new TestCase {
                    NavFiles = {
                        new TestCaseFile {FilePath = MkFilename("TaskA.nav"), Content = Resources.TaskA}
                    }
                }
            ) {
                TestName = "TaskA should be compilable"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskA)}.nav"), Content = Resources.TaskB}
                }
            }){
                TestName = "TaskB should be compilable"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskA)}.nav"), Content = Resources.TaskA},
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskB)}.nav"), Content = Resources.TaskB}
                }
            }){
                TestName = "TaskA and TaskB should be compilable at the same time"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.SingleFileNav)}.nav"), Content = Resources.SingleFileNav},
                }
            }){
                TestName = "TestNavGeneratorOnSingleFile"
            }
            ,
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskA)}.nav"), Content = Resources.TaskA},
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskB)}.nav"), Content = Resources.TaskB},
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.TaskC)}.nav"), Content = Resources.TaskC},
                }
            }){
                TestName = "Task C depends on Task A and Task B"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {FilePath = MkFilename($"{nameof(Resources.NestedChoices)}.nav"), Content = Resources.NestedChoices},
                }
            }){
                TestName = "Nested choices"
            },
            new TestCaseData(new TestCase {
                NavFiles = {
                    new TestCaseFile {
                        FilePath = MkFilename("TaskA.nav"),
                        Content = @"                           
                            task A [result string] {
                                init i;
                                exit e;
                                i--> e;
                            }"
                    }
                }
            }){
                TestName = "Tasksresult without explizit name"
            }
        };

        [Test, TestCaseSource(nameof(CompileTestCases))]
        public void CompileTest(TestCase testCase) {

            var syntaxProvider = new TestSyntaxProvider();
            // Dateien bekanntgeben - wir haben hier keine echten Dateien zur Hand!
            foreach(var navFile in testCase.NavFiles) {
                syntaxProvider.RegisterFile(navFile);
            }

            var syntaxTrees = new List<RoslynSyntaxTree>();
            foreach (var navFile in testCase.NavFiles) {

                // 1. Syntaxbaum aus Nav-File erstellen
                var codeGenerationUnitSyntax = syntaxProvider.FromFile(navFile.FilePath)?.GetRoot() as CodeGenerationUnitSyntax;
                Assert.That(codeGenerationUnitSyntax, Is.Not.Null, $"File '{navFile.FilePath}' not found");
                AssertNoDiagnosticErrors(codeGenerationUnitSyntax.SyntaxTree.Diagnostics, codeGenerationUnitSyntax.SyntaxTree.SourceText);

                // 2. Semantic Model erstellen aus Syntax erstellen
                var codeGenerationUnit = CodeGenerationUnit.FromCodeGenerationUnitSyntax(codeGenerationUnitSyntax, syntaxProvider: syntaxProvider);
                AssertNoDiagnosticErrors(codeGenerationUnit.Diagnostics, codeGenerationUnitSyntax.SyntaxTree.SourceText);

                var options = GenerationOptions.Default;
                var codeGenerator = new CodeGenerator(options);
                
                // 3. Code aus Semantic Model erstellen
                var codeGenerationResults = codeGenerator.Generate(codeGenerationUnit);
                
                foreach (var codeGenerationResult in codeGenerationResults) {
     
                    // 4. C#-Syntaxbäume des generierten Codes mittels Roslyn erstellen
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenerationResult.IBeginWfsCodeSpec.Content, path: codeGenerationResult.IBeginWfsCodeSpec.FilePath));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenerationResult.IWfsCodeSpec.Content     , path: codeGenerationResult.IWfsCodeSpec.FilePath));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenerationResult.WfsBaseCodeSpec.Content  , path: codeGenerationResult.WfsBaseCodeSpec.FilePath));
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeGenerationResult.WfsCodeSpec.Content      , path: codeGenerationResult.WfsCodeSpec.FilePath));
                    foreach(var toCodeSpec in codeGenerationResult.ToCodeSpecs) {
                        syntaxTrees.Add(CSharpSyntaxTree.ParseText(toCodeSpec.Content, path: toCodeSpec.FilePath));
                    }
                }                
            }
            // Pseudo Framework Code hinzufügen
            syntaxTrees.Add(GetFrameworkStubCode());

            foreach (var syntaxTree in syntaxTrees) {
                AssertNoDiagnosticErrors(syntaxTree.GetDiagnostics());
            }

            // 6. C# Compilation In-Memmory erstellen
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
            Assert.That(errors.Any(), Is.False, FormatDiagnostics(errors) + errors.FirstOrDefault()?.Location.SourceTree);
        }

        string FormatDiagnostics(IEnumerable<RoslynDiagnostic> diagnostics) {
            return diagnostics.Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine(FormatDiagnostic(d)), sb => sb.ToString());
        }

        static string FormatDiagnostic(RoslynDiagnostic diagnostic) {
            return $"{diagnostic.Id}: {diagnostic.Location} {diagnostic.GetMessage()}";
        }

        void AssertNoDiagnosticErrors(IEnumerable<Diagnostic> diagnostics, string sourceText) {
            var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.That(errors.Any(), Is.False, FormatDiagnostics(errors) + sourceText);
        }

        string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics) {
            return diagnostics.Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine(FormatDiagnostic(d)), sb => sb.ToString());
        }

        static string FormatDiagnostic(Diagnostic diagnostic) {
            return $"{diagnostic.Descriptor.Id}: {diagnostic.Location} {diagnostic.Message}";
        }

        static string MkFilename(string fileName) {
            return Path.Combine(@"n:\av", fileName);
        }

        public class TestCaseFile {
            public string Content { get; set; }
            public string FilePath { get; set; }
        }

        public class TestCase {
            public List<TestCaseFile> NavFiles { get; } = new List<TestCaseFile>();
        }

        class TestSyntaxProvider : SyntaxProvider {

            readonly Dictionary<string, string> _files;

            public TestSyntaxProvider() {
                _files = new Dictionary<string, string>();
            }

            public void RegisterFile(TestCaseFile file) {
                _files[file.FilePath] = file.Content;
            }

            public override SyntaxTree FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken)) {

                if (!_files.TryGetValue(filePath, out var content)) {
                    return null;
                }

                return SyntaxTree.ParseText(text: content, filePath: filePath);
            }
        }
    }  
}
