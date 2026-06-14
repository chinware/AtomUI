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
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public SplitterShowCase()
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
