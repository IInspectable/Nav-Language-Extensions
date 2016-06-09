using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public abstract class GoToTag: ITag {

        public abstract Task<Location> GetLocationAsync();
    }
}