#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Generator;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Nav.Language.Tests.Regression {

    [TestFixture, NonParallelizable]
    public class RegressionTests {

        [OneTimeSetUp]
        public void Setup() {

            GenerateFiles();
        }

        [Test]
        public void Healthy() {
            Assert.That(true, Is.True);
        }

        [Test, Explicit]
        public void DiscoverNavFiles() {

            var navs = CollectNavFiles().Aggregate(String.Empty, (s, f) => s += f.FilePath + Environment.NewLine);
            Assert.That(false, Is.True, navs);
        }

        [Test, Explicit]
        public void DiscoverExpectedFiles() {
            var cases = GetFileTestCases().Aggregate(String.Empty, (s, c) => s += c.RelativeExpectedFile + Environment.NewLine);
            Assert.That(false, Is.True, cases);
        }

        [Test, Explicit]
        public void GenerateFiles() {

            // Sicherstellen, dass auch wirklich alle Files (auch die "OneShots") neu geschrieben werden.
            foreach (var tc in GetFileTestCases()) {
                File.Delete(tc.GeneratedFile);
            }

            var fileSpecs = CollectNavFiles();

            var pipeline = NavCodeGeneratorPipeline.CreateDefault();
            var b        = pipeline.Run(fileSpecs);

            // TODO Bessere Fehlerbehandlung/Ausgabe
            Assert.That(b, Is.True);
        }

        [Test, TestCaseSource(nameof(GetFileTestCases))]
        public void CompareFile(FileTestCase pair) {

            var generatedContent = File.ReadAllText(pair.GeneratedFile);
            var expectedContent  = File.ReadAllText(pair.ExpectedFile);

            Assert.That(generatedContent, Is.EqualTo(expectedContent), $"File '{pair.GeneratedFile}' differes from expected file content '{pair.ExpectedFile}'");
        }

        static IEnumerable<FileSpec> CollectNavFiles() {
            var directory    = GetRegressiontestDirectory();
            var navFiles     = Directory.EnumerateFiles(directory, "*.nav", SearchOption.AllDirectories);
            var dirFileSpecs = navFiles.Select(file => new FileSpec(identity: PathHelper.GetRelativePath(directory, file), fileName: file));

            return dirFileSpecs;
        }

        static IEnumerable<FileTestCase> GetFileTestCases() {

            var directory = GetRegressiontestDirectory();

            var files = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories).Where(f => !f.EndsWith("expected.cs"));

            return files.Select(f => new FileTestCase {
                    GeneratedFile = Path.GetFullPath(f),
                    ExpectedFile  = Path.GetFullPath(Path.ChangeExtension(f, "expected.cs"))
                }
            );

        }

        static string GetRegressiontestDirectory() {

            return Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\Regression\Tests"));
        }

        public class FileTestCase {

            public string GeneratedFile { get; set; }
            public string ExpectedFile  { get; set; }

            public string RelativeGeneratedFile => PathHelper.GetRelativePath(GetRegressiontestDirectory(), GeneratedFile);
            public string RelativeExpectedFile  => PathHelper.GetRelativePath(GetRegressiontestDirectory(), ExpectedFile);

            public override string ToString() {
                return $"{RelativeGeneratedFile} <-?-> {RelativeExpectedFile}";
            }

        }

    }

}