#region Using Directives

using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.Underlining;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    sealed class GoToDefinitionMouseProcessor: MouseProcessorBase {

        readonly IWpfTextView _textView;
        readonly ITagAggregator<GoToDefinitionTag> _tagAggregator;
        readonly ModifierKeyState _keyState;

        Cursor _overriddenCursor;

        [CanBeNull]
        ITagSpan<GoToDefinitionTag> _navigateToTagSpan;

        GoToDefinitionMouseProcessor(IWpfTextView textView, TextViewConnectionListener textViewConnectionListener, IViewTagAggregatorFactoryService viewTagAggregatorFactoryService) {
            _textView      = textView;
            _tagAggregator = viewTagAggregatorFactoryService.CreateTagAggregator<GoToDefinitionTag>(textView);
            _keyState      = ModifierKeyState.GetStateForView(textView, textViewConnectionListener);

            _textView.LostAggregateFocus += OnTextViewLostAggregateFocus; 
            _keyState.KeyStateChanged    += OnKeyStateChanged;

            textViewConnectionListener.AddDisconnectAction(textView, RemoveMouseProcessorForView);
        }

        public static GoToDefinitionMouseProcessor GetMouseProcessorForView(IWpfTextView textView, TextViewConnectionListener textViewConnectionListener,
                                                                        IViewTagAggregatorFactoryService viewTagAggregatorFactoryService) {

            return textView.Properties.GetOrCreateSingletonProperty(() => new GoToDefinitionMouseProcessor(textView, textViewConnectionListener, viewTagAggregatorFactoryService));
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


        void UpdateNavigateToTagSpan(ITagSpan<GoToDefinitionTag> navigateToTagSpan) {

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

            var location = _navigateToTagSpan.Tag.Location;

            NavLanguagePackage.GoToLocationInPreviewTab(location);

            _navigateToTagSpan = null;
        }
    }
}