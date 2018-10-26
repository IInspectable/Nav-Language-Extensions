using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language.Internal {

    static class SyntaxBuildingExtension {

        public static IEnumerable<T> OfSyntaxType<T>(this IEnumerable<SyntaxNode> syntaxen) where T : SyntaxNode {
            return syntaxen.Where(s=>s!= null).Select(OfSyntaxType<T>);
        }

        static T OfSyntaxType<T>(this SyntaxNode syntax) where T : SyntaxNode {
            if (!(syntax is T)) {
                throw new InvalidOperationException($"{typeof(T).Name} expected");
            }
            return (T) syntax;
        }

        public static T OfSyntaxType<T>(this OptionalSyntaxElement optionalSyntaxElement) where T : SyntaxNode {
            return optionalSyntaxElement.Syntax?.OfSyntaxType<T>();
        }

        public static OptionalSyntaxElement Optional<T>(this T context, Func<T, SyntaxNode> visitor) where T : ParserRuleContext {
            return context == null ? new OptionalSyntaxElement(null) : new OptionalSyntaxElement(visitor(context));
        }
       
        internal struct OptionalSyntaxElement {
            public OptionalSyntaxElement(SyntaxNode syntax) {
                Syntax = syntax;
            }
            
            public SyntaxNode Syntax { get; }
        }
      
        public static IEnumerable<SyntaxNode> ZeroOrMore<T>(this IEnumerable<T> contexts, Func<T, SyntaxNode> visitor)
            where T : ParserRuleContext {
            return contexts.Select(visitor);
        }

        public static Location GetLocation(this IToken token, string filePath) {

            if (token == null) {
                return new Location(TextExtent.Missing, LinePosition.Empty, filePath);
            }

            var linePosition = new LinePosition(token.Line -1, token.Column);
            var textExtent = TextExtent.FromBounds(token.StartIndex, token.StopIndex + 1);

            var location = new Location(textExtent, linePosition, filePath);

            return location;
        }
    }
}