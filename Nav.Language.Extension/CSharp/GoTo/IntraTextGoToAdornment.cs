#region Using Directives

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.GoToLocation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    sealed class IntraTextGoToAdornment: ButtonBase {

        readonly IWpfTextView        _textView;
        readonly GoToLocationService _goToLocationService;
        readonly CrispImage          _crispImage;

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

        public IntraTextGoToTag GoToTag { get; private set; }

        protected override void OnVisualParentChanged(DependencyObject oldParent) {
            base.OnVisualParentChanged(oldParent);
            UpdateColor();
        }

        void OnClick(object sender, RoutedEventArgs e) {

            ThreadHelper.JoinableTaskFactory.RunAsync(async () => {

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var transform          = TransformToAncestor(_textView.VisualElement);
                var placementRectangle = transform.TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));

                await _goToLocationService.GoToLocationInPreviewTabAsync(
                    _textView,
                    placementRectangle,
                    GoToTag.Provider);
            });
        }

        internal void Update(IntraTextGoToTag goToTag) {
            GoToTag             = goToTag;
            ToolTip             = GoToTag.ToolTip;
            _crispImage.Moniker = GoToTag.ImageMoniker;

            UpdateColor();
        }

        void UpdateColor() {
            if (_textView.Background is SolidColorBrush backgroundBrush) {
                ImageThemingUtilities.SetImageBackgroundColor(_crispImage, backgroundBrush.Color);
            }
        }

    }

}