using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class TourShowCase : ReactiveUserControl<TourViewModel>
{
    public TourShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TourViewModel vm)
            {
                vm.CustomGapRadius = 2;
            }
        });
        InitializeComponent();
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

public class SkipTourActionButton : Button, ITourAction
{
    static SkipTourActionButton()
    {
        Tour.StyleTypeProperty.AddOwner<SkipTourActionButton>();
        SizeTypeProperty.OverrideDefaultValue<SkipTourActionButton>(AtomUI.SizeType.Small);
        ButtonTypeProperty.OverrideDefaultValue<SkipTourActionButton>(ButtonType.Default);
    }

    public int StepCount { get; set; }
    public int ActiveIndex { get; set; }
    public TourStyleType StyleType { get; set; }
}

