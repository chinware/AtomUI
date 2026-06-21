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
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public ToggleSwitchShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
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

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new ToggleSwitchApiDataGrid(),
            DesignTokenScenario => new ToggleSwitchDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown ToggleSwitch scenario: {scenario}")
        };
    }

}
