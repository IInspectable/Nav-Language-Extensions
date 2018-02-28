#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests.Diagnostics {

    [TestFixture]
    public class DiagnosticTests {

        [Test, TestCaseSource(nameof(GetTestCases))]
        public void TestDiagnostics(FileTestCase testCase) {

            string source = testCase.SourceText();
            var    unit   = BuildCodeGenerationUnit(source);

            var expected = ParseDiagnostics(source);
            var actual   = ToUnitTestString(unit.Diagnostics);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        public static IEnumerable<FileTestCase> GetTestCases() {

            return CollectNavFiles().Select(navFile => new FileTestCase {
                FilePath = navFile,
            });
        }

        static IEnumerable<string> CollectNavFiles() {
            var directory = GetDiagnosticTestDirectory();
            var navFiles  = Directory.EnumerateFiles(directory, "*.nav", SearchOption.AllDirectories);

            return navFiles;
        }

        static string GetDiagnosticTestDirectory() {

            return Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\Diagnostics\Tests"));
        }

        IEnumerable<string> ParseDiagnostics(string source) {

            return source.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)
                         .Where(l => l.StartsWith(UnitTestDiagnosticFormatter.LinePrefix))
                         .Select(s => s.TrimEnd());
        }

        IEnumerable<string> ToUnitTestString(IEnumerable<Diagnostic> diagnostics) {
            return diagnostics.Select(diagnostic => diagnostic.ToString(UnitTestDiagnosticFormatter.Instance));
        }

        CodeGenerationUnit BuildCodeGenerationUnit(string source, string filePath = null) {
            var syntax = Syntax.ParseCodeGenerationUnit(source, filePath);
            var model  = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax);
            return model;
        }

        public class FileTestCase {

            public string FilePath { get; set; }
            public string SourceText() => File.ReadAllText(FilePath);

            public override string ToString() {
                if(FilePath == null) {
                    return "Unknown";
                }

                return Path.GetFileNameWithoutExtension(FilePath);
            }

        }

    }

}