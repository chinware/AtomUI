using System;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : GalleryReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    private bool _wired;

    public CalendarShowCase()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (_wired)
        {
            return;
        }

        _wired = true;

        if (DataContext is CalendarViewModel vm)
        {
            RangeCalendar.ValidRange   = new CalendarDateRange(vm.ValidRangeStart, vm.ValidRangeEnd);
            RangeCalendar.DisabledDate = vm.DisableWeekends;
        }

        EventsCalendar.ValueChanged += (_, args) =>
            AppendLog($"ValueChanged: {args.OldValue:yyyy-MM-dd} -> {args.NewValue:yyyy-MM-dd}");
        EventsCalendar.Selected += (_, args) =>
            AppendLog($"Selected: {args.Value:yyyy-MM-dd} ({args.Source})");
        EventsCalendar.PanelChanged += (_, args) =>
            AppendLog($"PanelChanged: {args.Value:yyyy-MM} ({args.Mode})");
    }

    private void AppendLog(string line)
    {
        EventsLog.Text = line;
    }
}
