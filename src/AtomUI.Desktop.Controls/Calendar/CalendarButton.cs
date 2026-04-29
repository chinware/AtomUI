namespace AtomUI.Desktop.Controls;

/// <summary>
/// Represents a button on a
/// <see cref="T:Avalonia.Controls.Calendar" />.
/// </summary>
internal class CalendarButton : BaseCalendarButton
{
    protected override Type StyleKeyOverride => typeof(BaseCalendarButton);

    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal Calendar? Owner { get; set; }
}