using NUnit.Framework;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class SymbolVisitorTests {

        [Test]
        public void SymbolVisitorOfTVisitNodeReferenceSymbolFallBack() {

            var visitor = new SymbolVisitorOfBool();

            Assert.That(visitor.VisitChoiceNodeSymbol(null), Is.False);

            // Fallback auf VisitNodeReferenceSymbol
            Assert.That(visitor.VisitInitNodeReferenceSymbol(null),   Is.True);
            Assert.That(visitor.VisitChoiceNodeReferenceSymbol(null), Is.True);
            Assert.That(visitor.VisitGuiNodeReferenceSymbol(null),    Is.True);
            Assert.That(visitor.VisitTaskNodeReferenceSymbol(null),   Is.True);
        }

        class SymbolVisitorOfBool: SymbolVisitor<bool> {

            protected override bool DefaultVisit(ISymbol symbol) {
                return false;
            }

            public override bool VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                return true;
            }

        }

        [Test]
        public void SymbolVisitoVisitNodeReferenceSymbolFallBack() {

            var visitor = new TestSymbolVisitor();
            visitor.VisitChoiceNodeSymbol(null);
            Assert.That(visitor.VisitNodeReferenceSymbolCalled, Is.False);

            // Fallback auf VisitNodeReferenceSymbol

            visitor = new TestSymbolVisitor();
            visitor.VisitInitNodeReferenceSymbol(null);
            Assert.That(visitor.VisitNodeReferenceSymbolCalled, Is.True);

            visitor = new TestSymbolVisitor();
            visitor.VisitChoiceNodeReferenceSymbol(null);
            Assert.That(visitor.VisitNodeReferenceSymbolCalled, Is.True);

            visitor = new TestSymbolVisitor();
            visitor.VisitGuiNodeReferenceSymbol(null);
            Assert.That(visitor.VisitNodeReferenceSymbolCalled, Is.True);

            visitor = new TestSymbolVisitor();
            visitor.VisitTaskNodeReferenceSymbol(null);
            Assert.That(visitor.VisitNodeReferenceSymbolCalled, Is.True);

        }

        class TestSymbolVisitor: SymbolVisitor {

            public bool VisitNodeReferenceSymbolCalled { get; private set; }

            public override void VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                VisitNodeReferenceSymbolCalled = true;
            }

        }

    }

}