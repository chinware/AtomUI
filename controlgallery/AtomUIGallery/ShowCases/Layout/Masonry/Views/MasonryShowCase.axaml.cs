using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Masonry;

public partial class MasonryShowCase : GalleryReactiveUserControl<MasonryViewModel>
{
    public const string LanguageId = nameof(MasonryShowCase);

    public MasonryShowCase()
    {
        InitializeComponent();
    }

    private void HandleDynamicMasonryLayoutChanged(object? sender, MasonryLayoutChangedEventArgs e)
    {
        ViewModel?.UpdateDynamicMasonryColumns(e.Items);
    }

    private void HandleRemoveDynamicMasonryItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: MasonryDynamicItem item })
        {
            ViewModel?.RemoveDynamicMasonryItem(item.Key);
        }
    }

}
