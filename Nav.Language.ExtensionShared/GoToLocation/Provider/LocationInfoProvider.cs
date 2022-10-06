#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider; 

abstract class LocationInfoProvider : ILocationInfoProvider {

    public abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = new CancellationToken());

    protected static IEnumerable<T> ToEnumerable<T>(T value) {
        return new[] { value };
    }
}