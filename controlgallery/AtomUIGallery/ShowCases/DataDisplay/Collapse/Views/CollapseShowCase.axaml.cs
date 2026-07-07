using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseShowCase : GalleryReactiveUserControl<CollapseViewModel>
{
    public const string LanguageId = nameof(CollapseShowCase);

    public CollapseShowCase()
    {
        InitializeComponent();
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is CollapseViewModel viewModel)
        {
            viewModel.HandleExpandButtonPosOptionCheckedChanged(sender, args);
        }
    }
}
