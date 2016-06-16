#region Using Directives

using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.Underlining;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    sealed class GoToMouseProcessor: MouseProcessorBase {

        readonly IWpfTextView _textView;
        readonly IWaitIndicator _waitIndicator;
        readonly ITagAggregator<GoToTag> _tagAggregator;
        readonly ModifierKeyState _keyState;

        Cursor _overriddenCursor;

        [CanBeNull]
        ITagSpan<GoToTag> _navigateToTagSpan;

        GoToMouseProcessor(IWpfTextView textView, 
                           TextViewConnectionListener textViewConnectionListener, 
                           IViewTagAggregatorFactoryService viewTagAggregatorFactoryService, 
                           IWaitIndicator waitIndicator) {
            _textView      = textView;
            _waitIndicator = waitIndicator;
            _tagAggregator = viewTagAggregatorFactoryService.CreateTagAggregator<GoToTag>(textView);
            _keyState      = ModifierKeyState.GetStateForView(textView, textViewConnectionListener);

            _textView.LostAggregateFocus += OnTextViewLostAggregateFocus; 
            _keyState.KeyStateChanged    += OnKeyStateChanged;

            textViewConnectionListener.AddDisconnectAction(textView, RemoveMouseProcessorForView);
        }

        public static GoToMouseProcessor GetMouseProcessorForView(IWpfTextView textView, 
                                                                  TextViewConnectionListener textViewConnectionListener,
                                                                  IViewTagAggregatorFactoryService viewTagAggregatorFactoryService,
                                                                  IWaitIndicator waitIndicator) {

            return textView.Properties.GetOrCreateSingletonProperty(() => new GoToMouseProcessor(textView, textViewConnectionListener, viewTagAggregatorFactoryService, waitIndicator));
        }

        void RemoveMouseProcessorForView(IWpfTextView textView) {
            textView.Properties.RemoveProperty(GetType());
            _tagAggregator.Dispose();
            _textView.LostAggregateFocus -= OnTextViewLostAggregateFocus;
            _keyState.KeyStateChanged    -= OnKeyStateChanged;
        }

        public override void PostprocessMouseMove(MouseEventArgs e) {
            UpdateNavigateToTagSpan();
        }
        
        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e) {
            NavigateToTagSpan();
        }
        
        void OnTextViewLostAggregateFocus(object sender, EventArgs e) {
            RemoveNavigateToTagSpan();
        }

        void OnKeyStateChanged(object sender, EventArgs e) {
            UpdateNavigateToTagSpan();
        }

        void UpdateNavigateToTagSpan() {

            if(!_keyState.IsOnlyModifierKeyControlPressed) {
                RemoveNavigateToTagSpan();
                return;
            }

            var navigateToTagSpan = _textView.GetGoToDefinitionTagSpanAtMousePosition(_tagAggregator);

            if(navigateToTagSpan!=null) {                
                UpdateNavigateToTagSpan(navigateToTagSpan);
            } else {
                RemoveNavigateToTagSpan();
            }
        }

        void UpdateNavigateToTagSpan(ITagSpan<GoToTag> navigateToTagSpan) {

            if(navigateToTagSpan.Span == _navigateToTagSpan?.Span &&
               navigateToTagSpan.Tag  == _navigateToTagSpan?.Tag) {
                return;
            }

            RemoveNavigateToTagSpan();

            _navigateToTagSpan = navigateToTagSpan;
            UnderlineTagger.GetOrCreateSingelton(_textView.TextBuffer)?.AddUnderlineSpan(navigateToTagSpan.Span);

            _overriddenCursor = _textView.VisualElement.Cursor;
            _textView.VisualElement.Cursor = Cursors.Hand;
        }

        void RemoveNavigateToTagSpan() {

            if (_navigateToTagSpan == null) {
                return;
            }

            UnderlineTagger.GetOrCreateSingelton(_textView.TextBuffer)?.RemoveUnderlineSpan(_navigateToTagSpan.Span);
            _navigateToTagSpan = null;

            _textView.VisualElement.Cursor = _overriddenCursor;
        }

        void NavigateToTagSpan() {

            if (_navigateToTagSpan == null) {
                return;
            }

            _textView.Selection.Clear();

            var span = _navigateToTagSpan;
            RemoveNavigateToTagSpan();

            NavLanguagePackage.GoToLocationInPreviewTabWithWaitIndicator(_waitIndicator, cancellationToken => span.Tag.GetLocationAsync(cancellationToken));           
        }
    }
}