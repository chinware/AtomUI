using AtomUI.Controls;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.DatePicker;

public partial class DatePickerShowCase : GalleryReactiveUserControl<DatePickerViewModel>
{
    public const string LanguageId = nameof(DatePickerShowCase);

    public DatePickerShowCase()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (DataContext is DatePickerViewModel viewModel)
            {
                viewModel.PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
            }
        });
    }

    private void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.HandlePickerSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void HandlePickerPlacementCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.HandlePickerPlacementCheckedChanged(sender, args);
        }
    }

    private void SetBoundSelectedDateTimeTomorrow(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.BoundSelectedDateTime = DateTime.Today.AddDays(1);
        }
    }

    private void ClearBoundSelectedDateTime(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.BoundSelectedDateTime = null;
        }
    }

    private void SetBoundSelectedDateRangeThisWeek(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            var today       = DateTime.Today;
            var daysToStart = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var startDate   = today.AddDays(-daysToStart);

            viewModel.BoundRangeStartSelectedDate = startDate;
            viewModel.BoundRangeEndSelectedDate   = startDate.AddDays(6);
        }
    }

    private void ClearBoundSelectedDateRange(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (DataContext is DatePickerViewModel viewModel)
        {
            viewModel.BoundRangeStartSelectedDate = null;
            viewModel.BoundRangeEndSelectedDate   = null;
        }
    }
}
