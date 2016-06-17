#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    abstract class LocationInfoProvider : ILocationInfoProvider {

        public abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = new CancellationToken());

        protected static IEnumerable<T> ToEnumerable<T>(T value) {
            return new[] { value };
        }
    }
}