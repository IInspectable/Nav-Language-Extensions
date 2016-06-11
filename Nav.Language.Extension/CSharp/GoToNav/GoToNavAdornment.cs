#region Using Directives

using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav
{

    sealed class GoToNavAdornment : Button {

        readonly GoToNavTag _gotoNavTag;
        readonly IWpfTextView _textView;
        readonly CrispImage _crispImage;

        internal GoToNavAdornment(GoToNavTag gotoNavTag, IWpfTextView textView) {
            _gotoNavTag = gotoNavTag;
            _textView = textView;

            Width       = 20;
            Height      = 20;
            Background  = Brushes.Transparent;
            BorderBrush = Brushes.Transparent;
            Cursor      = Cursors.Hand;
            Margin      = new Thickness(0, 0, 0, 0);
            ToolTip     = _gotoNavTag.TaskInfo.NavFileName;

            _crispImage = new CrispImage {
                //Moniker = KnownMonikers.GoToSourceCode
                Moniker = KnownMonikers.GoToDeclaration
            };
 
            UpdateColor();
            Content = _crispImage;

            Click += ColorAdornment_Click;
        }

        public GoToNavTag GotoNavTag {
            get { return _gotoNavTag; }
        }

        void UpdateColor() {

            var backgroundBrush = _textView.Background as SolidColorBrush;
            if(backgroundBrush != null) {
                ImageThemingUtilities.SetImageBackgroundColor(_crispImage, backgroundBrush.Color);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent) {
            base.OnVisualParentChanged(oldParent);
            UpdateColor();
        }

        void ColorAdornment_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Hi 5!");
        }
        
        internal void Update(GoToNavTag goToNavTag)
        {
            
        }
    }
}