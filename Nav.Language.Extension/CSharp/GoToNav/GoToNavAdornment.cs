#region Using Directives

using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    sealed class GoToNavAdornment : Button {

        readonly IWpfTextView _textView;
        readonly CrispImage _crispImage;
        GoToNavTag _gotoNavTag;

        internal GoToNavAdornment(GoToNavTag goToNavTag, IWpfTextView textView) {

            _textView   = textView;
            _crispImage = new CrispImage();

            Width       = 20;
            Height      = 20;
            Background  = Brushes.Transparent;
            BorderBrush = Brushes.Transparent;
            Cursor      = Cursors.Hand;
            Margin      = new Thickness(0, 0, 0, 0);            
            Content     = _crispImage;

            Click += ColorAdornment_Click;
            
            Update(goToNavTag);
        }

        public GoToNavTag GotoNavTag {
            get { return _gotoNavTag; }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent) {
            base.OnVisualParentChanged(oldParent);
            UpdateColor();
        }

        async void ColorAdornment_Click(object sender, RoutedEventArgs e) {

            var location = await _gotoNavTag.GetLocationAsync();

            if(location != null) {                
                NavLanguagePackage.GoToLocationInPreviewTab(location);
            }
        }

        internal void Update(GoToNavTag goToNavTag) {
            _gotoNavTag = goToNavTag;
            
            UpdateColor();

            if (_gotoNavTag.TaskInfo is NavTriggerInfo) {
                _crispImage.Moniker = SymbolImageMonikers.SignalTrigger;
                ToolTip = "Go To Trigger Definition";
            } else {
                _crispImage.Moniker = SymbolImageMonikers.TaskDefinition;
                ToolTip = "Go To Task Definition";
            }
        }

        void UpdateColor() {

            var backgroundBrush = _textView.Background as SolidColorBrush;
            if (backgroundBrush != null) {
                ImageThemingUtilities.SetImageBackgroundColor(_crispImage, backgroundBrush.Color);
            }
        }
    }
}