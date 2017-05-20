#region Using Directives

using System;
using System.Text;
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

        public DiagnosticSeverity Severity {
            get { return Descriptor.DefaultSeverity; }
        }

        public DiagnosticCategory Category {
            get { return Descriptor.Category; }
        }

        [NotNull]
        public String Message {
            get { return FormatMessage(); }
        }
        
        public override string ToString() {

            StringBuilder sb = new StringBuilder();
           
            sb.Append($"{FormatFilePath()}");
            sb.Append($"({FormatLocation()})");

            sb.Append($": {FormatSeverity()} {FormatId()}");

            sb.Append($": {Message}");

            return sb.ToString();
        }

        string FormatFilePath() {
            return Location.FilePath ??String.Empty;
        }

        string FormatLocation() {

            StringBuilder sb=new StringBuilder();

            sb.Append($"{Location.StartLine + 1},{Location.StartCharacter + 1}");

            if (Location.StartLine      != Location.EndLine ||
                Location.StartCharacter != Location.EndCharacter) {

                sb.Append($",{Location.EndLine + 1},{Location.EndCharacter + 1}");
            }

            return sb.ToString();
        }

        string FormatSeverity() {
            return Severity.ToString().ToLower();
        }

        string FormatId() {
            return Descriptor.Id;
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