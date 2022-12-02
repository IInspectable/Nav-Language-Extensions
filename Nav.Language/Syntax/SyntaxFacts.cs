#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Generated;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language; 

public static class SyntaxFacts {

    // Keywords
    public static readonly string TaskKeyword            = GetLiteralName(NavGrammar.TaskKeyword);
    public static readonly string TaskrefKeyword         = GetLiteralName(NavGrammar.TaskrefKeyword);
    public static readonly string InitKeyword            = GetLiteralName(NavGrammar.InitKeyword);
    public static readonly string InitKeywordAlt         = InitKeyword.ToPascalcase();
    public static readonly string EndKeyword             = GetLiteralName(NavGrammar.EndKeyword);
    public static readonly string ChoiceKeyword          = GetLiteralName(NavGrammar.ChoiceKeyword);
    public static readonly string DialogKeyword          = GetLiteralName(NavGrammar.DialogKeyword);
    public static readonly string ViewKeyword            = GetLiteralName(NavGrammar.ViewKeyword);
    public static readonly string ExitKeyword            = GetLiteralName(NavGrammar.ExitKeyword);
    public static readonly string OnKeyword              = GetLiteralName(NavGrammar.OnKeyword);
    public static readonly string IfKeyword              = GetLiteralName(NavGrammar.IfKeyword);
    public static readonly string ElseKeyword            = GetLiteralName(NavGrammar.ElseKeyword);
    public static readonly string SpontaneousKeyword     = GetLiteralName(NavGrammar.SpontaneousKeyword);
    public static readonly string SpontKeyword           = GetLiteralName(NavGrammar.SpontKeyword);
    public static readonly string DoKeyword              = GetLiteralName(NavGrammar.DoKeyword);
    public static readonly string ResultKeyword          = GetLiteralName(NavGrammar.ResultKeyword);
    public static readonly string ParamsKeyword          = GetLiteralName(NavGrammar.ParamsKeyword);
    public static readonly string BaseKeyword            = GetLiteralName(NavGrammar.BaseKeyword);
    public static readonly string NamespaceprefixKeyword = GetLiteralName(NavGrammar.NamespaceprefixKeyword);
    public static readonly string UsingKeyword           = GetLiteralName(NavGrammar.UsingKeyword);
    public static readonly string CodeKeyword            = GetLiteralName(NavGrammar.CodeKeyword);
    public static readonly string GeneratetoKeyword      = GetLiteralName(NavGrammar.GeneratetoKeyword);
    public static readonly string NotimplementedKeyword  = GetLiteralName(NavGrammar.NotimplementedKeyword);
    public static readonly string AbstractmethodKeyword  = GetLiteralName(NavGrammar.AbstractmethodKeyword);
    public static readonly string DonotinjectKeyword     = GetLiteralName(NavGrammar.DonotinjectKeyword);
    public static readonly string GoToEdgeKeyword        = GetLiteralName(NavGrammar.GoToEdgeKeyword);
    public static readonly string NonModalEdgeKeyword    = GetLiteralName(NavGrammar.NonModalEdgeKeyword);
    public static readonly string ModalEdgeKeyword       = "o->";
    public static readonly string ModalEdgeKeywordAlt    = "*->";

    public static readonly ImmutableHashSet<string> NavKeywords = new[] {
        TaskKeyword,
        TaskrefKeyword,
        InitKeyword,
        InitKeywordAlt,
        EndKeyword,
        ChoiceKeyword,
        DialogKeyword,
        ViewKeyword,
        ExitKeyword,
        OnKeyword,
        IfKeyword,
        ElseKeyword,
        SpontaneousKeyword,
        SpontKeyword,
        DoKeyword,
        GoToEdgeKeyword,
        NonModalEdgeKeyword,
        ModalEdgeKeyword,
        ModalEdgeKeywordAlt
    }.ToImmutableHashSet();

    public static bool IsNavKeyword(string value) {
        return NavKeywords.Contains(value);
    }

    public static readonly ImmutableHashSet<string> CodeKeywords = new[] {
        ResultKeyword,
        ParamsKeyword,
        BaseKeyword,
        NamespaceprefixKeyword,
        UsingKeyword,
        CodeKeyword,
        GeneratetoKeyword,
        NotimplementedKeyword,
        AbstractmethodKeyword,
        DonotinjectKeyword

    }.ToImmutableHashSet();

