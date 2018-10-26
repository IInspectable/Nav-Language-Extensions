#region Using Directives

using NUnit.Framework;
using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class PathProviderTests {

        [Test]
        public void TestGeneratedFolderName() {
            Assert.That(PathProvider.GeneratedFolderName, Is.EqualTo("generated"), "Wrong GeneratedFolderName");
        }
       
        [Test]
        public void TestGeneratedFileNameSuffix() {
            Assert.That(PathProvider.GeneratedFileNameSuffix, Is.EqualTo("generated"), "Wrong GeneratedFileNameSuffix");
        }

        [Test]
        public void TestCSharpFileExtension() {
            Assert.That(PathProvider.CSharpFileExtension, Is.EqualTo("cs"), "Wrong CSharpFileExtension");
        }

        [Test]
        public void TestGeneratedPaths() {

            var taskName = "Test";
            var syntaxFileName = @"n:\av\tets.nav";

            // ReSharper disable once RedundantArgumentDefaultValue
            var pathProvider = new PathProvider(syntaxFileName: syntaxFileName, taskName: taskName, generateTo: null);

            Assert.That(pathProvider.TaskName              , Is.EqualTo(taskName));
            Assert.That(pathProvider.SyntaxFileName        , Is.EqualTo(syntaxFileName));

            Assert.That(pathProvider.WflDirectory          , Is.EqualTo(@"n:\av\WFL"));            
            Assert.That(pathProvider.WfsFileName           , Is.EqualTo(@"n:\av\WFL\TestWFS.cs"));

            Assert.That(pathProvider.WflGeneratedDirectory , Is.EqualTo(@"n:\av\WFL\generated"));
            Assert.That(pathProvider.WfsBaseFileName       , Is.EqualTo(@"n:\av\WFL\generated\TestWFSBase.generated.cs"));            
            Assert.That(pathProvider.IBeginWfsFileName     , Is.EqualTo(@"n:\av\WFL\generated\IBeginTestWFS.generated.cs"));
            
            Assert.That(pathProvider.IwflGeneratedDirectory, Is.EqualTo(@"n:\av\IWFL\generated"));
            Assert.That(pathProvider.IWfsFileName          , Is.EqualTo(@"n:\av\IWFL\generated\ITestWFS.generated.cs"));

            Assert.That(pathProvider.GetToFileName("MyTo") , Is.EqualTo(@"n:\av\IWFL\generated\MyTo.generated.cs"));
        }

        [Test]
        public void TestGeneratedPathsWithTo() {

            var taskName = "Test";
            var syntaxFileName = @"n:\av\tets.nav";
            var generateTo = "generateTo";

            var pathProvider = new PathProvider(syntaxFileName: syntaxFileName, taskName: taskName, generateTo: generateTo);

            Assert.That(pathProvider.TaskName              , Is.EqualTo(taskName));
            Assert.That(pathProvider.SyntaxFileName        , Is.EqualTo(syntaxFileName));

            Assert.That(pathProvider.WflDirectory          , Is.EqualTo(@"n:\av\WFL\generateTo"));
            Assert.That(pathProvider.WfsFileName           , Is.EqualTo(@"n:\av\WFL\generateTo\TestWFS.cs"));

            Assert.That(pathProvider.WflGeneratedDirectory , Is.EqualTo(@"n:\av\WFL\generateTo\generated"));
            Assert.That(pathProvider.WfsBaseFileName       , Is.EqualTo(@"n:\av\WFL\generateTo\generated\TestWFSBase.generated.cs"));
            Assert.That(pathProvider.IBeginWfsFileName     , Is.EqualTo(@"n:\av\WFL\generateTo\generated\IBeginTestWFS.generated.cs"));

            Assert.That(pathProvider.IwflGeneratedDirectory, Is.EqualTo(@"n:\av\IWFL\generateTo\generated"));
            Assert.That(pathProvider.IWfsFileName          , Is.EqualTo(@"n:\av\IWFL\generateTo\generated\ITestWFS.generated.cs"));

        }
    }
}