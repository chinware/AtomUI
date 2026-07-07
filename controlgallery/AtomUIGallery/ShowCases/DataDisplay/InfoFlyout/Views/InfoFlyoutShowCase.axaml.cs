using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.InfoFlyout;

public partial class InfoFlyoutShowCase : GalleryReactiveUserControl<InfoFlyoutViewModel>
{
    public const string LanguageId = nameof(InfoFlyoutShowCase);

    public InfoFlyoutShowCase()
    {
        InitializeComponent();
    }

    private void HandleArrowSegmentedSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (DataContext is InfoFlyoutViewModel viewModel)
        {
            viewModel.HandleSelectionChanged(sender, args);
        }
    }
}
