using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;

namespace AtomUIGallery.ShowCases.ColorPicker;

public partial class ColorPickerShowCase : GalleryReactiveUserControl<ColorPickerViewModel>
{
    public const string LanguageId = nameof(ColorPickerShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public ColorPickerShowCase()
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
            ApiScenario         => new ColorPickerApiDataGrid(),
            DesignTokenScenario => new ColorPickerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown ColorPicker scenario: {scenario}")
        };
    }

    private void HandleCustomRenderTextAttached(object? sender, VisualTreeAttachmentEventArgs args)
    {
        if (sender is AtomUI.Desktop.Controls.ColorPicker colorPicker)
        {
            AtomUIColorPicker.SetColorTextFormatter(colorPicker, (color, _) =>
            {
                var colorText = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
                return $"Custom Text ({colorText})";
            });
        }
    }
}
