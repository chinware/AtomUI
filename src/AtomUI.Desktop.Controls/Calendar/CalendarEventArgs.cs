using System;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

public sealed class CalendarValueChangedEventArgs : EventArgs
{
    public CalendarValueChangedEventArgs(DateTime oldValue, DateTime newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public DateTime OldValue { get; }
    public DateTime NewValue { get; }
}

public sealed class CalendarSelectedEventArgs : EventArgs
{
    public CalendarSelectedEventArgs(DateTime value, CalendarSelectSource source)
    {
        Value  = value;
        Source = source;
    }

    public DateTime Value { get; }
    public CalendarSelectSource Source { get; }
}

public sealed class CalendarPanelChangedEventArgs : EventArgs
{
    public CalendarPanelChangedEventArgs(DateTime value, CalendarMode mode)
    {
        Value = value;
        Mode  = mode;
    }

    public DateTime Value { get; }
    public CalendarMode Mode { get; }
}

// Legacy event args for old DatePicker CalendarView (will be removed in later phases)
internal class CalendarDateChangedEventArgs : RoutedEventArgs
{
    internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
    {
        RemovedDate = removedDate;
        AddedDate   = addedDate;
    }

    public DateTime? RemovedDate { get; private set; }
    public DateTime? AddedDate { get; private set; }
}

internal class CalendarModeChangedEventArgs : RoutedEventArgs
{
    public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
    {
        OldMode = oldMode;
        NewMode = newMode;
    }

    public CalendarMode OldMode { get; private set; }
    public CalendarMode NewMode { get; private set; }
}
