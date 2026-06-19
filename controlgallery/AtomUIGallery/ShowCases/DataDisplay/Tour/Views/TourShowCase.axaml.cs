using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Tour;

public partial class TourShowCase : GalleryReactiveUserControl<TourViewModel>
{
    public const string LanguageId = nameof(TourShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public TourShowCase()
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
            ApiScenario         => new TourApiDataGrid(),
            DesignTokenScenario => new TourDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Tour scenario: {scenario}")
        };
    }

    private void HandleBasicBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.BasicCaseTourOpened = true;
        }
    }

    private void HandleNonMaskBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.NonMaskTourOpened = true;
        }
    }

    private void HandlePlacementBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.PlacementTourOpened = true;
        }
    }

    private void HandleCustomIndicatorBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.CustomIndicatorTourOpened = true;
        }
    }

    private void HandleCustomMaskBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.CustomMaskTourOpened = true;
        }
    }

    private void HandleCustomGapBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.CustomGapTourOpened = true;
        }
    }

    private void HandleCustomActionBeginTour(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is TourViewModel vm)
        {
            vm.CustomActionTourOpened = true;
        }
    }
}

public class SkipTourActionButton : AtomUIButton, ITourAction
{
    static SkipTourActionButton()
    {
        AtomUITour.StyleTypeProperty.AddOwner<SkipTourActionButton>();
        SizeTypeProperty.OverrideDefaultValue<SkipTourActionButton>(AtomUI.CustomizableSizeType.Small);
        ButtonTypeProperty.OverrideDefaultValue<SkipTourActionButton>(ButtonType.Default);
    }

    public int StepCount { get; set; }
    public int ActiveIndex { get; set; }
    public TourStyleType StyleType { get; set; }
}
