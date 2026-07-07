using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Timeline;

public partial class TimelineShowCase : GalleryReactiveUserControl<TimelineViewModel>
{
    public const string LanguageId = nameof(TimelineShowCase);

    public TimelineShowCase()
    {
        InitializeComponent();
    }

    private void ReverseButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TimelineViewModel viewModel)
        {
            viewModel.ReverseTimelineIsReverse = !viewModel.ReverseTimelineIsReverse;
        }
    }

    private void ModeChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton { IsChecked: true, Tag: TimelineMode mode } &&
            DataContext is TimelineViewModel viewModel)
        {
            viewModel.SelectedTimelineMode = mode;
        }
    }
}
