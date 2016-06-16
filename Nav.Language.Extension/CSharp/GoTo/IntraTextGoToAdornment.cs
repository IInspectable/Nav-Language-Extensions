#region Using Directives

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    sealed class IntraTextGoToAdornment : ButtonBase {

        readonly IWpfTextView _textView;
        readonly IWaitIndicator _waitIndicator;
        readonly CrispImage _crispImage;
        IntraTextGoToTag _goToTag;

        internal IntraTextGoToAdornment(IntraTextGoToTag goToTag, IWpfTextView textView, IWaitIndicator waitIndicator) {

            _textView   = textView;
            _waitIndicator = waitIndicator;
            _crispImage = new CrispImage();

            Width       = 20;
            Height      = 20;
            Background  = Brushes.Transparent;
            BorderBrush = Brushes.Transparent;
            Cursor      = Cursors.Hand;
            Margin      = new Thickness(0, 0, 0, 0);            
            Content     = _crispImage;

            Click += OnClick;
            
            Update(goToTag);
        }

        public IntraTextGoToTag GoToTag {
            get { return _goToTag; }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent) {
            base.OnVisualParentChanged(oldParent);
            UpdateColor();
        }

        async void OnClick(object sender, RoutedEventArgs e) {
            await NavLanguagePackage.GoToLocationInPreviewTabAsync(_waitIndicator, cancellationToken => _goToTag.GetLocationAsync(cancellationToken));
        }

        internal void Update(IntraTextGoToTag goToTag) {
            _goToTag            = goToTag;            
            ToolTip             = _goToTag.ToolTip;
            _crispImage.Moniker = _goToTag.ImageMoniker;          

            UpdateColor();
        }

        void UpdateColor() {

            var backgroundBrush = _textView.Background as SolidColorBrush;
            if (backgroundBrush != null) {
                ImageThemingUtilities.SetImageBackgroundColor(_crispImage, backgroundBrush.Color);
            }
        }
    }
}