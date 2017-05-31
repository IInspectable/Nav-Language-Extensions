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
            // TODO ist das wirklcih schön, wenn wir zwei Fehlemeldungen für die selbe Ursache bekommen?
            ExpectExactly(unit, This(DiagnosticDescriptors.Semantic.Nav0021ConnectionPointWithName0AlreadyDeclared, 2), 
                                This(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared, 2));
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
