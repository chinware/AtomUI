using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Expander;

public partial class ExpanderShowCase : GalleryReactiveUserControl<ExpanderViewModel>
{
    public const string LanguageId = nameof(ExpanderShowCase);

    public ExpanderShowCase()
    {
        InitializeComponent();
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is ExpanderViewModel viewModel)
        {
            viewModel.HandleExpandButtonPosOptionCheckedChanged(sender, args);
        }
    }

    private void HandleExpandDirectionOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is ExpanderViewModel viewModel)
        {
            viewModel.HandleExpandDirectionOptionCheckedChanged(sender, args);
        }
    }
}
