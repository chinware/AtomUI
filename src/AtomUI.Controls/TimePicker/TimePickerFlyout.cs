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
      
      BindUtils.RelayBind(TimePickerRef, TimePicker.MinuteIncrementProperty, presenter, TimePickerFlyoutPresenter.MinuteIncrementProperty);
      BindUtils.RelayBind(TimePickerRef, TimePicker.SecondIncrementProperty, presenter, TimePickerFlyoutPresenter.SecondIncrementProperty);
      BindUtils.RelayBind(TimePickerRef, TimePicker.ClockIdentifierProperty, presenter, TimePickerFlyoutPresenter.ClockIdentifierProperty);
      
      CalculateShowArrowEffective();
      SetupArrowPosition(Popup, presenter);
      return presenter;
   }
}