#region Using Directives

using NUnit.Framework;
using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests {


    [TestFixture]
    public class SyntaxNodeTriviaTests {

        int _nlCharCount;
        [SetUp]
        public void Setup() {
            // Merkwürdigerweise haben beim Compile auf Appveyor die NLs nur eine Länge von 1...
            var nlTestString = @"
";
            _nlCharCount = nlTestString.Length;
        }

        // TODO Weitere Tests für Trivias
        [Test]
        public void GetLeadingTriviaExtentTests() {

            string source = 
@"//Foo
    task A;
    // Comment
    task B; //Comment
task C;
";
            var ndb = Syntax.ParseNodeDeclarationBlock(source);

            var taskA = ndb.NodeDeclarations[0];
            var lteA  = new TextExtent(0,            length: 5 + _nlCharCount + 4);
            var tteA  = new TextExtent(lteA.End + 7, length: _nlCharCount);
            Assert.That(taskA.GetLeadingTriviaExtent(), Is.EqualTo(lteA));
            Assert.That(taskA.GetTrailingTriviaExtent(), Is.EqualTo(tteA));

            var taskB = ndb.NodeDeclarations[1];
            var lteB  = new TextExtent(tteA.End,     length: 14 + _nlCharCount + 4);
            var tteB  = new TextExtent(lteB.End + 7, length: 10 + _nlCharCount);
            Assert.That(taskB.GetLeadingTriviaExtent(), Is.EqualTo(lteB));
            Assert.That(taskB.GetTrailingTriviaExtent(), Is.EqualTo(tteB));

            var taskC = ndb.NodeDeclarations[2];
            var lteC  = new TextExtent(tteB.End,     length: 0);
            var tteC  = new TextExtent(lteC.End+7,   length: _nlCharCount);
            Assert.That(taskC.GetLeadingTriviaExtent(),  Is.EqualTo(lteC));
            Assert.That(taskC.GetTrailingTriviaExtent(), Is.EqualTo(tteC)); 
        }

        [Test]
        public void GetLeadingTriviaExtentTests2() {

            string source =@" task A;    /* Comment*/    task B; /*Comment*/task C;
";
            var ndb = Syntax.ParseNodeDeclarationBlock(source);
            //
            var taskA = ndb.NodeDeclarations[0];
            var lteA  = new TextExtent(0, length: 1);
            var tteA  = new TextExtent(lteA.End + 7, length: 20);
            Assert.That(taskA.GetLeadingTriviaExtent(), Is.EqualTo(lteA));
            Assert.That(taskA.GetTrailingTriviaExtent(), Is.EqualTo(tteA)); 
            //
            var taskB = ndb.NodeDeclarations[1];
            //
            Assert.That(taskB.GetLeadingTriviaExtent(), Is.EqualTo(new TextExtent(28, length: 0)));
            Assert.That(taskB.GetTrailingTriviaExtent(), Is.EqualTo(new TextExtent(35, length: 12))); 
            //
            var taskC = ndb.NodeDeclarations[2];
            //
            Assert.That(taskC.GetLeadingTriviaExtent(), Is.EqualTo(new TextExtent(47, length: 0)));
            Assert.That(taskC.GetTrailingTriviaExtent(), Is.EqualTo(new TextExtent(54, length: _nlCharCount)));
        }

        [Test]
        public void GetLeadingTriviaExtentTestsOnlyWhitespace() {

            string source =
                @"//Foo
    task A;
    // Comment
    task B; //Comment
task C;
";
            var ndb = Syntax.ParseNodeDeclarationBlock(source);

            var taskA = ndb.NodeDeclarations[0];
            var lteA  = new TextExtent(5 + _nlCharCount, length: 2 + _nlCharCount);
            var tteA  = new TextExtent(lteA.End + 7,     length: _nlCharCount);
            Assert.That(taskA.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(lteA));
            Assert.That(taskA.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(tteA));

            var taskB = ndb.NodeDeclarations[1];
            var lteB  = new TextExtent(tteA.End + 14 + _nlCharCount, length: 4);
            var tteB  = new TextExtent(lteB.End + 7,                 length: 1);
            Assert.That(taskB.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(lteB));
            Assert.That(taskB.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(tteB)); 

            var taskC = ndb.NodeDeclarations[2];
            var lteC  = new TextExtent(tteB.End + 9 + _nlCharCount, length: 0);
            var tteC  = new TextExtent(lteC.End + 7,                length: _nlCharCount);
            Assert.That(taskC.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(lteC));
            Assert.That(taskC.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(tteC));
        }

        [Test]
        public void GetLeadingTriviaExtentTestsOnlyWhitespace2() {

            string source = @" task A;    /* Comment*/    task B; /*Comment*/task C;
";
            var ndb = Syntax.ParseNodeDeclarationBlock(source);
            //
            var taskA = ndb.NodeDeclarations[0];

            Assert.That(taskA.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(0, length: 1)));
            Assert.That(taskA.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(8, length: 4)));
            //
            var taskB = ndb.NodeDeclarations[1];
            //
            Assert.That(taskB.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(28, length: 0)));
            Assert.That(taskB.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(35, length: 1)));
            //
            var taskC = ndb.NodeDeclarations[2];
            //
            Assert.That(taskC.GetLeadingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(47, length: 0)));
            Assert.That(taskC.GetTrailingTriviaExtent(onlyWhiteSpace: true), Is.EqualTo(new TextExtent(54, length: _nlCharCount))); 
        }
    }
}
