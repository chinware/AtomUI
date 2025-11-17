namespace AtomUI.Controls;

internal sealed class CalendarDayButton : BaseCalendarDayButton
{
    protected override Type StyleKeyOverride => typeof(BaseCalendarDayButton);

    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal Calendar? Owner { get; set; }
}