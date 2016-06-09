using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    public abstract class GoToDefinitionTag: ITag {

        public abstract Task<Location> GetLocationAsync();
    }
}