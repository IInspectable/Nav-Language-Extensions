#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    public class GoToLocationTag : GoToTag, ITag {

        public GoToLocationTag(LocationInfo location) {

            Locations = new List<LocationInfo> {
                location
            };
        }

        public List<LocationInfo> Locations { get; }

        public override Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            return Task.FromResult(Locations.AsEnumerable());
        }

        #region Equality members

        // TODO Equality

        #endregion
    }
}