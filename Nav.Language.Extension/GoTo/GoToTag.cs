#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public abstract class GoToTag: ITag {
        [NotNull]
        public abstract Task<Location> GoToLocationAsync(CancellationToken cancellationToken=default(CancellationToken));
    }
}