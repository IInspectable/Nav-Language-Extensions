#region Using Directives

using System.Linq;
using System.Collections.Immutable;
using Pharmatechnik.Nav.Language.Generated;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class SyntaxFacts {
        // Keywords
        public static readonly string TaskKeyword            = GetLiteralName(NavGrammarLexer.TaskKeyword);
        public static readonly string TaskrefKeyword         = GetLiteralName(NavGrammarLexer.TaskrefKeyword);
        public static readonly string InitKeyword            = GetLiteralName(NavGrammarLexer.InitKeyword);
        public static readonly string InitKeywordAlt         = InitKeyword.ToPascalcase();
        public static readonly string EndKeyword             = GetLiteralName(NavGrammarLexer.EndKeyword);
        public static readonly string ChoiceKeyword          = GetLiteralName(NavGrammarLexer.ChoiceKeyword);
        public static readonly string DialogKeyword          = GetLiteralName(NavGrammarLexer.DialogKeyword);
        public static readonly string ViewKeyword            = GetLiteralName(NavGrammarLexer.ViewKeyword);
        public static readonly string ExitKeyword            = GetLiteralName(NavGrammarLexer.ExitKeyword);
        public static readonly string OnKeyword              = GetLiteralName(NavGrammarLexer.OnKeyword);
        public static readonly string IfKeyword              = GetLiteralName(NavGrammarLexer.IfKeyword);
        public static readonly string ElseKeyword            = GetLiteralName(NavGrammarLexer.ElseKeyword);
        public static readonly string SpontaneousKeyword     = GetLiteralName(NavGrammarLexer.SpontaneousKeyword);
        public static readonly string SpontKeyword           = GetLiteralName(NavGrammarLexer.SpontKeyword);
        public static readonly string DoKeyword              = GetLiteralName(NavGrammarLexer.DoKeyword);
        public static readonly string ResultKeyword          = GetLiteralName(NavGrammarLexer.ResultKeyword);
        public static readonly string ParamsKeyword          = GetLiteralName(NavGrammarLexer.ParamsKeyword);
        public static readonly string BaseKeyword            = GetLiteralName(NavGrammarLexer.BaseKeyword);
        public static readonly string NamespaceprefixKeyword = GetLiteralName(NavGrammarLexer.NamespaceprefixKeyword);
        public static readonly string UsingKeyword           = GetLiteralName(NavGrammarLexer.UsingKeyword);
        public static readonly string CodeKeyword            = GetLiteralName(NavGrammarLexer.CodeKeyword);
        public static readonly string GeneratetoKeyword      = GetLiteralName(NavGrammarLexer.GeneratetoKeyword);
        public static readonly string NotimplementedKeyword  = GetLiteralName(NavGrammarLexer.NotimplementedKeyword);
        public static readonly string AbstractmethodKeyword  = GetLiteralName(NavGrammarLexer.AbstractmethodKeyword);
        public static readonly string DonotinjectKeyword     = GetLiteralName(NavGrammarLexer.DonotinjectKeyword);
        public static readonly string GoToEdgeKeyword        = GetLiteralName(NavGrammarLexer.GoToEdgeKeyword);        
        public static readonly string NonModalEdgeKeyword    = GetLiteralName(NavGrammarLexer.NonModalEdgeKeyword);
        public static readonly string ModalEdgeKeyword       = "o->";
        public static readonly string ModalEdgeKeywordAlt    = "*->";

        public static readonly ImmutableHashSet<string> NavKeywords = new[] {
            TaskKeyword           ,
            TaskrefKeyword        ,
            InitKeyword           ,
            InitKeywordAlt        ,
            EndKeyword            ,
            ChoiceKeyword         ,
            DialogKeyword         ,
            ViewKeyword           ,
            ExitKeyword           ,
            OnKeyword             ,
            IfKeyword             ,
            ElseKeyword           ,
            SpontaneousKeyword    ,
            SpontKeyword          ,
            DoKeyword             ,
            GoToEdgeKeyword       ,
            NonModalEdgeKeyword   ,
            ModalEdgeKeyword      ,
            ModalEdgeKeywordAlt
        }.ToImmutableHashSet();

        public static bool IsNavKeyword(string value) {
            return NavKeywords.Contains(value);
        }

        public static readonly ImmutableHashSet<string> CodeKeywords = new[] {
            ResultKeyword         ,
            ParamsKeyword         ,
            BaseKeyword           ,
            NamespaceprefixKeyword,
            UsingKeyword          ,
            CodeKeyword           ,
            GeneratetoKeyword     ,
            NotimplementedKeyword ,
            AbstractmethodKeyword ,
            DonotinjectKeyword    
            
        }.ToImmutableHashSet();

        public static bool IsCodeKeyword(string value) {
            return CodeKeywords.Contains(value);
        }

        public static readonly ImmutableHashSet<string> Keywords = Enumerable.Concat(NavKeywords, CodeKeywords).ToImmutableHashSet();

        public static bool IsKeyword(string value) {
            return Keywords.Contains(value);
        }

        // Punctuation
        public static readonly string OpenBrace    = GetLiteralName(NavGrammarLexer.OpenBrace);
        public static readonly string CloseBrace   = GetLiteralName(NavGrammarLexer.CloseBrace);
        public static readonly string OpenParen    = GetLiteralName(NavGrammarLexer.OpenParen);
        public static readonly string CloseParen   = GetLiteralName(NavGrammarLexer.CloseParen);
        public static readonly string OpenBracket  = GetLiteralName(NavGrammarLexer.OpenBracket);
        public static readonly string CloseBracket = GetLiteralName(NavGrammarLexer.CloseBracket);
        public static readonly string LessThan     = GetLiteralName(NavGrammarLexer.LessThan);
        public static readonly string GreaterThan  = GetLiteralName(NavGrammarLexer.GreaterThan);
        public static readonly string Semicolon    = GetLiteralName(NavGrammarLexer.Semicolon);
        public static readonly string Comma        = GetLiteralName(NavGrammarLexer.Comma);
        public static readonly string Colon        = GetLiteralName(NavGrammarLexer.Colon);

        public static readonly ImmutableHashSet<string> Punctuations = new[] {
            OpenBrace   ,
            CloseBrace  ,
            OpenParen   ,
            CloseParen  ,
            OpenBracket ,
            CloseBracket,
            LessThan    ,
            GreaterThan ,
            Semicolon   ,
            Comma       ,
            Colon
        }.ToImmutableHashSet();

        public static bool IsPunctuation(string value) {
            return Punctuations.Contains(value);
        }

        public static bool IsIdentifierCharacter(char c) {

            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c >= '0' && c <= '9' ||
                   c == 'Ä' || c == 'Ö' || c == 'Ü' || 
                   c == 'ä' || c == 'ö' || c == 'ü' || 
                   c == 'ß' || c == '.' || c == '_';
        }

        public static bool IsValidIdentifier(string value) {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            return value.All(IsIdentifierCharacter);
        }

        // Comment strings
        public static readonly string SingleLineComment = "//";
        public static readonly string BlockCommentStart = "/*";
        public static readonly string BlockCommentEnd   = "*/";

        static string GetLiteralName(int tokenType) {
            return NavGrammarLexer.DefaultVocabulary.GetLiteralName(tokenType).Trim('\'');
        }        
    }
}