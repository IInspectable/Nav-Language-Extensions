#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("csharp")]
    [TagType(typeof(IntraTextAdornmentTag))]
    sealed class IntraTextGoToAdornmentTaggerProvider : IViewTaggerProvider {

        readonly IBufferTagAggregatorFactoryService _bufferTagAggregatorFactoryService;

        [ImportingConstructor]
        public IntraTextGoToAdornmentTaggerProvider(IBufferTagAggregatorFactoryService bufferTagAggregatorFactoryService) {
            _bufferTagAggregatorFactoryService = bufferTagAggregatorFactoryService;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if(textView == null)
                throw new ArgumentNullException(nameof(textView));

            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(buffer != textView.TextBuffer)
                return null;

            return IntraTextGoToAdornmentTagger.GetTagger(
                (IWpfTextView) textView,
                new Lazy<ITagAggregator<IntraTextGoToTag>>(
                    () => _bufferTagAggregatorFactoryService.CreateTagAggregator<IntraTextGoToTag>(textView.TextBuffer)))
                as ITagger<T>;
        }
    }
}