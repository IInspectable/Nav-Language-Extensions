namespace Pharmatechnik.Nav.Language.Text; 

public static class ClassifiedTexts {

    public static readonly ClassifiedText Space = new ClassifiedText(" ", TextClassification.Whitespace);
    public static readonly ClassifiedText Colon = Punctuation(SyntaxFacts.Colon.ToString());

    public static ClassifiedText Text(char c) => Text(c.ToString());
    public static ClassifiedText Text(string text) => new ClassifiedText(text,                            TextClassification.Text);
    public static ClassifiedText Keyword(string keyword) => new ClassifiedText(keyword,                   TextClassification.Keyword);
    public static ClassifiedText TaskName(string taskName) => new ClassifiedText(taskName,                TextClassification.TaskName);
    public static ClassifiedText GuiNode(string formName) => new ClassifiedText(formName,                 TextClassification.GuiNode);
    public static ClassifiedText ChoiceNode(string formName) => new ClassifiedText(formName,              TextClassification.ChoiceNode);
    public static ClassifiedText MethodName(string formName) => new ClassifiedText(formName,              TextClassification.ChoiceNode);
    public static ClassifiedText Identifier(string identifier) => new ClassifiedText(identifier,          TextClassification.Identifier);
    public static ClassifiedText ConnectionPoint(string identifier) => new ClassifiedText(identifier,     TextClassification.ConnectionPoint);
    public static ClassifiedText Whitespace(string whitespace) => new ClassifiedText(whitespace,          TextClassification.Whitespace);
    public static ClassifiedText Punctuation(string punctuation) => new ClassifiedText(punctuation,       TextClassification.Punctuation);
    public static ClassifiedText StringLiteral(string stringLiteral) => new ClassifiedText(stringLiteral, TextClassification.StringLiteral);

}