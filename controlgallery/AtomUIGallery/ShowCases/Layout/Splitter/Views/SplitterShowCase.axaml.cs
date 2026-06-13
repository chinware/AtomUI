using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using RadioButton = AtomUI.Desktop.Controls.RadioButton;

namespace AtomUIGallery.ShowCases.Splitter;

public partial class SplitterShowCase : GalleryReactiveUserControl<SplitterViewModel>
{
    public const string LanguageId = nameof(SplitterShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public SplitterShowCase()
    {
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
            ApiScenario         => new SplitterApiDataGrid(),
            DesignTokenScenario => new SplitterDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Splitter scenario: {scenario}")
        };
    }

    private void HandleShowCollapsibleIconChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not AtomUIRadioButton { IsChecked: true } radioButton)
        {
            return;
        }

        var mode = ParseShowMode(radioButton.Tag);
        if (mode == null)
        {
            return;
        }

        UpdateShowCollapsibleIconMode(mode.Value);
    }

    private SplitterCollapsibleIconDisplayMode? ParseShowMode(object? tag)
    {
        if (tag is SplitterCollapsibleIconDisplayMode mode)
        {
            return mode;
        }

        if (tag is string text &&
            Enum.TryParse<SplitterCollapsibleIconDisplayMode>(text, true, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private void UpdateShowCollapsibleIconMode(SplitterCollapsibleIconDisplayMode mode)
    {
        foreach (var panel in ExamplesContent.GetVisualDescendants().OfType<Control>())
        {
            if (panel.Name is "ShowCollapsiblePanelFirst" or "ShowCollapsiblePanelSecond" or "ShowCollapsiblePanelThird")
            {
                ApplyShowMode(panel, mode);
            }
        }
    }

    private static void ApplyShowMode(Control panel, SplitterCollapsibleIconDisplayMode mode)
    {
        AtomUISplitter.SetCollapsible(panel, new SplitterPanelCollapsible()
        {
            IsEnabled           = true,
            ShowCollapsibleIcon = mode
        });
    }
}
