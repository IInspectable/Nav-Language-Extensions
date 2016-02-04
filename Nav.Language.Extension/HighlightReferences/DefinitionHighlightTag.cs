using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.HighlightReferences {

    class DefinitionHighlightTag : TextMarkerTag {

        public DefinitionHighlightTag() : base(MarkerFormatDefinitionNames.DefinitionHighlight) {

        }
    }
}