using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class ButtonSpinnerDecoratedBox : AddOnDecoratedBox
{
   protected override Type StyleKeyOverride => typeof(AddOnDecoratedBox);
   
   #region 内部属性定义

   internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, Location> ButtonSpinnerLocationProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, Location>(nameof(ButtonSpinnerLocation),
                                                                           o => o.ButtonSpinnerLocation,
                                                                           (o, v) => o.ButtonSpinnerLocation = v);

   internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, bool> ShowButtonSpinnerProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, bool>(nameof(ShowButtonSpinner),
                                                                       o => o.ShowButtonSpinner,
                                                                       (o, v) => o.ShowButtonSpinner = v);

   private bool _showButtonSpinner;

   internal bool ShowButtonSpinner
   {
      get => _showButtonSpinner;
      set => SetAndRaise(ShowButtonSpinnerProperty, ref _showButtonSpinner, value);
   }

   private Location _buttonSpinnerLocation;

   internal Location ButtonSpinnerLocation
   {
      get => _buttonSpinnerLocation;
      set => SetAndRaise(ButtonSpinnerLocationProperty, ref _buttonSpinnerLocation, value);
   }

   #endregion
}