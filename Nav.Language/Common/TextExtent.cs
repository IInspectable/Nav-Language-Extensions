﻿using System;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public struct TextExtent: IExtent, IEquatable<TextExtent> {

        readonly int _start;
        readonly int _length;

        public TextExtent(int start, int length) {

            if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            if(start < -1) {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            if(start==-1 && length > 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            _start  = start;
            _length = length;
        }

        public static readonly TextExtent Empty   = new TextExtent(start:  0, length: 0);
        public static readonly TextExtent Missing = new TextExtent(start: -1, length: 0);

        public static TextExtent FromBounds(int start, int end) {
            return new TextExtent(start: start, length: end - start);
        }

        public bool IsMissing {
            get { return _start<0; }
        }

        public bool IsEmpty {
            get { return Length==0; }
        }

        public bool IsEmptyOrMissing {
            get { return IsEmpty || IsMissing; }
        }

        /// <summary>
        /// Gets the starting index of the extent [0..n].
        /// -1 if the is unknown/missing
        /// </summary>
        public int Start {
            get { return _start; }
        }

        /// <summary>
        /// Gets the length of the extent. Length is guaranteed to be great or equal to 0.
        /// </summary>
        public int Length {
            get { return _length; }
        }

        /// <summary>
        /// Gets the end index of the extent, starting with 0;
        /// This index is actually one character past the end of the extent.
        /// </summary>
        public int End {
            get { return _start+_length; }
        }
        
        public bool Contains(TextExtent other) {
            return other.Start >= Start && other.End <= End;
        }

        /// <summary>
        /// Determines whether <paramref name="extent"/> intersects this extent. Two extents are considered to 
        /// intersect if they have positions in common or the end of one extent 
        /// coincides with the start of the other extent.
        /// </summary>       
        public bool IntersectsWith(TextExtent extent) {
            return extent.Start <= End && extent.End >= Start;
        }

        /// <summary>
        /// Returns the intersection with the given extent, or null if there is no intersection.
        /// </summary>
        public TextExtent? Intersection(TextExtent span) {
            int intersectStart = Math.Max(Start, span.Start);
            int intersectEnd   = Math.Min(End, span.End);

            return intersectStart <= intersectEnd ? FromBounds(intersectStart, intersectEnd) : (TextExtent?)null;
        }

        public override string ToString() {
            if (IsMissing) {
                return "<missing>";
            }
            return $"[{Start}-{End}]";
        }
        
        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are the same.
        /// </summary>
        public static bool operator ==(TextExtent left, TextExtent right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are different.
        /// </summary>
        public static bool operator !=(TextExtent left, TextExtent right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(TextExtent other) {
            return other.Start == Start && other.End == End;
        }

        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is TextExtent extent && Equals(extent);
        }

        /// <summary>
        /// Provides a hash function for <see cref="TextExtent"/>.
        /// </summary>
        public override int GetHashCode() {
            return Start^End;
        }        
    }
} 
