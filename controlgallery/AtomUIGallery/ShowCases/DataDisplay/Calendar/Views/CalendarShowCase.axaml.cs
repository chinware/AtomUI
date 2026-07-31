using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : GalleryReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    public CalendarShowCase()
    {
        InitializeComponent();
    }

    private void OnCalendarValueChanged(object? sender, CalendarValueChangedEventArgs e)
    {
        if (DataContext is CalendarViewModel vm)
        {
            vm.EventLog = $"ValueChanged: {e.OldValue:yyyy-MM-dd} -> {e.NewValue:yyyy-MM-dd}";
        }
    }

    private void OnCalendarSelected(object? sender, CalendarSelectedEventArgs e)
    {
        if (DataContext is CalendarViewModel vm)
        {
            vm.EventLog = $"Selected: {e.Value:yyyy-MM-dd} ({e.Source})";
        }
    }

    private void OnCalendarPanelChanged(object? sender, CalendarPanelChangedEventArgs e)
    {
        if (DataContext is CalendarViewModel vm)
        {
            vm.EventLog = $"PanelChanged: {e.Value:yyyy-MM} ({e.Mode})";
        }
    }
}
