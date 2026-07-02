using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

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

    private void HandleTourExampleLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Control root)
        {
            return;
        }

        SetTourStepTarget(root, "BasicCaseUploadStep", "BasicCaseUpload");
        SetTourStepTarget(root, "BasicCaseSaveStep", "BasicCaseSave");
        SetTourStepTarget(root, "BasicCaseEllipsisStep", "BasicCaseEllipsis");
        SetTourStepTarget(root, "NonMaskCaseUploadStep", "NonMaskCaseUpload");
        SetTourStepTarget(root, "NonMaskSaveStep", "NonMaskSave");
        SetTourStepTarget(root, "NonMaskEllipsisStep", "NonMaskEllipsis");
        SetTourStepTarget(root, "PlacementRightStep", "PlacementBeginTour");
        SetTourStepTarget(root, "PlacementTopStep", "PlacementBeginTour");
        SetTourStepTarget(root, "PlacementLeftStep", "PlacementBeginTour");
        SetTourStepTarget(root, "CustomIndicatorUploadStep", "CustomIndicatorUpload");
        SetTourStepTarget(root, "CustomIndicatorSaveStep", "CustomIndicatorSave");
        SetTourStepTarget(root, "CustomIndicatorEllipsisStep", "CustomIndicatorEllipsis");
        SetTourStepTarget(root, "CustomMaskUploadStep", "CustomMaskUpload");
        SetTourStepTarget(root, "CustomMaskSaveStep", "CustomMaskSave");
        SetTourStepTarget(root, "CustomMaskEllipsisStep", "CustomMaskEllipsis");
        SetTourStepTarget(root, "CustomActionUploadStep", "CustomActionUpload");
        SetTourStepTarget(root, "CustomActionSaveStep", "CustomActionSave");
        SetTourStepTarget(root, "CustomActionEllipsisStep", "CustomActionEllipsis");
        SetTourStepTarget(root, "CustomGapStep", "CustomGapControl");
    }

    private static void SetTourStepTarget(Control root, string stepName, string targetName)
    {
        var step = FindDescendantByName<TourStep>(root, stepName);
        var target = FindDescendantByName<Control>(root, targetName);
        if (step is not null && target is not null)
        {
            step.Target = target;
        }
    }

    private static T? FindDescendantByName<T>(Control root, string name)
        where T : Control
    {
        if (root is T typedRoot && typedRoot.Name == name)
        {
            return typedRoot;
        }

        return root.GetVisualDescendants().OfType<T>().FirstOrDefault(control => control.Name == name)
               ?? root.GetLogicalDescendants().OfType<T>().FirstOrDefault(control => control.Name == name);
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
