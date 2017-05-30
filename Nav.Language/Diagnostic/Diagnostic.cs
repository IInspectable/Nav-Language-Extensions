#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    
    [Serializable]
    public sealed class Diagnostic {
        
        [NotNull]
        readonly object[] _messageArgs;
        
        public Diagnostic(Location location, DiagnosticDescriptor descriptor, params object[] messageArgs) {
            Location     = location   ?? throw new ArgumentNullException(nameof(location));
            Descriptor   = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _messageArgs = messageArgs?? EmptyMessageArgs;
        }

        static readonly object [] EmptyMessageArgs={};

        [NotNull]
        public Location Location { get; }

        [NotNull]
        public DiagnosticDescriptor Descriptor { get; }

        public DiagnosticSeverity Severity => Descriptor.DefaultSeverity;

        public DiagnosticCategory Category => Descriptor.Category;

        [NotNull]
        public String Message => FormatMessage();

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