﻿#region Using Directives

using NUnit.Framework;
using Pharmatechnik.Nav.Language;

#endregion

namespace Nav.Language.Tests {

    [TestFixture]
    public class SyntaxFactsTest {

        readonly string[] _keywords = {
            "spontaneous",
            "choice",
            "generateto",
            "base",
            "else",
            "view",
            "init",
            "Init",
            "exit",
            "end",
            "if",
            "do",
            "on",
            "spont",
            "donotinject",
            "taskref",
            "params",
            "namespaceprefix",
            "using",
            "result",
            "task",
            "dialog",
            "notimplemented",
            "abstractmethod",
            "-->",
            "*->",
            "o->",
            "==>",
            "code"
        };

        readonly string[] _punctuations = {
            ",",
            ")",
            "(",
            ">",
            "<",
            ";",
            ":",
            "}",
            "{",
            "]",
            "["
        };

        [Test]
        public void TaskKeywordTest() {
            Assert.That(SyntaxFacts.TaskKeyword, Is.EqualTo("task"));
        }

        [Test]
        public void TaskrefKeywordTest() {
            Assert.That(SyntaxFacts.TaskrefKeyword, Is.EqualTo("taskref"));
        }

        [Test]
        public void InitKeywordAltTest() {
            Assert.That(SyntaxFacts.InitKeywordAlt, Is.EqualTo("Init"));
        }

        [Test]
        public void InitKeywordTest() {
            Assert.That(SyntaxFacts.InitKeyword, Is.EqualTo("init"));
        }

        [Test]
        public void EndKeywordTest() {
            Assert.That(SyntaxFacts.EndKeyword, Is.EqualTo("end"));
        }

        [Test]
        public void ChoiceKeywordTest() {
            Assert.That(SyntaxFacts.ChoiceKeyword, Is.EqualTo("choice"));
        }

        [Test]
        public void DialogKeywordTest() {
            Assert.That(SyntaxFacts.DialogKeyword, Is.EqualTo("dialog"));
        }

        [Test]
        public void ViewKeywordTest() {
            Assert.That(SyntaxFacts.ViewKeyword, Is.EqualTo("view"));
        }

        [Test]
        public void ExitKeywordTest() {
            Assert.That(SyntaxFacts.ExitKeyword, Is.EqualTo("exit"));
        }

        [Test]
        public void OnKeywordTest() {
            Assert.That(SyntaxFacts.OnKeyword, Is.EqualTo("on"));
        }

        [Test]
        public void IfKeywordTest() {
            Assert.That(SyntaxFacts.IfKeyword, Is.EqualTo("if"));
        }

        [Test]
        public void ElseKeywordTest() {
            Assert.That(SyntaxFacts.ElseKeyword, Is.EqualTo("else"));
        }

        [Test]
        public void SpontaneousKeywordTest() {
            Assert.That(SyntaxFacts.SpontaneousKeyword, Is.EqualTo("spontaneous"));
        }

        [Test]
        public void SpontKeywordTest() {
            Assert.That(SyntaxFacts.SpontKeyword, Is.EqualTo("spont"));
        }

        [Test]
        public void DoKeywordTest() {
            Assert.That(SyntaxFacts.DoKeyword, Is.EqualTo("do"));
        }

        [Test]
        public void ResultKeywordTest() {
            Assert.That(SyntaxFacts.ResultKeyword, Is.EqualTo("result"));
        }

        [Test]
        public void ParamsKeywordTest() {
            Assert.That(SyntaxFacts.ParamsKeyword, Is.EqualTo("params"));
        }

        [Test]
        public void BaseKeywordTest() {
            Assert.That(SyntaxFacts.BaseKeyword, Is.EqualTo("base"));
        }

        [Test]
        public void NamespaceprefixKeywordTest() {
            Assert.That(SyntaxFacts.NamespaceprefixKeyword, Is.EqualTo("namespaceprefix"));
        }

        [Test]
        public void UsingKeywordTest() {
            Assert.That(SyntaxFacts.UsingKeyword, Is.EqualTo("using"));
        }

        [Test]
        public void CodeKeywordTest() {
            Assert.That(SyntaxFacts.CodeKeyword, Is.EqualTo("code"));
        }

        [Test]
        public void GeneratetoKeywordTest() {
            Assert.That(SyntaxFacts.GeneratetoKeyword, Is.EqualTo("generateto"));
        }

