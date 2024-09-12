namespace AtomUI.Controls.CalendarPresenter;

internal sealed class RangeCalendarDayButton : BaseCalendarDayButton
{
    protected override Type StyleKeyOverride => typeof(BaseCalendarDayButton);

    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal RangeCalendar? Owner { get; set; }

    /// <summary>
    /// 是否在主要的 Month View 里面
    /// </summary>
    internal bool IsInPrimaryMonView { get; set; }
}