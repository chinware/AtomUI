using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarTheme : BaseControlTheme
{
   public const string RootPart = "PART_Root";
   public const string CalendarItemPart = "PART_CalendarItem";
   public const string FramePart = "PART_Frame";
   
   public CalendarTheme()
      : base(typeof(Calendar))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Calendar>((calendar, scope) =>
      {
         var frame = new Border()
         {
            Name = FramePart
         };
         
         CreateTemplateParentBinding(frame, Border.BorderBrushProperty, Calendar.BorderBrushProperty);
         CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, Calendar.BorderThicknessProperty);
         CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, Calendar.CornerRadiusProperty);
         CreateTemplateParentBinding(frame, Border.BackgroundProperty, Calendar.BackgroundProperty);
         CreateTemplateParentBinding(frame, Border.PaddingProperty, Calendar.PaddingProperty);
         
         var rootLayout = new StackPanel()
         {
            Name = RootPart,
            ClipToBounds = true,
            Orientation = Orientation.Horizontal
         };
         rootLayout.RegisterInNameScope(scope);

         var calendarItem = new CalendarItem()
         {
            Name = CalendarItemPart,
         };
         
         calendarItem.RegisterInNameScope(scope);

         rootLayout.Children.Add(calendarItem);
         frame.Child = rootLayout;
         
         return frame;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(Calendar.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      commonStyle.Add(Calendar.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      commonStyle.Add(Calendar.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(Calendar.PaddingProperty, CalendarTokenResourceKey.PanelContentPadding);
      commonStyle.Add(Calendar.MinWidthProperty, CalendarTokenResourceKey.ItemPanelMinWidth);
      commonStyle.Add(Calendar.MinHeightProperty, CalendarTokenResourceKey.ItemPanelMinHeight);
      Add(commonStyle);
   }
}