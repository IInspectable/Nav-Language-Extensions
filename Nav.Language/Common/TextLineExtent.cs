using System;

namespace Pharmatechnik.Nav.Language {

    /// <summary>
    /// Represents an extent of a single line.
    /// </summary>
    [Serializable]
    public struct TextLineExtent: IExtent, IEquatable<TextLineExtent> {

        public TextLineExtent(int line, TextExtent extent) {

            if(extent.IsMissing && line != -1) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if (line < 0 && !extent.IsMissing) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if(line < -1) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if (line > extent.End) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            Line   = line;
            Extent = extent;
        }

        public static readonly TextLineExtent Missing = new TextLineExtent(-1, TextExtent.Missing);
        public static readonly TextLineExtent Empty   = new TextLineExtent(0, TextExtent.Empty);

        public bool IsMissing { get { return Line < 0; } }

        /// <summary>
        /// The line number. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The extent of the line.
        /// </summary>
        public TextExtent Extent { get; }
        
        int IExtent.Start {
            get { return Extent.Start; }
        }

        int IExtent.End {
            get { return Extent.End; }
        }
        
        /// <summary>
        /// Determines whether two <see cref="TextLineExtent"/> are the same.
        /// </summary>
        public static bool operator ==(TextLineExtent left, TextLineExtent right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are different.
        /// </summary>
        public static bool operator !=(TextLineExtent left, TextLineExtent right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TextLineExtent"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(TextLineExtent other) {
            return other.Line == Line && other.Extent == Extent;
        }

        /// <summary>
        /// Determines whether two <see cref="TextLineExtent"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is TextLineExtent extent && Equals(extent);
        }

        /// <summary>
        /// Provides a hash function for <see cref="TextLineExtent"/>.
        /// </summary>
        public override int GetHashCode() {
            return Line ^ Extent.GetHashCode();
        }
    }
}