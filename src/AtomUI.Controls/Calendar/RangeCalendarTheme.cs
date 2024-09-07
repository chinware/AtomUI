using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class RangeCalendarTheme : BaseControlTheme
{
   public const string RootPart = "PART_Root";
   public const string CalendarItemPart = "PART_CalendarItem";
   public const string SecondaryCalendarItemPart = "PART_SecondaryCalendarItem";
   public const string FramePart = "PART_Frame";
   
   public RangeCalendarTheme()
      : base(typeof(RangeCalendar))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<RangeCalendar>((rangeCalendar, scope) =>
      {
         var frame = new Border()
         {
            Name = FramePart
         };
         
         CreateTemplateParentBinding(frame, Border.BorderBrushProperty, RangeCalendar.BorderBrushProperty);
         CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, RangeCalendar.BorderThicknessProperty);
         CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, RangeCalendar.CornerRadiusProperty);
         CreateTemplateParentBinding(frame, Border.BackgroundProperty, RangeCalendar.BackgroundProperty);
         CreateTemplateParentBinding(frame, Border.PaddingProperty, RangeCalendar.PaddingProperty);
         
         var rootLayout = new StackPanel()
         {
            Name = RootPart,
            ClipToBounds = true,
            Orientation = Orientation.Horizontal,
         };
         TokenResourceBinder.CreateTokenBinding(rootLayout, StackPanel.SpacingProperty, CalendarTokenResourceKey.RangeCalendarSpacing);
         rootLayout.RegisterInNameScope(scope);

         var calendarItem = new RangeCalendarItem()
         {
            Name = CalendarItemPart,
            HorizontalAlignment = HorizontalAlignment.Left
         };
         
         calendarItem.RegisterInNameScope(scope);

         rootLayout.Children.Add(calendarItem);
         
         var secondaryCalendarItem = new RangeCalendarItem()
         {
            Name = SecondaryCalendarItemPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            IsPrimary = false
         };
         
         secondaryCalendarItem.RegisterInNameScope(scope);

         rootLayout.Children.Add(secondaryCalendarItem);
         
         frame.Child = rootLayout;
         
         return frame;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(RangeCalendar.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      commonStyle.Add(RangeCalendar.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      commonStyle.Add(RangeCalendar.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(RangeCalendar.PaddingProperty, CalendarTokenResourceKey.PanelContentPadding);
      commonStyle.Add(RangeCalendar.MinWidthProperty, CalendarTokenResourceKey.ItemPanelMinWidth);
      commonStyle.Add(RangeCalendar.MinHeightProperty, CalendarTokenResourceKey.ItemPanelMinHeight);
      Add(commonStyle);
   }
}
