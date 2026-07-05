
using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.TimePicker;

public partial class TimePickerShowCase : GalleryReactiveUserControl<TimePickerViewModel>
{
    public const string LanguageId = nameof(TimePickerShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public TimePickerShowCase()
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
            ApiScenario         => new TimePickerApiDataGrid(),
            DesignTokenScenario => new TimePickerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown TimePicker scenario: {scenario}")
        };
    }

    private void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.HandlePickerSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void SetBoundSelectedTimeToNoon(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundSelectedTime = new TimeSpan(12, 0, 0);
        }
    }

    private void ClearBoundSelectedTime(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundSelectedTime = null;
        }
    }
}
