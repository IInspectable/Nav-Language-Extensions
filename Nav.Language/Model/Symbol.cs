using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    abstract partial class Symbol: ISymbol {

        protected Symbol(string name, [NotNull] Location location) {
            Name     = name;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public virtual string Name { get; }

        [NotNull]
        public Location Location { get; }
        
        int IExtent.Start { get { return Location.Start; } }
        int IExtent.End { get { return Location.End; } }
    }
}