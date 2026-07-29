using AtomUI;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedShowCase : GalleryReactiveUserControl<SegmentedViewModel>
{
    public const string LanguageId = nameof(SegmentedShowCase);

    public SegmentedShowCase()
    {
        InitializeComponent();
    }

    public void HandleRoundShapeSizeSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is not SelectingItemsControl segmented ||
            DataContext is not SegmentedViewModel viewModel)
        {
            return;
        }

        viewModel.RoundShapeSizeType = segmented.SelectedIndex switch
        {
            0 => CustomizableSizeType.Small,
            2 => CustomizableSizeType.Large,
            _ => CustomizableSizeType.Middle
        };
    }

}
