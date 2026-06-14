using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.NumberUpDown;

public partial class NumberUpDownShowCase : GalleryReactiveUserControl<NumberUpDownViewModel>
{
    public const string LanguageId = nameof(NumberUpDownShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public NumberUpDownShowCase()
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
            ApiScenario         => new NumberUpDownApiDataGrid(),
            DesignTokenScenario => new NumberUpDownDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown NumberUpDown scenario: {scenario}")
        };
    }
}
