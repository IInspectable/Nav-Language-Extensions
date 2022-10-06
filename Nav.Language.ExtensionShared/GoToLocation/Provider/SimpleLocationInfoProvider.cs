#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider; 

class SimpleLocationInfoProvider: LocationInfoProvider {

    public SimpleLocationInfoProvider() {
        Locations = new List<LocationInfo>();
    }

    public SimpleLocationInfoProvider(LocationInfo locationInfo) {
        Locations = new List<LocationInfo> { locationInfo };
    }

    public SimpleLocationInfoProvider(IEnumerable<LocationInfo> locationInfos) {
        Locations = new List<LocationInfo>(locationInfos);
    }
        
    public List<LocationInfo> Locations { get; }

    public override Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = new CancellationToken()) {
        return Task.FromResult(Locations.AsEnumerable());
    }
}