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
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public ExpanderShowCase()
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
            ApiScenario         => new ExpanderApiDataGrid(),
            DesignTokenScenario => new ExpanderDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Expander scenario: {scenario}")
        };
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
