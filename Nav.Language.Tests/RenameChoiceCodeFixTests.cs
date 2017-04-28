#region Using Directives

using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class RenameChoiceCodeFixTests {

        [Test]
        public void SimpleRename() {

            var orgText = @"
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
";
            var expected = @"
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
";            
            var taskName  = "MessageBox";
            var orgChoice = "Choice_Ok";
            var newChoice = "Choice_Renamed";

            var orgCodeGenerationUnit = GetCodeGenerationUnit(orgText);
            Assert.That(orgCodeGenerationUnit.Diagnostics.Any(), Is.False, "Test Code should not have any diagnostics");

            var choiceNodeSymbol = FindNodeSymbol<IChoiceNodeSymbol>(orgCodeGenerationUnit, taskName, orgChoice);
            RenameChoiceCodeFix codeFix = new RenameChoiceCodeFix(GetEditorSettings(), orgCodeGenerationUnit, choiceNodeSymbol);

            Assert.That(codeFix.CanApplyFix(), Is.True);

            var textChanges = codeFix.GetTextChanges(newChoice);

            var actual = ApplyChanges(orgText, textChanges);
            var actualCodeGenerationUnit = GetCodeGenerationUnit(actual);
            Assert.That(actualCodeGenerationUnit.Diagnostics.Any(), Is.False, "Result Code should not have any diagnostics");

            Assert.That(actual, Is.EqualTo(expected));
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
    }    
}
