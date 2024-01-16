namespace Pharmatechnik.Nav.Language.SemanticAnalyzer;

public static class AnalyzerExtensions {

    public static bool IsWarningDisabled(this INodeSymbol node, DiagnosticDescriptor descriptor) {
        var source = node.SyntaxTree?.SourceText;
        if (source == null)
            return false;

        var disableString = $"{SyntaxFacts.SingleLineComment} disable {descriptor.Id}";
        var triviaExtent  = node.Syntax.GetTrailingTriviaExtent();

        return source.Substring(triviaExtent).Contains(disableString);
    }

}