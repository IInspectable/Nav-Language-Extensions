#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation {

    public class GoToTag: ITag {

        public GoToTag() {
            Provider = new List<ILocationInfoProvider>();
        }

        public GoToTag(ILocationInfoProvider provider) {
            // TODO arg check
            Provider = new List<ILocationInfoProvider> { provider };
        }

        public List<ILocationInfoProvider> Provider { get; }

        [NotNull]
        public virtual async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var locationInfos = new List<LocationInfo>();

            foreach(var provider in Provider) {
                var lis = await provider.GetLocationsAsync(cancellationToken);

                locationInfos.AddRange(lis);
            }

            return locationInfos;
        }

        protected static IEnumerable<T> ToEnumerable<T>(T value) {
            return new [] { value };
        }
    }
}