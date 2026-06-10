using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Space;

public partial class SpaceSizeShowCase : GalleryReactiveUserControl<SpaceViewModel>
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
        if (sender is AtomUIRadioButton radioButton &&
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
