using System.Collections.Generic;
using NUnit.Framework;
using Pharmatechnik.Nav.Language;
using System.Linq;

namespace Nav.Language.Tests {
    [TestFixture]
    public class SemanticErrorTests {

        [Test]
        public void Nav0003SourceFileNeedsToBeSavedBeforeIncludeDirectiveCanBeProcessed() {

            var nav = @"
            taskref ""foo.nav""
//                   ^-------^ Nav0003SourceFileNeedsToBeSavedBeforeIncludeDirectiveCanBeProcessed
            task A
            {
                init I1;            
                exit e1;
                I1 --> e1;     
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0003SourceFileNeedsToBeSavedBeforeIncludeDirectiveCanBeProcessed));
        }

        [Test]
        public void Nav0004File0NotFound() {

            var nav = @"
            taskref ""foo.nav""
//                   ^-------^ Nav0004File0NotFound
            task A
            {
                init I1;            
                exit e1;
                I1 --> e1;     
            }
            ";

            var unit = ParseModel(nav, MkFileName("bar.nav"));

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0004File0NotFound));
        }

        [Test]
        public void Nav0005IncludeFile0HasSomeErrors() {

            // TODO Nav0005IncludeFile0HasSomeErrors
        }

        [Test]
        public void Nav0010CannotResolveTask0_Unused() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                task C;
//                   ^-- Nav0010CannotResolveTask0
//              ^-----^  Nav1012TaskNode0NotRequired
                I1 --> e1;     
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0),
                                This(DiagnosticDescriptors.DeadCode.Nav1012TaskNode0NotRequired));
        }

        [Test]
        public void Nav0010CannotResolveTask0_Used() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                task C;
//                   ^-- Nav0010CannotResolveTask0
                I1 --> e1;   
                I1 --> C;  
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0));
        }

        [Test]
        public void Nav0011CannotResolveNode0() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                
                I1 --> e1;     
                I2 --> e1;
