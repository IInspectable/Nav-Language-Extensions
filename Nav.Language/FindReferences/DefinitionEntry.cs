using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class DefinitionEntry {

        public DefinitionEntry(ImmutableArray<ClassifiedText> displayParts) {
            DisplayParts = displayParts;

        }

        public ImmutableArray<ClassifiedText> DisplayParts { get; }
        public string Name => DisplayParts.ToText();

    }

}