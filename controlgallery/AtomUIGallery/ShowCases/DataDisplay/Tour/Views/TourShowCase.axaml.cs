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
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public TourShowCase()
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
        SizeTypeProperty.OverrideDefaultValue<SkipTourActionButton>(AtomUI.SizeType.Small);
        ButtonTypeProperty.OverrideDefaultValue<SkipTourActionButton>(ButtonType.Default);
    }

    public int StepCount { get; set; }
    public int ActiveIndex { get; set; }
    public TourStyleType StyleType { get; set; }
}
