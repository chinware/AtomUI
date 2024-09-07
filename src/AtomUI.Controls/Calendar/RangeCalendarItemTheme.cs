using AtomUI.Theme.Styling;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeCalendarItemTheme : CalendarItemTheme
{
   public RangeCalendarItemTheme()
      : base(typeof(RangeCalendarItem)) { }

   protected override IconButton BuildPreviousButton()
   {
      var previousButton = base.BuildPreviousButton();
      CreateTemplateParentBinding(previousButton, IconButton.IsVisibleProperty, RangeCalendarItem.IsPrimaryProperty);
      return previousButton;
   }

   protected override IconButton BuildPreviousMonthButton()
   {
      var previousMonthButton = base.BuildPreviousMonthButton();
      CreateTemplateParentBinding(previousMonthButton, IconButton.IsVisibleProperty,
                                  RangeCalendarItem.IsPrimaryProperty);
      return previousMonthButton;
   }

   protected override IconButton BuildNextButton()
   {
      var nextButton = base.BuildNextButton();
      CreateTemplateParentBinding(nextButton, IconButton.IsVisibleProperty, RangeCalendarItem.IsPrimaryProperty,
                                  BindingMode.Default,
                                  BoolConverters.Not);
      return nextButton;
   }

   protected override IconButton BuildNextMonthButton()
   {
      var nextMonthButton = base.BuildNextMonthButton();
      CreateTemplateParentBinding(nextMonthButton, IconButton.IsVisibleProperty, RangeCalendarItem.IsPrimaryProperty,
                                  BindingMode.Default,
                                  BoolConverters.Not);
      return nextMonthButton;
   }
}