        [Test]
        public void NotimplementedKeywordTest() {
            Assert.That(SyntaxFacts.NotimplementedKeyword, Is.EqualTo("notimplemented"));
        }

        [Test]
        public void AbstractmethodKeywordTest() {
            Assert.That(SyntaxFacts.AbstractmethodKeyword, Is.EqualTo("abstractmethod"));
        }

        [Test]
        public void DonotinjectKeywordTest() {
            Assert.That(SyntaxFacts.DonotinjectKeyword, Is.EqualTo("donotinject"));
        }

        [Test]
        public void GoToEdgeKeywordTest() {
            Assert.That(SyntaxFacts.GoToEdgeKeyword, Is.EqualTo("-->"));
        }

        [Test]
        public void ModalEdgeKeywordTest() {
            Assert.That(SyntaxFacts.ModalEdgeKeyword, Is.EqualTo("o->"));
        }

        [Test]
        public void ModalEdgeKeywordAltTest() {
            Assert.That(SyntaxFacts.ModalEdgeKeywordAlt, Is.EqualTo("*->"));
        }

        [Test]
        public void NonModalEdgeKeywordTest() {
            Assert.That(SyntaxFacts.NonModalEdgeKeyword, Is.EqualTo("==>"));
        }

        [Test]
        public void OpenBraceTest() {
            Assert.That(SyntaxFacts.OpenBrace, Is.EqualTo("{"));
        }

        [Test]
        public void CloseBraceTest() {
            Assert.That(SyntaxFacts.CloseBrace, Is.EqualTo("}"));
        }

        [Test]
        public void OpenParenTest() {
            Assert.That(SyntaxFacts.OpenParen, Is.EqualTo("("));
        }

        [Test]
        public void CloseParenTest() {
            Assert.That(SyntaxFacts.CloseParen, Is.EqualTo(")"));
        }

        [Test]
        public void OpenBracketTest() {
            Assert.That(SyntaxFacts.OpenBracket, Is.EqualTo("["));
        }

        [Test]
        public void CloseBracketTest() {
            Assert.That(SyntaxFacts.CloseBracket, Is.EqualTo("]"));
        }

        [Test]
        public void LessThanTest() {
            Assert.That(SyntaxFacts.LessThan, Is.EqualTo("<"));
        }

        [Test]
        public void GreaterThanTest() {
            Assert.That(SyntaxFacts.GreaterThan, Is.EqualTo(">"));
        }

        [Test]
        public void SemicolonTest() {
            Assert.That(SyntaxFacts.Semicolon, Is.EqualTo(";"));
        }

        [Test]
        public void CommaTest() {
            Assert.That(SyntaxFacts.Comma, Is.EqualTo(","));
        }

        [Test]
        public void ColonTest() {
            Assert.That(SyntaxFacts.Colon, Is.EqualTo(":"));
        }

        [Test]
        public void SingleLineCommentStringTest() {
            Assert.That(SyntaxFacts.SingleLineComment, Is.EqualTo("//"));
        }

        [Test]
        public void BlockCommentStartStringTest() {
            Assert.That(SyntaxFacts.BlockCommentStart, Is.EqualTo("/*"));
        }

        [Test]
        public void BlockCommentEndStringTest() {
            Assert.That(SyntaxFacts.BlockCommentEnd, Is.EqualTo("*/"));
        }
        
        [Test]
        public void KeywordsTest() {
            Assert.That(SyntaxFacts.Keywords, Is.EquivalentTo(_keywords));
        }

        [Test]
        public void IsKeywordTest() {
            foreach(var k in _keywords) {
                Assert.That(SyntaxFacts.IsKeyword(k), Is.True, $"'{k}' should be a keyword");
            }

            var notAKeyword = "Max";
            Assert.That(SyntaxFacts.IsKeyword(notAKeyword), Is.False, $"'{notAKeyword}' should NOT be a keyword");
        }

        
        [Test]
        public void PunctuationsTest() {
            Assert.That(SyntaxFacts.Punctuations, Is.EquivalentTo(_punctuations));
        }

        [Test]
        public void IsPunctuationTest() {
            foreach (var p in _punctuations) {
                Assert.That(SyntaxFacts.IsPunctuation(p), Is.True, $"'{p}' should be a punctuation");
            }

            var notAPunctuation = "!";
            Assert.That(SyntaxFacts.IsPunctuation(notAPunctuation), Is.False, $"'{notAPunctuation}' should NOT be a punctuation");
        }
    }
}
