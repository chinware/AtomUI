using System;
using System.Collections.Generic;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public partial class ToggleSwitchShowCase : GalleryReactiveUserControl<ToggleSwitchViewModel>
{
    public const string LanguageId = nameof(ToggleSwitchShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public ReactiveCommand<Unit, Unit> ToggleSwitchCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ToggleLoadingStatus { get; private set; }

    public ToggleSwitchShowCase()
    {
        ToggleSwitchCommand = ReactiveCommand.Create(HandleToggleDisabledStatus);
        ToggleLoadingStatus = ReactiveCommand.Create(HandleToggleLoadingStatus);
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not TabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
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

    private void HandleToggleDisabledStatus()
    {
        if (ToggleDisabledSwitch != null)
        {
            ToggleDisabledSwitch.IsEnabled = !ToggleDisabledSwitch.IsEnabled;
        }
    }

    private void HandleToggleLoadingStatus()
    {
        if (ToggleSwitchDefault != null)
        {
            ToggleSwitchDefault.IsLoading = !ToggleSwitchDefault.IsLoading;
        }

        if (ToggleSwitchSmall != null)
        {
            ToggleSwitchSmall.IsLoading = !ToggleSwitchSmall.IsLoading;
        }
    }
}
