using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.DatePicker;

public partial class DatePickerShowCase : GalleryReactiveUserControl<DatePickerViewModel>
{
    public const string LanguageId = nameof(DatePickerShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public DatePickerShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(_ =>
        {
            if (DataContext is DatePickerViewModel viewModel)
            {
                viewModel.PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
            }
        });
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
            ApiScenario         => new DatePickerApiDataGrid(),
            DesignTokenScenario => new DatePickerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown DatePicker scenario: {scenario}")
        };
    }

    private void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.HandlePickerSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void HandlePickerPlacementCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.HandlePickerPlacementCheckedChanged(sender, args);
        }
    }

    private void SetBoundSelectedDateTimeTomorrow(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.BoundSelectedDateTime = DateTime.Today.AddDays(1);
        }
    }

    private void ClearBoundSelectedDateTime(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.BoundSelectedDateTime = null;
        }
    }
}
