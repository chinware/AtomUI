using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class RangeCalendarItem : CalendarItem
{
   private const string PrimaryCalendarPC = ":primary";
   private const string SecondaryCalendarPC = ":secondary";
   
   #region 内部属性定义

   private bool _isPrimary = true;

   internal static readonly DirectProperty<RangeCalendarItem, bool> IsPrimaryProperty
      = AvaloniaProperty.RegisterDirect<RangeCalendarItem, bool>(nameof(IsPrimary),
                                                                 o => o.IsPrimary,
                                                                 (o, v) => o.IsPrimary = v);

   internal bool IsPrimary
   {
      get => _isPrimary;
      set => SetAndRaise(IsPrimaryProperty, ref _isPrimary, value);
   }

   #endregion
   
   public RangeCalendarItem()
   {
      UpdatePseudoClasses();
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == IsPrimaryProperty) {
         UpdatePseudoClasses();
      }
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(PrimaryCalendarPC, IsPrimary);
      PseudoClasses.Set(SecondaryCalendarPC, !IsPrimary);
   }
}