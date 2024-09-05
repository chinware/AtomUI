using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class RangeTimePickerFlyout : Flyout
{
   internal RangeTimePicker RangeTimePickerRef { get; set; }

   public RangeTimePickerFlyout(RangeTimePicker timePicker)
   {
      RangeTimePickerRef = timePicker;
   }

   protected override Control CreatePresenter()
   {
      var presenter = new RangeTimePickerFlyoutPresenter(RangeTimePickerRef);
      
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
      
      BindUtils.RelayBind(RangeTimePickerRef, RangeTimePicker.MinuteIncrementProperty, presenter, RangeTimePickerFlyoutPresenter.MinuteIncrementProperty);
      BindUtils.RelayBind(RangeTimePickerRef, RangeTimePicker.SecondIncrementProperty, presenter, RangeTimePickerFlyoutPresenter.SecondIncrementProperty);
      BindUtils.RelayBind(RangeTimePickerRef, RangeTimePicker.ClockIdentifierProperty, presenter, RangeTimePickerFlyoutPresenter.ClockIdentifierProperty);
      
      CalculateShowArrowEffective();
      SetupArrowPosition(Popup, presenter);
      return presenter;
   }
}