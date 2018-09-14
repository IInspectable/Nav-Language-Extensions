#region Using Directives

using System;

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class NavDefinitionBucket: DefinitionBucket {

        public NavDefinitionBucket(DefinitionEntry definitionEntry, string sourceTypeIdentifier, string identifier, object tooltip = null, StringComparer comparer = null, bool expandedByDefault = true)
            : base(definitionEntry.Name, sourceTypeIdentifier, identifier, tooltip, comparer, expandedByDefault) {
            DefinitionEntry = definitionEntry;
        }

        public DefinitionEntry DefinitionEntry { get; }

        public override bool TryGetValue(string key, out object content) {
            content = null;
            switch (key) {
                case StandardTableKeyNames2.DefinitionIcon:
                    content = ImageMonikers.TaskDefinition;
                    break;
            }

            return false;
        }

    }

}