#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Outlining {
    
    interface IOutliningRegionTagCreator {
        IOutliningRegionTag CreateTag(object collapsed, SnapshotSpan span);
    }

    sealed class OutliningTagger: ParserServiceDependent, ITagger<IOutliningRegionTag>, IOutliningRegionTagCreator {

        readonly List<ITagSpan<IOutliningRegionTag>>  _outLineTags;
        readonly CodeContentControlProvider           _codeContentControlProvider;

        OutliningTagger(ITextBuffer textBuffer, CodeContentControlProvider codeContentControlProvider): base(textBuffer) {

            _outLineTags                = new List<ITagSpan<IOutliningRegionTag>>();
            _codeContentControlProvider = codeContentControlProvider;
        }
        
        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer, CodeContentControlProvider codeContentControlProvider) where T : ITag {
            return new TextBufferScopedTagger<T>(
                textBuffer,
                typeof(OutliningTagger), () =>
                   new OutliningTagger(textBuffer, codeContentControlProvider) as ITagger<T>);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IOutliningRegionTag CreateTag(object collapsed, SnapshotSpan span) {
            return new OutliningRegionTag(false, false, collapsed, _codeContentControlProvider.CreateContentControlForOutlining(span));
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            return _outLineTags;
        }
        
        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            var parseResult = ParserService.ParseResult;
            if (parseResult == null) {
                return;
            }

            UpdateRegions(parseResult);
            
            TagsChanged?.Invoke(this, e);
        }

        void UpdateRegions(ParseResult parseResult) {
            _outLineTags.Clear();            
            //_outLineTags.AddRange(CodeNamespaceDeclarationOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(CodeUsingDirectiveOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(TaskReferenceOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(TaskDefinitionsOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(NodeDeclarationBlockOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(TransitionDefinitionBlockOutlineTagger.GetTags(parseResult, this));
            _outLineTags.AddRange(MultilineCommentOutlineTagger.GetTags(parseResult, this));
        }        
    }
}