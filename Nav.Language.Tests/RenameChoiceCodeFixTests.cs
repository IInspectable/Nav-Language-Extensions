#region Using Directives

using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.CodeFixes.Rename;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class RenameChoiceCodeFixTests {

        public static IEnumerable<TestCaseData> TestCases = new[] {
            //=============================================
            //
            CreateTestCase(
                    originalSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Ok;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Choice_Ok on Ok;
    Choice_Ok      --> Ok;
}
",
                    expectedSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Renamed;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Choice_Renamed on Ok;
    Choice_Renamed --> Ok;
}
",
                    taskName: "MessageBox",
                    choiceName: "Choice_Ok",
                    newChoiceName: "Choice_Renamed")
                .SetName("Simple rename"),

            //=============================================
            //
            CreateTestCase(
                    originalSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Ok;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Choice_Ok on Ok;
    Choice_Ok      --> Ok;
}
",
                    expectedSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_RenamedLonger;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Choice_RenamedLonger on Ok;
    Choice_RenamedLonger --> Ok;
}
",
                    taskName: "MessageBox",
                    choiceName: "Choice_Ok",
                    newChoiceName: "Choice_RenamedLonger")
                .SetName("Rename with long choice name"),

            //=============================================
            //
            CreateTestCase(
                    originalSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Ok;

    init --> MessageBoxOK;  
    
    MessageBoxOK  	-->	Choice_Ok on Ok;
    Choice_Ok		-->	Ok;
}
",
                    expectedSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice C;

    init --> MessageBoxOK;  
    
    MessageBoxOK  	-->	C on Ok;
    C               -->	Ok;
}
",
                    taskName: "MessageBox",
                    choiceName: "Choice_Ok",
                    newChoiceName: "C")
                .SetName("Rename with short choice name"),

            //=============================================
            //
            CreateTestCase(
                    originalSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Ok;

    init --> MessageBoxOK;  
    
    MessageBoxOK  	    -->	Choice_Ok on Ok;
    Choice_Ok	/* Foo*/-->	Ok;
}
",
                    expectedSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice C;

    init --> MessageBoxOK;  
    
    MessageBoxOK  	    -->	C on Ok;
    C           /* Foo*/-->	Ok;
}
",
                    taskName: "MessageBox",
                    choiceName: "Choice_Ok",
                    newChoiceName: "C")
                .SetName("Rename with comment between source and edge"),

            //=============================================
            //
            CreateTestCase(
                    originalSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Choice_Ok;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Choice_Ok on Ok;
	Choice_Ok
            --> Ok;
}
",
                    expectedSourceText: @"
[namespaceprefix Test]

task MessageBox [base StandardWFS : ILegacyMessageBoxWFS]
				[result MessageBoxResult]
{
    init [params string message];
        
    view MessageBoxOK;
    
    exit Ok;
    choice Renamed_Choice;

    init --> MessageBoxOK;  
    
    MessageBoxOK   --> Renamed_Choice on Ok;
	Renamed_Choice
            --> Ok;
}
",
                    taskName: "MessageBox",
                    choiceName: "Choice_Ok",
                    newChoiceName: "Renamed_Choice")
                .SetName("Rename with new line between source and edge"),
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void SimpleRename(string originalSourceText, string expectedSourceText, string taskName, string choiceName, string newChoiceName) {

            var orgCodeGenerationUnit = GetCodeGenerationUnit(originalSourceText);
            Assert.That(orgCodeGenerationUnit.Diagnostics.Any(), Is.False, "Test Code should not have any diagnostics");

            var choiceNodeSymbol = FindNodeSymbol<IChoiceNodeSymbol>(orgCodeGenerationUnit, taskName, choiceName);
            ChoiceRenameCodeFix codeFix = new ChoiceRenameCodeFix(choiceNodeSymbol, orgCodeGenerationUnit, GetEditorSettings());

            Assert.That(codeFix.CanApplyFix(), Is.True);

            var textChanges = codeFix.GetTextChanges(newChoiceName);

            var actual = ApplyChanges(originalSourceText, textChanges);
            var actualCodeGenerationUnit = GetCodeGenerationUnit(actual);
            Assert.That(actualCodeGenerationUnit.Diagnostics.Any(), Is.False, "Result Code should not have any diagnostics");

            Assert.That(actual, Is.EqualTo(expectedSourceText));
        }

        #region Infrastructure

        static TestCaseData CreateTestCase(string originalSourceText, string expectedSourceText, string taskName, string choiceName, string newChoiceName) {
            return new TestCaseData(originalSourceText, expectedSourceText, taskName, choiceName, newChoiceName);
        }

        string ApplyChanges(string text, IEnumerable<TextChange> textChanges) {
            var writer = new TextChangeWriter();
            return writer.ApplyTextChanges(text, textChanges);
        }

        EditorSettings GetEditorSettings() {
            return new EditorSettings(4, "\r\n");
        }

        private static T FindNodeSymbol<T>(CodeGenerationUnit unit, string taskName, string nodeName)where T : INodeSymbol {
            var taskDefinitionSymbol = unit.TaskDefinitions.First(td => td.Name == taskName);
            var choiceNodeSymbol = taskDefinitionSymbol.NodeDeclarations.OfType<T>().First(cn => cn.Name == nodeName);
            return choiceNodeSymbol;
        }

        CodeGenerationUnit GetCodeGenerationUnit(string text) {

            var tree=SyntaxTree.ParseText(text);
            return CodeGenerationUnit.FromCodeGenerationUnitSyntax(tree.GetRoot() as CodeGenerationUnitSyntax);
        }

        #endregion
    }    
}