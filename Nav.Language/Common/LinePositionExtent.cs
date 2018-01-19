using System;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public struct LinePositionExtent: IEquatable<LinePositionExtent> {

        public LinePositionExtent(LinePosition start, LinePosition end) {

            if (start > end) {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            Start = start;
            End   = end;
        }

        public LinePosition Start { get; }
        public LinePosition End   { get; }

        /// <summary>
        /// Determines whether two <see cref="LinePositionExtent"/> are the same.
        /// </summary>
        public static bool operator ==(LinePositionExtent left, LinePositionExtent right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LinePositionExtent"/> are different.
        /// </summary>
        public static bool operator !=(LinePositionExtent left, LinePositionExtent right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LinePositionExtent"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(LinePositionExtent other) {
            return other.Start == Start && other.End == End;
        }

        /// <summary>
        /// Determines whether two <see cref="LinePositionExtent"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is LinePositionExtent extent && Equals(extent);
        }

        /// <summary>
        /// Provides a hash function for <see cref="LinePositionExtent"/>.
        /// </summary>
        public override int GetHashCode() {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

    }

}