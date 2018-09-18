#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceEntry {

        public ReferenceEntry(DefinitionEntry definition, Location location,
                              ImmutableArray<ClassifiedText> lineParts,
                              TextExtent lineHighlightExtent,
                              ImmutableArray<ClassifiedText> previewParts,
                              TextExtent previewHighlightExtent) {

            Definition             = definition ?? throw new ArgumentNullException(nameof(definition));
            Location               = location   ?? throw new ArgumentNullException(nameof(location));
            LineParts              = lineParts;
            LineHighlightExtent    = lineHighlightExtent;
            PreviewParts           = previewParts;
            PreviewHighlightExtent = previewHighlightExtent;

        }

        public DefinitionEntry                Definition             { get; }
        public TextExtent                     LineHighlightExtent    { get; }
        public ImmutableArray<ClassifiedText> LineParts              { get; }
        public Location                       Location               { get; }
        public ImmutableArray<ClassifiedText> PreviewParts           { get; }
        public TextExtent                     PreviewHighlightExtent { get; }

        public string LineText => LineParts.JoinText();

    }

}