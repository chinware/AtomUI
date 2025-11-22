using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DrawerShowCase : ReactiveUserControl<DrawerViewModel>
{
    public static readonly IValueConverter PlacementTextConverter =
        new FuncValueConverter<object?, object?>(x =>
        {
            if (x is int intValue)
            {
                var placement = (DrawerPlacement)intValue;
                return placement.ToString();
            }

            return x;
        });

    public DrawerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is DrawerViewModel viewModel)
            {
            }
        });
        InitializeComponent();
    }

    private void HandleOpenLargeSizeDrawer(object sender, RoutedEventArgs e)
    {
        PresetSizeDrawer.SizeType = CustomizableSizeType.Large;
        PresetSizeDrawer.IsOpen   = true;
    }
    
    private void HandleOpenCustomSizeDrawer(object sender, RoutedEventArgs e)
    {
        PresetSizeDrawer.SizeType   = CustomizableSizeType.Custom;
        PresetSizeDrawer.DialogSize = new Dimension(400);
        PresetSizeDrawer.IsOpen     = true;
    }

    private void HandleOpenCustomPercentageSizeDrawer(object sender, RoutedEventArgs e)
    {
        PresetSizeDrawer.SizeType   = CustomizableSizeType.Custom;
        PresetSizeDrawer.DialogSize = new Dimension(50, DimensionUnitType.Percentage);
        PresetSizeDrawer.IsOpen     = true;
    }
    
    private void HandleOpenDefaultSizeDrawer(object sender, RoutedEventArgs e)
    {
        PresetSizeDrawer.SizeType = CustomizableSizeType.Small;
        PresetSizeDrawer.IsOpen   = true;
    }

    private void HandleOpenMultilevelLevelTwoDrawer(object sender, RoutedEventArgs e)
    {
        MultiLevelDrawerLevelTwo.IsOpen = true;
    }
}