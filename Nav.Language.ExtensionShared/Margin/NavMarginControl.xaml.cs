#region Using Directives

using System;
using System.Windows;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Controls;

using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.Commands;
using Pharmatechnik.Nav.Language.Extension;

#endregion

namespace Pharmatechnik.Language.Nav.Extension.Margin;

public partial class NavMarginControl {

    private readonly IWpfTextView _textView;

    public NavMarginControl(IWpfTextView textView) {
        _textView = textView;
        InitializeComponent();

        UpdateTooltips();

        var buttonStyle = (Style)textView.VisualElement.TryFindResource("FileHealthIndicatorButtonStyle");
        if (buttonStyle != null) {
            foreach (var button in LayoutPanel.Children.OfType<Button>()) {
                button.Style = buttonStyle;
            }
        }

    }

    void UpdateTooltips() {
        NavPreviewButton.ToolTip  = GetTooltipText(KnownCommandIds.ViewCode,           "View Code");
        GenerateNavButton.ToolTip = GetTooltipText(KnownCommandIds.NavGenerateCommand, "C# Code aus .nav-Dateien generieren");
    }

    private void OnGenerateNavButtonClick(object sender, RoutedEventArgs e) {
        ThreadHelper.ThrowIfNotOnUIThread();

        _textView.VisualElement.Focus();
        NavLanguagePackage.InvokeCommand(KnownCommandIds.NavGenerateCommand);
        UpdateTooltips();
    }

    private void OnNavPreviewClick(object sender, RoutedEventArgs e) {
        ThreadHelper.ThrowIfNotOnUIThread();

        _textView.VisualElement.Focus();
        NavLanguagePackage.InvokeCommand(KnownCommandIds.ViewCode);
        UpdateTooltips();
    }

    static string GetTooltipText(CommandID commandId, string commandName) {

        var tooltipText = commandName;

        var keyBinding = KeyBindingHelper.GetGlobalKeyBinding(commandId.Guid, commandId.ID);
        if (!String.IsNullOrEmpty(keyBinding)) {
            tooltipText += $" ({keyBinding})";
        }

        return tooltipText;
    }

}