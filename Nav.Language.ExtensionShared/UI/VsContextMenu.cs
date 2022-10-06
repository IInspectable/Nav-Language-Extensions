#region Using Directives

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.UI; 

class VsContextMenu : ContextMenu {

    static VsContextMenu() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsContextMenu), new FrameworkPropertyMetadata(typeof(VsContextMenu)));

    }

    #region DependencyProperty Header

    /// <summary>
    /// Registers a dependency property as backing store for the Header property
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(VsContextMenu),
                                    new FrameworkPropertyMetadata(null,
                                                                  FrameworkPropertyMetadataOptions.AffectsRender |
                                                                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    /// <summary>
    /// Gets or sets the Header.
    /// </summary>
    /// <value>The Header.</value>
    public object Header {
        get { return GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    #endregion
}