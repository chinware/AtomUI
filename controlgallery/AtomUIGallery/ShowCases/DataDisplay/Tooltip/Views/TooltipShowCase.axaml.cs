using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Tooltip;

public partial class TooltipShowCase : GalleryReactiveUserControl<TooltipViewModel>
{
    public const string LanguageId = nameof(TooltipShowCase);

    public TooltipShowCase()
    {
        InitializeComponent();
    }

    private void HandleArrowSegmentedSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (DataContext is TooltipViewModel viewModel)
        {
            viewModel.HandleSelectionChanged(sender, args);
        }
    }
}
