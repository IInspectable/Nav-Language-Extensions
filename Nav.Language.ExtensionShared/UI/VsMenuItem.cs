#region Using Directives

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.UI {

    class VsMenuItem : MenuItem {

        static VsMenuItem() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VsMenuItem), new FrameworkPropertyMetadata(typeof(VsMenuItem)));
        }
    }
}