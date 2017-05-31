#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    // TODO IEquatable?
    [Serializable]
    public sealed class Diagnostic {
        
        [NotNull]
        readonly object[] _messageArgs;

        public Diagnostic(Location location, DiagnosticDescriptor descriptor, params object[] messageArgs) {
            Location            = location   ?? throw new ArgumentNullException(nameof(location));
            Descriptor          = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            AdditionalLocations = EmptyAdditionalLocations;
            _messageArgs        = messageArgs?? EmptyMessageArgs;            
        }

        public Diagnostic(Location location, Location additionalLocation, DiagnosticDescriptor descriptor, params object[] messageArgs) 
            : this(location, new []{ additionalLocation }, descriptor, messageArgs) {         
        }

        public Diagnostic(Location location, IEnumerable<Location> additionalLocations, DiagnosticDescriptor descriptor, params object[] messageArgs) {
            Location            = location   ?? throw new ArgumentNullException(nameof(location));
            Descriptor          = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            AdditionalLocations = additionalLocations?.Where(loc=> loc != null).ToImmutableArray() ?? EmptyAdditionalLocations;
            _messageArgs        = messageArgs ?? EmptyMessageArgs;            
        }

        public Diagnostic WithLocation(Location location) {
            return new Diagnostic(location, Descriptor, _messageArgs);
        }

        static readonly object [] EmptyMessageArgs={};
        static readonly IReadOnlyList<Location> EmptyAdditionalLocations = Enumerable.Empty<Location>().ToImmutableList();

        [NotNull]
        public Location Location { get; }
        [NotNull]
        public IReadOnlyList<Location> AdditionalLocations { get; }

        public IEnumerable<Location> GetLocations() {
            yield return Location;
            foreach(var location in AdditionalLocations) {
                yield return location;
            }
        }

        public IEnumerable<Diagnostic> ExpandLocations() {
            return GetLocations().Select(WithLocation);            
        }       
        
        public DiagnosticDescriptor Descriptor { get; }
        public DiagnosticSeverity Severity => Descriptor.DefaultSeverity;
        public DiagnosticCategory Category => Descriptor.Category;
        public String Message              => FormatMessage();

        public override string ToString() {
            return DiagnosticFormatter.Default.Format(this);
        }

        string FormatMessage() {
            if (_messageArgs.Length != 0) {
                return String.Format(Descriptor.MessageFormat, _messageArgs);
            } else {
                return Descriptor.MessageFormat;
            }
        }
    }
}