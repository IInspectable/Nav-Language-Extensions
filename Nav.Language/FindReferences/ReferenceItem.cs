#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceItem {

        public ReferenceItem(DefinitionItem definition,
                             Location location,
                             ImmutableArray<ClassifiedText> textParts,
                             TextExtent textHighlightExtent,
                             ImmutableArray<ClassifiedText> toolTipParts,
                             TextExtent toolTipHighlightExtent) {

            Definition             = definition ?? throw new ArgumentNullException(nameof(definition));
            Location               = location   ?? throw new ArgumentNullException(nameof(location));
            TextParts              = textParts;
            TextHighlightExtent    = textHighlightExtent;
            ToolTipParts           = toolTipParts;
            ToolTipHighlightExtent = toolTipHighlightExtent;

        }

        public static ReferenceItem NoReferencesFoundTo(DefinitionItem definition) {
            return new ReferenceItem(
                definition            : definition,
                location              : definition.Location,
                textParts             : new[] {
                    ClassifiedTexts.Text($"No references found to '{definition.Text}'.")
                }.ToImmutableArray(),
                textHighlightExtent   : TextExtent.Missing,
                toolTipParts          : ImmutableArray<ClassifiedText>.Empty,
                toolTipHighlightExtent: TextExtent.Missing
            );
        }

        public DefinitionItem                 Definition             { get; }
        public Location                       Location               { get; }
        public ImmutableArray<ClassifiedText> TextParts              { get; }
        public TextExtent                     TextHighlightExtent    { get; }
        public ImmutableArray<ClassifiedText> ToolTipParts           { get; }
        public TextExtent                     ToolTipHighlightExtent { get; }

        public string Text        => TextParts.JoinText();
        public string ToolTip     => ToolTipParts.JoinText();
        public string ProjectName => Definition.SearchRoot;

    }

}