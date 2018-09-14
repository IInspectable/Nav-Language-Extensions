#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceEntry {

        public ReferenceEntry(DefinitionEntry definition, Location location, string text) {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Location   = location   ?? throw new ArgumentNullException(nameof(location));
            Text       = text;

        }

        public DefinitionEntry Definition { get; }
        public Location        Location   { get; }
        public string          Text       { get; }

    }

}