    public static bool IsCodeKeyword(string value) {
        return CodeKeywords.Contains(value);
    }

    public static readonly ImmutableHashSet<string> Keywords = NavKeywords.Concat(CodeKeywords).ToImmutableHashSet();

    public static bool IsKeyword(string value) {
        return Keywords.Contains(value);
    }

    public static readonly ImmutableHashSet<string> HiddenKeywords = new[] {
        SpontaneousKeyword,
        SpontKeyword,
        NotimplementedKeyword,
        ModalEdgeKeywordAlt,
        NonModalEdgeKeyword

    }.ToImmutableHashSet();

    public static bool IsHiddenKeyword(string value) {
        return HiddenKeywords.Contains(value);
    }

    public static readonly ImmutableHashSet<string> EdgeKeywords = new[] {
        GoToEdgeKeyword,
        NonModalEdgeKeyword,
        ModalEdgeKeyword,
        ModalEdgeKeywordAlt

    }.ToImmutableHashSet();

    public static bool IsEdgeKeyword(string value) {
        return EdgeKeywords.Contains(value);
    }

    // Punctuation
    public static readonly char OpenBrace    = GetLiteralNameAsChar(NavGrammar.OpenBrace);
    public static readonly char CloseBrace   = GetLiteralNameAsChar(NavGrammar.CloseBrace);
    public static readonly char OpenParen    = GetLiteralNameAsChar(NavGrammar.OpenParen);
    public static readonly char CloseParen   = GetLiteralNameAsChar(NavGrammar.CloseParen);
    public static readonly char OpenBracket  = GetLiteralNameAsChar(NavGrammar.OpenBracket);
    public static readonly char CloseBracket = GetLiteralNameAsChar(NavGrammar.CloseBracket);
    public static readonly char LessThan     = GetLiteralNameAsChar(NavGrammar.LessThan);
    public static readonly char GreaterThan  = GetLiteralNameAsChar(NavGrammar.GreaterThan);
    public static readonly char Semicolon    = GetLiteralNameAsChar(NavGrammar.Semicolon);
    public static readonly char Comma        = GetLiteralNameAsChar(NavGrammar.Comma);
    public static readonly char Colon        = GetLiteralNameAsChar(NavGrammar.Colon);

    public static readonly ImmutableHashSet<char> Punctuations = new[] {
        OpenBrace,
        CloseBrace,
        OpenParen,
        CloseParen,
        OpenBracket,
        CloseBracket,
        LessThan,
        GreaterThan,
        Semicolon,
        Comma,
        Colon
    }.ToImmutableHashSet();

    public static bool IsPunctuation(string value) {

        if (value?.Length != 1) {
            return false;
        }

        return Punctuations.Contains(value[0]);
    }

    public static bool IsPunctuation(char value) {
        return Punctuations.Contains(value);
    }

    public static bool IsIdentifierCharacter(char c) {

        return c is >= 'a' and <= 'z' ||
               c is >= 'A' and <= 'Z' ||
               c is >= '0' and <= '9' ||
               c == 'Ä'               || c == 'Ö' || c == 'Ü' ||
               c == 'ä'               || c == 'ö' || c == 'ü' ||
               c == 'ß'               || c == '.' || c == '_';
    }

    public static bool IsValidIdentifier(string value) {
        if (string.IsNullOrEmpty(value)) {
            return false;
        }

        if (Keywords.Contains(value)) {
            return false;
        }

        return value.All(IsIdentifierCharacter);
    }

    // Comment strings
    public static readonly string SingleLineComment = "//";
    public static readonly string BlockCommentStart = "/*";
    public static readonly string BlockCommentEnd   = "*/";

    static char GetLiteralNameAsChar(int tokenType) {
        string name = GetLiteralName(tokenType);
        if (name.Length != 1) {
            throw new InvalidOperationException($"{name} has more or less than one char.");
        }

        return name[0];
    }

    static string GetLiteralName(int tokenType) {
        return NavGrammar.DefaultVocabulary.GetLiteralName(tokenType).Trim('\'');
    }

    public static bool IsTrivia(TextClassification classification) {
        return classification == TextClassification.Comment || classification == TextClassification.Whitespace;
    }

}