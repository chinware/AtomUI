namespace AtomUI.Controls.CalendarPresenter;

internal sealed class RangeCalendarButton : BaseCalendarButton
{
    protected override Type StyleKeyOverride => typeof(BaseCalendarButton);

    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal RangeCalendar? Owner { get; set; }
}