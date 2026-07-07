using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public partial class ToggleSwitchShowCase : GalleryReactiveUserControl<ToggleSwitchViewModel>
{
    public const string LanguageId = nameof(ToggleSwitchShowCase);

    public ToggleSwitchShowCase()
    {
        InitializeComponent();
    }

    public void HandleToggleDisabledButtonClick(object? sender, RoutedEventArgs args)
    {
        if (sender is Control { DataContext: ToggleSwitchViewModel viewModel })
        {
            viewModel.IsDisabledDemoEnabled = !viewModel.IsDisabledDemoEnabled;
        }
    }

    public void HandleToggleLoadingButtonClick(object? sender, RoutedEventArgs args)
    {
        if (sender is Control { DataContext: ToggleSwitchViewModel viewModel })
        {
            viewModel.IsLoadingDemoLoading = !viewModel.IsLoadingDemoLoading;
        }
    }

}
