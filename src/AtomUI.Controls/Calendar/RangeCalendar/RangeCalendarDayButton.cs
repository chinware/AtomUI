namespace AtomUI.Controls;

internal sealed class RangeCalendarDayButton : BaseCalendarDayButton
{
   protected override Type StyleKeyOverride => typeof(BaseCalendarDayButton);
   
   /// <summary>
   /// Gets or sets the Calendar associated with this button.
   /// </summary>
   internal RangeCalendar? Owner { get; set; }
}