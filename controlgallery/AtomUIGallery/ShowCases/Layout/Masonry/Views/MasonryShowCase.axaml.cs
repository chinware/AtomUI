using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.LogicalTree;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

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

    private void HandleImageSkeletonLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control root)
        {
            return;
        }

        BindSkeletonVisibilityToImageSource(root, "SpecialCoverImage", "SpecialCoverSkeleton");
        BindSkeletonVisibilityToImageSource(root, "MasonryImage", "MasonryImageSkeleton");
    }

    private static void BindSkeletonVisibilityToImageSource(Control root, string imageName, string skeletonName)
    {
        var image = FindDescendantByName<Image>(root, imageName);
        var skeleton = FindDescendantByName<Border>(root, skeletonName);
        if (image is null || skeleton is null)
        {
            return;
        }

        skeleton.Bind(IsVisibleProperty, new Binding
        {
            Source    = image,
            Path      = nameof(Image.Source),
            Converter = ObjectConverters.IsNull
        });
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
