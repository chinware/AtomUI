using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class TimePickerFlyout : Flyout
{
   internal TimePicker TimePickerRef { get; set; }

   public TimePickerFlyout(TimePicker timePicker)
   {
      TimePickerRef = timePicker;
   }

   protected override Control CreatePresenter()
   {
      var presenter = new TimePickerFlyoutPresenter(TimePickerRef);
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);

      CalculateShowArrowEffective();
      SetupArrowPosition(Popup, presenter);
      return presenter;
   }
}