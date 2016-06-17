#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.CodeAnalysis;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    public abstract class GoToTag: ITag {

        [NotNull]
        public abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken=default(CancellationToken));

        protected static IEnumerable<T> ToEnumerable<T>(T value) {
            return new [] { value };
        }
    }
}