using Avalonia.Layout;

namespace AtomUI.Controls;

public class TimePickerFlyoutPresenter : FlyoutPresenter
{
   protected override Type StyleKeyOverride => typeof(TimePickerFlyoutPresenter);
   
   internal TimePicker TimePickerRef { get; set; }

   public TimePickerFlyoutPresenter(TimePicker timePicker)
   {
      TimePickerRef = timePicker;
      HorizontalAlignment = HorizontalAlignment.Left;
   }
}