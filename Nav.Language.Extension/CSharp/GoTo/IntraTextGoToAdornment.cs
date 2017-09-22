#region Using Directives

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    sealed class IntraTextGoToAdornment : ButtonBase {

        readonly IWpfTextView _textView;
        readonly GoToLocationService _goToLocationService;
        readonly CrispImage _crispImage;

        IntraTextGoToTag _goToTag;

        internal IntraTextGoToAdornment(IntraTextGoToTag goToTag, IWpfTextView textView, GoToLocationService goToLocationService) {

            _textView            = textView;
            _goToLocationService = goToLocationService;
            _crispImage          = new CrispImage();

            RenderOptions.SetBitmapScalingMode(_crispImage, BitmapScalingMode.NearestNeighbor);

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

            var transform          = TransformToAncestor(_textView.VisualElement);
            var placementRectangle = transform.TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));

            await _goToLocationService.GoToLocationInPreviewTabAsync(
                _textView, 
                placementRectangle,
                _goToTag.Provider);
        }

        internal void Update(IntraTextGoToTag goToTag) {
            _goToTag            = goToTag;            
            ToolTip             = _goToTag.ToolTip;
            _crispImage.Moniker = _goToTag.ImageMoniker;          

            UpdateColor();
        }

        void UpdateColor() {
            if (_textView.Background is SolidColorBrush backgroundBrush) {
                ImageThemingUtilities.SetImageBackgroundColor(_crispImage, backgroundBrush.Color);
            }
        }
    }
}