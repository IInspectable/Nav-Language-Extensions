#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public class Location : IEquatable<Location> {

        readonly TextExtent _extent;
        readonly LinePosition _start;
        readonly LinePosition _end;
        readonly string _filePath;

        protected Location(Location location) {
            _extent   = location._extent;
            _start    = location._start;
            _end      = location._end;
            _filePath = location._filePath;
        }

        public Location(TextExtent extent, LinePositionExtent linePositionExtent, [CanBeNull] string filePath) {
            _extent      = extent;
            _start       = linePositionExtent.Start;
            _end         = linePositionExtent.End;
            _filePath    = filePath;
        }
        
        public Location(TextExtent extent, LinePosition linePosition, [CanBeNull] string filePath): 
            this(extent, new LinePositionExtent(linePosition, linePosition), filePath) {
        }

        public Location(string filePath) {
            _extent   = TextExtent.Empty;
            _start    = LinePosition.Empty;
            _end      = LinePosition.Empty;
            _filePath = filePath;
        }

        //TODO Missing/None

        public TextExtent Extent {
            get { return _extent; }
        }

        public LinePosition StartLinePosition {
            get { return _start; }
        }

        public LinePosition EndLinePosition {
            get { return _end; }
        }

        public LinePositionExtent LineExtent {
            get { return new LinePositionExtent(_start, _end); }
        }

        /// <summary>
        /// The path to the file or null.
        /// </summary>
        [CanBeNull]
        public string FilePath { get { return _filePath; } }

        /// <summary>
        /// Gets the starting index of the location [0..n].
        /// -1 if the is unknown/missing
        /// </summary>
        public int Start { get { return _extent.Start; } }
        /// <summary>
        /// The start line number of the location. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int StartLine { get { return _start.Line; } }
        /// <summary>
        /// The character position within the starting line (zero based).
        /// </summary>
        public int StartCharacter { get { return _start.Character; } }
        /// <summary>
        /// Gets the length of the location. Length is guaranteed to be great or equal to 0.
        /// </summary>
        public int Length { get { return _extent.Length; } }
        /// <summary>
        /// Gets the end index of the location, starting with 0;
        /// This index is actually one character past the end of the location.
        /// </summary>
        public int End { get { return _extent.End; } }
        /// <summary>
        /// The end line number of the location. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int EndLine { get { return _end.Line; } }
        /// <summary>
        /// The character position within the end line (zero based).
        /// </summary>
        public int EndCharacter { get { return _end.Character; } }
        
        public override string ToString() {
            return $"{_filePath}@{StartLine+1}:{StartCharacter+1}";
        }

        #region Equality members

        public bool Equals(Location other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return _extent.Equals(other._extent) &&
                   _start.Equals(other._start) &&
                   _end.Equals(other._end) &&
                   string.Equals(_filePath, other._filePath);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj is Location location && Equals(location);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = _extent.GetHashCode();
                hashCode = (hashCode * 397) ^ _start.GetHashCode();
                hashCode = (hashCode * 397) ^ _end.GetHashCode();
                hashCode = (hashCode * 397) ^ (_filePath?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Location left, Location right) {
            return Equals(left, right);
        }

        public static bool operator !=(Location left, Location right) {
            return !Equals(left, right);
        }

        #endregion    
    }
}
