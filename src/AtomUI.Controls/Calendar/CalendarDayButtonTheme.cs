using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarDayButtonTheme : BaseControlTheme
{
   private const string ContentPart = "PART_Content";
   
   public CalendarDayButtonTheme()
      : base(typeof(CalendarDayButton))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<CalendarDayButton>((calendarDayButton, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPart,
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, CalendarDayButton.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, CalendarDayButton.ForegroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty, CalendarDayButton.BackgroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, CalendarDayButton.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty, CalendarDayButton.BorderBrushProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty, CalendarDayButton.BorderThicknessProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, CalendarDayButton.FontSizeProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, CalendarDayButton.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, CalendarDayButton.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalAlignmentProperty, CalendarDayButton.HorizontalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalAlignmentProperty, CalendarDayButton.VerticalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, CalendarDayButton.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, CalendarDayButton.VerticalContentAlignmentProperty);
       
         return contentPresenter;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      commonStyle.Add(CalendarDayButton.ClickModeProperty, ClickMode.Release);
      commonStyle.Add(CalendarDayButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(CalendarDayButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(CalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextLabel);
      commonStyle.Add(CalendarDayButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      commonStyle.Add(CalendarDayButton.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(CalendarDayButton.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(CalendarDayButton.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      commonStyle.Add(CalendarDayButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
      commonStyle.Add(CalendarDayButton.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      commonStyle.Add(CalendarDayButton.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(CalendarDayButton.WidthProperty, CalendarTokenResourceKey.CellWidth);
      commonStyle.Add(CalendarDayButton.HeightProperty, CalendarTokenResourceKey.CellHeight);
      commonStyle.Add(CalendarDayButton.MarginProperty, CalendarTokenResourceKey.CellMargin);

      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.LineHeightProperty, CalendarTokenResourceKey.CellLineHeight);
      commonStyle.Add(contentStyle);

      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(CalendarDayButton.BackgroundProperty, CalendarTokenResourceKey.CellHoverBg);
      commonStyle.Add(hoverStyle);

      var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
      inactiveStyle.Add(CalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      commonStyle.Add(inactiveStyle);
      
      var todayStyle = new Style(selector => selector.Nesting().Class(CalendarDayButton.TodayPC));
      todayStyle.Add(CalendarDayButton.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      commonStyle.Add(todayStyle);
      
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(CalendarDayButton.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      selectedStyle.Add(CalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorWhite);
      selectedStyle.Add(CalendarDayButton.BorderThicknessProperty, new Thickness(0));
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
   }
}