//              ^-- Nav0011CannotResolveNode0
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0));
        }

        [Test]
        public void Nav0012CannotResolveExit0() {

            var nav = @"
            task B {
                init i1;
                exit e1;
                i1 --> e1;
            }
            task A
            {
                task B;
                init I1;            
                exit e1;
                
                I1 --> B;    
                B:e1 --> e1;
                B:e2 --> e1;
//                ^-- Nav0012CannotResolveExit0
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0));
        }

        [Test]
        public void Nav0020TaskWithName0AlreadyDeclared() {

            var nav = @"
            task A {
                init i1;
                exit e1;
                i1 --> e1;
            }
            task A
            {
                init I1;            
                exit e1;
                
                I1 --> e1;    
            }
            ";

            var unit = ParseModel(nav);

            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0020TaskWithName0AlreadyDeclared, 2));
        }

        [Test]
        public void Nav0021ConnectionPointWithName0AlreadyDeclared() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                exit e1;
                
                I1 --> e1;    
            }
            ";

            var unit = ParseModel(nav);
            // TODO ist das wirklich schön, wenn wir zwei Fehlemeldungen für die selbe Ursache bekommen?
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0021ConnectionPointWithName0AlreadyDeclared, 2), 
                                This(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared, 2));
        }

        [Test]
        public void Nav0022NodeWithName0AlreadyDeclared() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                task A;
                task A;
                
                I1   --> e1;    
                I1   --> A;
                A:e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared, 2));
        }

        [Test]
        public void Nav0022NodeWithName0AlreadyDeclared_3Nodes() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                task A;
                task A;
                task A;
                
                I1   --> e1;    
                I1   --> A;
                A:e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared, 2), 
                                This(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared, 2));
        }

        [Test]
        public void Nav0026TriggerWithName0AlreadyDeclared() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                view A;
                
                I1 --> A;
                A  --> e1 on Foo, Foo;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0026TriggerWithName0AlreadyDeclared, 2));
        }

        [Test]
        public void Nav0023AnOutgoingEdgeForTrigger0IsAlreadyDeclared() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                view A;
                
                I1 --> A;
                A  --> e1 on Foo;
                A  --> e1 on Foo;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0023AnOutgoingEdgeForTrigger0IsAlreadyDeclared, 2));
        }

        [Test]
        public void Nav0100TaskNode0MustNotContainLeavingEdges() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                task A;
                
                I1 --> A;
                A --> e1;
                A:e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0100TaskNode0MustNotContainLeavingEdges));
        }

        [Test]
        public void Nav0101ExitNodeMustNotContainLeavingEdges() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                
                I1 --> e1;
                e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0101ExitNodeMustNotContainLeavingEdges));
        }

        [Test]
        [Ignore("Dieser Test funktioniert in der Praxis nicht, da er bereits zu einem Syntaxfehler führt.")]
        public void Nav0102EndNodeMustNotContainLeavingEdges() {

            var nav = @"
          
            task A
            {
                init I1;            
                exit e1;
                end;
                
                I1 --> e1;
                end --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0102EndNodeMustNotContainLeavingEdges));
        }

        [Test]
        public void Nav0103InitNodeMustNotContainIncomingEdges() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                task A;

                I1   --> e1;
                I1   --> A;
                A:e1 --> I1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0103InitNodeMustNotContainIncomingEdges));
        }

        [Test]
        public void Nav0104ChoiceNode0MustOnlyReachedByGoTo_OnModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                choice Choice_e1;

                I1   o-> Choice_e1;
                Choice_e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0104ChoiceNode0MustOnlyReachedByGoTo_OnNonModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                choice Choice_e1;

                I1   ==> Choice_e1;
                Choice_e1 --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0104ChoiceNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0105ExitNode0MustOnlyReachedByGoTo_OnModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                view V;

                I1   --> V;
                V o-> e1 on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0105ExitNode0MustOnlyReachedByGoTo_OnNonModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                view V;

                I1   --> V;
                V ==> e1 on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0105ExitNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0106EndNode0MustOnlyReachedByGoTo_OnModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                end;
                view V;

                I1   --> V;
                V o-> end on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0106EndNode0MustOnlyReachedByGoTo_OnNonModalEdge() {

            var nav = @"
            task A
            {
                init I1;            
                end;
                view V;

                I1   --> V;
                V ==> end on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo));
        }

        [Test]
        public void Nav0107ExitNode0HasNoIncomingEdges() {

            var nav = @"
            task A
            {
                init I1;            
                exit e1;
                view V;

                I1  --> V;
                V   --> V on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0107ExitNode0HasNoIncomingEdges));
        }

        [Test]
        public void Nav0108EndNodeHasNoIncomingEdges() {

            var nav = @"
            task A
            {
                init I1;            
                end;
                view V;

                I1  --> V;
                V   --> V on Trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0108EndNodeHasNoIncomingEdges));
        }

        [Test]
        public void Nav0109InitNode0HasNoOutgoingEdges() {

            var nav = @"
            task A
            {
                init I1;    
                init I2;
//              ^---- Nav0109InitNode0HasNoOutgoingEdges
                exit e1;

                I1  --> e1;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0109InitNode0HasNoOutgoingEdges));
        }

        [Test]
        public void Nav0110Edge0NotAllowedIn1BecauseItsReachableFromInit2() {

            var nav = @"
            task A
            {
                init I1;    
                exit e1;
                choice Choice_e1;
                view v1;

                I1  --> Choice_e1;
                Choice_e1 o-> v1;
//                        ^-- Nav0110Edge0NotAllowedIn1BecauseItsReachableFromInit2
                v1 --> e1 on trigger;
            }
            ";

            var unit = ParseModel(nav);
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0110Edge0NotAllowedIn1BecauseItsReachableFromInit2));
        }

        #region Infrastructure

        static DiagnosticExpectation This(DiagnosticDescriptor diagnosticDescriptor, int locationCount = 1) {
            return new DiagnosticExpectation(diagnosticDescriptor, locationCount);
        }

        class DiagnosticExpectation {

            public DiagnosticExpectation(DiagnosticDescriptor diagnosticDescriptor, int locationCount=1) {
                DiagnosticDescriptor = diagnosticDescriptor;
                LocationCount = locationCount;
            }

            public DiagnosticDescriptor DiagnosticDescriptor { get; }
            public int LocationCount { get; }
        }
        
        void ExpectExactly(CodeGenerationUnit unit, params DiagnosticExpectation[] diags) {
            ExpectExactly(unit.Diagnostics, diags);
        }

        void ExpectExactly(IReadOnlyList<Diagnostic> diagnostics, params DiagnosticExpectation[] expects) {
            Assert.That(diagnostics.Select(diag => diag.Descriptor), Is.EquivalentTo(expects.Select(e=>e.DiagnosticDescriptor)));

            var expectedLocations=diagnostics.Join(expects, 
                                  diag => diag.Descriptor.Id, 
                                  exp => exp.DiagnosticDescriptor.Id, 
                                  (diag, exp) => new { Diagnostic= diag, ExpectedLocations= exp.LocationCount });

            foreach (var x in expectedLocations) {
                Assert.That(x.Diagnostic.GetLocations().Count(), Is.EqualTo(x.ExpectedLocations), x.Diagnostic.ToString());
            }
        }

        CodeGenerationUnit ParseModel(string source, string filePath=null) {
            var syntax=Syntax.ParseCodeGenerationUnit(source, filePath);
            var model= CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax);
            return model;
        }

        static string MkFileName(string filename) {
            return $@"n:\av\{filename}";
        }

        #endregion
    }
}
