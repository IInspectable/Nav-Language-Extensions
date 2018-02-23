using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Generator;
using Pharmatechnik.Nav.Utilities.IO;

namespace Nav.Language.Tests.Regression {

    [TestFixture]
    public class RegressionTests {

        [OneTimeSetUp]
        public void Setup() {

            var fileSpecs = CollectNavFiles();

            var pipeline = NavCodeGeneratorPipeline.CreateDefault();
            var b        = pipeline.Run(fileSpecs);

            // TODO Bessere Fehlerbehandlung/Ausgabe
            Assert.That(b, Is.True);
        }

        [Test]
        public void TestDirectory() {
            Assert.That(TestContext.CurrentContext.TestDirectory, Is.EqualTo("Fail"));
        }

        [Test, Explicit]
        public void GenerateFiles() {
            // Generiert jediglich alle Files(=> Setup)
            Assert.That(true);
        }

        [Test, TestCaseSource(nameof(CollectFiles))]
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

        static IEnumerable<FileTestCase> CollectFiles() {

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