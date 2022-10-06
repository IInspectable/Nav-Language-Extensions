using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace Pharmatechnik.Nav.Language.Extension.Common; 

sealed class TextBufferScopedClassifier : IClassifier, IDisposable {
    readonly TextBufferScopedValue<IClassifier> _textBufferScopedValue;
        
    internal TextBufferScopedClassifier(
        ITextBuffer textBuffer,
        object key,
        Func<IClassifier> createFunc) {
        _textBufferScopedValue = TextBufferScopedValue<IClassifier>.GetOrCreate(textBuffer, key, createFunc);
    }

    IClassifier Classifier {
        get { return _textBufferScopedValue.Value; }
    }

    public void Dispose() {
        _textBufferScopedValue.Dispose();
    }

    event EventHandler<ClassificationChangedEventArgs> IClassifier.ClassificationChanged {
        add { Classifier.ClassificationChanged    += value; }
        remove { Classifier.ClassificationChanged -= value; }
    }

    IList<ClassificationSpan> IClassifier.GetClassificationSpans(SnapshotSpan span) {
        return Classifier.GetClassificationSpans(span);
    }
}