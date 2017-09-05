using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Pharmatechnik.Nav.Language;
// ReSharper disable PossibleNullReferenceException

namespace Nav.Language.Tests {

    [TestFixture]
    public class SyntaxTreeNavigationTests {

        [Test]
        public void TestTokenParent() {
            var syntaxTree = SyntaxTree.ParseText("task Test { init I;}");
                    
            Assert.That(syntaxTree.Diagnostics.Count, Is.EqualTo(0));

            var task = syntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>().First();

            Assert.That(task.TaskKeyword, Is.Not.Null);
            Assert.That(task.TaskKeyword.Parent, Is.EqualTo(task));

            var initNode = task.NodeDeclarationBlock.InitNodes().First();

            Assert.That(initNode.InitKeyword.Parent,          Is.EqualTo(initNode));
            Assert.That(initNode.Parent,                  Is.EqualTo(task.NodeDeclarationBlock));
            Assert.That(task.NodeDeclarationBlock.Parent, Is.EqualTo(task));
            Assert.That(task.Parent,                      Is.EqualTo(syntaxTree.GetRoot()));
            Assert.That(syntaxTree.GetRoot().Parent,           Is.Null);
        }

        [Test]
        public void TestRandomText() {
            
            string s = TestHelper.RandomString(400000);

            var sw = new Stopwatch();
            sw.Start();
            var syntaxTree = SyntaxTree.ParseText(s);
            var duration = sw.Elapsed;

            var lastToken = syntaxTree.Tokens.Last();
            Assert.That(lastToken.End, Is.EqualTo(s.Length));

            Console.WriteLine(duration.TotalMilliseconds);
        }

        [Test]
        public void TestTokenGaps() {

            string s = Resources.LargeNav;

            var syntaxTree = SyntaxTree.ParseText(s);

            int pos = 0;
            foreach (var token in syntaxTree.Tokens) {
                Assert.That(token.Start, Is.EqualTo(pos));
                pos = token.End;
            }

            Assert.That(pos, Is.EqualTo(s.Length));          
        }

        [Test]
        public void TestTokenGapsWithError() {

            string s = Resources.NavWithError;

            var syntaxTree = SyntaxTree.ParseText(s);

            int pos = 0;
            foreach (var token in syntaxTree.Tokens) {
                Assert.That(token.Start, Is.EqualTo(pos));
                pos = token.End;
            }

            Assert.That(pos, Is.EqualTo(s.Length));
        }

        [Test]
        public void TestTokenWithErrorAtStart() {

            string nav = @"
                    [
                    task T1	
                    {
                    	init I;
                    	exit E;
                    }";

            var syntaxTree = SyntaxTree.ParseText(nav);
            Assert.That(syntaxTree.Diagnostics.Any(), Is.True);

            Assert.That(syntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>().First().Identifier.ToString(), Is.EqualTo("T1"));
        }

        [Test]
        public void TestTokenWithErrorAtEnd() {

            string nav = @"
                    task T1	
                    {
                    	init I;
                    	exit E;
                    }
                    
                    task {";

            var syntaxTree = SyntaxTree.ParseText(nav);
            Assert.That(syntaxTree.Diagnostics.Any(), Is.True);
            Assert.That(syntaxTree.GetRoot().DescendantNodes().OfType<TaskDefinitionSyntax>().First().Identifier.ToString(), Is.EqualTo("T1"));
        }

        [Test]
        [Ignore("Enablen sobald Trivias unterstützt werden.")]
        public void TestCommentToken() {
            var syntaxTree = SyntaxTree.ParseText("task /* Kommentar*/ \r\nTest { init I;}");

            Assert.That(syntaxTree.Diagnostics.Count, Is.EqualTo(0));
            Assert.That(syntaxTree.Tokens.Count(t=>t.Parent== null), Is.EqualTo(0));

            var task = syntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>().First();

            Assert.That(task.ChildTokens().Count(), Is.EqualTo(4));

            var comments = task.ChildTokens().OfClassification(SyntaxTokenClassification.Comment).ToList();
            Assert.That(comments.Count, Is.EqualTo(1));
            Assert.That(comments[0].ToString(), Is.EqualTo("/* Kommentar*/"));
            Assert.That(comments[0].Classification, Is.EqualTo(SyntaxTokenClassification.Comment));
        }

        [Test]
        public void TestPrevNextToken() {
            var syntaxTree = SyntaxTree.ParseText("task Test {}");

            Assert.That(syntaxTree.Diagnostics.Count, Is.EqualTo(0));

            var taskKeywordToken = syntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>().First().TaskKeyword;

            Assert.That(taskKeywordToken.Classification, Is.EqualTo(SyntaxTokenClassification.Keyword));

            var nameToken = taskKeywordToken.NextToken(SyntaxTokenClassification.TaskName);
            Assert.That(nameToken.Classification, Is.EqualTo(SyntaxTokenClassification.TaskName));
            Assert.That(nameToken.ToString(), Is.EqualTo("Test"));

            var prevToken = nameToken.PreviousToken(SyntaxTokenClassification.Keyword);
            Assert.That(prevToken.Classification, Is.EqualTo(SyntaxTokenClassification.Keyword));
            Assert.That(prevToken.ToString(), Is.EqualTo("task"));
        }
    }
}