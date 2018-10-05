#region Using Directives

using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class DefinitionItem {

        DefinitionItem([CanBeNull] ISymbol symbol,
                       ImmutableArray<ClassifiedText> textParts,
                       bool expandedByDefault,
                       string sortKey) {

            Symbol            = symbol;
            TextParts         = textParts;
            ExpandedByDefault = expandedByDefault;
            SortKey           = sortKey ?? "";
        }

        public static DefinitionItem Create(ISymbol symbol,
                                            ImmutableArray<ClassifiedText> textParts,
                                            bool expandedByDefault = true,
                                            string sortKey = null) {

            return new DefinitionItem(symbol, textParts, expandedByDefault, sortKey);

        }

        public static DefinitionItem CreateSimpleItem(string text) {
            return SimpleTextDefinition.Create(text);
        }

        [CanBeNull]
        public ISymbol Symbol { get; }

        [CanBeNull]
        public Location Location => Symbol?.Location;

        public ImmutableArray<ClassifiedText> TextParts         { get; }
        public bool                           ExpandedByDefault { get; } // TODO In etwas allgemeineres umbenennen?
        public string                         SortKey           { get; }

        public string Text     => TextParts.JoinText();
        public string SortText => SortKey + Text;

        class SimpleTextDefinition: DefinitionItem {

            SimpleTextDefinition(ImmutableArray<ClassifiedText> textParts):
                base(null, textParts: textParts, expandedByDefault: true, sortKey: null) {
            }

            public static SimpleTextDefinition Create(string text) {

                var textParts = new[] {ClassifiedTexts.Text(text)}.ToImmutableArray();

                return new SimpleTextDefinition(textParts);
            }

        }

    }

}