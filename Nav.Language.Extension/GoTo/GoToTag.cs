#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public abstract class GoToTag: ITag {

        public abstract Task<Location> GetLocationAsync(CancellationToken cancellationToken=default(CancellationToken));
    }
}