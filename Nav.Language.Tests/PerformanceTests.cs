#region Using Directives

using System.Linq;

using NUnit.Framework;
using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests; 

[TestFixture]
public class PerformanceTests {

    [Ignore("Too slow on CI build"), Test, MaxTime(200)]
    public void TestPerformance() {

        SyntaxTree.ParseText(Resources.LargeNav);

        var syntaxTree = SyntaxTree.ParseText(Resources.LargeNav);

        var lastToken = syntaxTree.Tokens.Last();

        Assert.That(lastToken.End, Is.EqualTo(Resources.LargeNav.Length));
    }
}