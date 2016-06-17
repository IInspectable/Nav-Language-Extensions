#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    public interface ILocationInfoProvider {

        [NotNull]
        Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken));        
    }
}