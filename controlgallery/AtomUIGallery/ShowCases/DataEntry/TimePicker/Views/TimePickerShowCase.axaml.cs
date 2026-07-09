using AtomUI.Controls;

namespace AtomUIGallery.ShowCases.TimePicker;

public partial class TimePickerShowCase : GalleryReactiveUserControl<TimePickerViewModel>
{
    public const string LanguageId = nameof(TimePickerShowCase);

    public TimePickerShowCase()
    {
        InitializeComponent();
    }

    private void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.HandlePickerSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void SetBoundSelectedTimeToNoon(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundSelectedTime = new TimeSpan(12, 0, 0);
        }
    }

    private void ClearBoundSelectedTime(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundSelectedTime = null;
        }
    }

    private void SetBoundSelectedTimeRangeToWorkHours(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundRangeStartSelectedTime = new TimeSpan(9, 0, 0);
            viewModel.BoundRangeEndSelectedTime   = new TimeSpan(18, 0, 0);
        }
    }

    private void ClearBoundSelectedTimeRange(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is TimePickerViewModel viewModel)
        {
            viewModel.BoundRangeStartSelectedTime = null;
            viewModel.BoundRangeEndSelectedTime   = null;
        }
    }
}
