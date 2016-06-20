#region Using Directives

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.UI {

    class VsContextMenu: ContextMenu {

        static VsContextMenu() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VsContextMenu), new FrameworkPropertyMetadata(typeof(VsContextMenu)));
            
        }

        public VsContextMenu() {
            SetResourceReference(StyleProperty, typeof(VsContextMenu));
        }
    }
}
