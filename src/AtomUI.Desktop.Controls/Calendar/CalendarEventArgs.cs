namespace AtomUI.Desktop.Controls;

/// <summary>
/// <see cref="Calendar.ValueChanged"/> 事件参数。
/// </summary>
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

/// <summary>
/// <see cref="Calendar.Selected"/> 事件参数。
/// </summary>
public sealed class CalendarSelectedEventArgs : EventArgs
{
    public CalendarSelectedEventArgs(DateTime value, CalendarSelectSource source)
    {
        Value = value;
        Source = source;
    }

    public DateTime Value { get; }
    public CalendarSelectSource Source { get; }
}

/// <summary>
/// <see cref="Calendar.PanelChanged"/> 事件参数。
/// </summary>
public sealed class CalendarPanelChangedEventArgs : EventArgs
{
    public CalendarPanelChangedEventArgs(DateTime value, CalendarMode mode)
    {
        Value = value;
        Mode = mode;
    }

    public DateTime Value { get; }
    public CalendarMode Mode { get; }
}
