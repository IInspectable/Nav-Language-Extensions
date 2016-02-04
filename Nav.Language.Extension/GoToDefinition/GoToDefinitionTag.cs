using System;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    public class GoToDefinitionTag: ITag {

        public GoToDefinitionTag(string fileName) {
            if(fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            Location = new Location(fileName);
        }

        public GoToDefinitionTag(Location location) {
            if(location == null) {
                throw new ArgumentNullException(nameof(location));
            }
            Location = location;
        }
        
        public Location Location { get; }
    }
}