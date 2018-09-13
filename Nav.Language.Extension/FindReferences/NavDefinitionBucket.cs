#region Using Directives

using System;

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class NavDefinitionBucket: DefinitionBucket {

        public NavDefinitionBucket(string name, string sourceTypeIdentifier, string identifier, object tooltip = null, StringComparer comparer = null, bool expandedByDefault = true)
            : base(name, sourceTypeIdentifier, identifier, tooltip, comparer, expandedByDefault) {
        }

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