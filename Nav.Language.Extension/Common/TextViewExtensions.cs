#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

     static partial class TextViewExtensions {

        public static ISet<IContentType> GetContentTypes(this ITextView textView) {
            return new HashSet<IContentType>(
                textView.BufferGraph.GetTextBuffers(_ => true).Select(b => b.ContentType));
        }

        public static ITextBuffer GetBufferContainingCaret(this ITextView textView, string contentType = NavLanguageContentDefinitions.ContentType) {
            var point = GetCaretPoint(textView, s => s.ContentType.IsOfType(contentType));
            return point?.Snapshot.TextBuffer;
        }

        public static SnapshotPoint? GetCaretPoint(this ITextView textView, Predicate<ITextSnapshot> match) {
            var caret = textView.Caret.Position;
            var span = textView.BufferGraph.MapUpOrDownToFirstMatch(new SnapshotSpan(caret.BufferPosition, 0), match);
            return span?.Start;
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static TProperty GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            Func<TTextView, TProperty> valueCreator) where TTextView : ITextView {
            return textView.GetOrCreateAutoClosingProperty(typeof(TProperty), valueCreator);
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static TProperty GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            object key,
            Func<TTextView, TProperty> valueCreator) where TTextView : ITextView {
            GetOrCreateAutoClosingProperty(textView, key, valueCreator, out var value);
            return value;
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static bool GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            object key,
            Func<TTextView, TProperty> valueCreator,
            out TProperty value) where TTextView : ITextView {
            return AutoClosingViewProperty<TProperty, TTextView>.GetOrCreateValue(textView, key, valueCreator, out value);
        }
    }   
}