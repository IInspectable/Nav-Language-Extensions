#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    public abstract class GoToTag: ITag {

        // TODO hier LocationResult zur�ckliefern, damit der Aufrufer ggf. eine Fehlermeldung anzeigen kann
        [NotNull]
        public abstract Task<Location> GetLocationAsync(CancellationToken cancellationToken=default(CancellationToken));
    }
}