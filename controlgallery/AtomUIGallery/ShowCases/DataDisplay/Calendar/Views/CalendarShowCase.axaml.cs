using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : GalleryReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    public CalendarShowCase()
    {
        InitializeComponent();
    }

    private void OnEventsCalendarLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not AtomUI.Desktop.Controls.Calendar calendar || DataContext is not CalendarViewModel vm)
        {
            return;
        }

        calendar.ValueChanged += (_, args) =>
            vm.EventLog = $"ValueChanged: {args.OldValue:yyyy-MM-dd} -> {args.NewValue:yyyy-MM-dd}";
        calendar.Selected += (_, args) =>
            vm.EventLog = $"Selected: {args.Value:yyyy-MM-dd} ({args.Source})";
        calendar.PanelChanged += (_, args) =>
            vm.EventLog = $"PanelChanged: {args.Value:yyyy-MM} ({args.Mode})";
    }
}
