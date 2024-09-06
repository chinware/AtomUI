using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarTheme : BaseControlTheme
{
   public const string RootPart = "PART_Root";
   public const string CalendarItemPart = "PART_CalendarItem";
   
   public CalendarTheme()
      : base(typeof(Calendar))
   {
      
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Calendar>((calendar, scope) =>
      {
         var rootLayout = new StackPanel()
         {
            Name = RootPart,
            ClipToBounds = true
         };
         rootLayout.RegisterInNameScope(scope);

         var calendarItem = new CalendarItem()
         {
            Name = CalendarItemPart
         };
         calendarItem.RegisterInNameScope(scope);
         CreateTemplateParentBinding(calendarItem, CalendarItem.BorderBrushProperty, Calendar.BorderBrushProperty);
         CreateTemplateParentBinding(calendarItem, CalendarItem.BorderThicknessProperty, Calendar.BorderThicknessProperty);
         CreateTemplateParentBinding(calendarItem, CalendarItem.CornerRadiusProperty, Calendar.CornerRadiusProperty);
         CreateTemplateParentBinding(calendarItem, CalendarItem.BackgroundProperty, Calendar.BackgroundProperty);
         rootLayout.Children.Add(calendarItem);
         
         return rootLayout;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(Calendar.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      commonStyle.Add(Calendar.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      commonStyle.Add(Calendar.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      Add(commonStyle);
   }
}