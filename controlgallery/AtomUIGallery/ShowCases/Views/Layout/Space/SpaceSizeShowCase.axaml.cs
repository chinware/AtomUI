using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SpaceSizeShowCase : ReactiveUserControl<SpaceViewModel>
{
    public SpaceSizeShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is SpaceViewModel vm)
        {
            vm.SizeType = CustomizableSizeType.Small;
        }
    }

    private void HandleSizeTypeChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton &&
            radioButton.IsChecked == true)
        {
            var sizeType = (CustomizableSizeType)radioButton.Tag!;
            if (DataContext is SpaceViewModel vm)
            {
                vm.SizeType = sizeType;
            }

            if (CustomSizeSlider is not null)
            {
                CustomSizeSlider.IsVisible = sizeType == CustomizableSizeType.Custom;
            }
        }
    }
}
