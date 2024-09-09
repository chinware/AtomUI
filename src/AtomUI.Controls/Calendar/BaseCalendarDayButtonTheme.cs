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
internal class BaseCalendarDayButtonTheme : BaseControlTheme
{
   private const string ContentPart = "PART_Content";
   
   public BaseCalendarDayButtonTheme()
      : base(typeof(BaseCalendarDayButton))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<BaseCalendarDayButton>((calendarDayButton, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPart,
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, BaseCalendarDayButton.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, BaseCalendarDayButton.ForegroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty, BaseCalendarDayButton.BackgroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, BaseCalendarDayButton.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty, BaseCalendarDayButton.BorderBrushProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty, BaseCalendarDayButton.BorderThicknessProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, BaseCalendarDayButton.FontSizeProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, BaseCalendarDayButton.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, BaseCalendarDayButton.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalAlignmentProperty, BaseCalendarDayButton.HorizontalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalAlignmentProperty, BaseCalendarDayButton.VerticalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, BaseCalendarDayButton.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, BaseCalendarDayButton.VerticalContentAlignmentProperty);
       
         return contentPresenter;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      commonStyle.Add(BaseCalendarDayButton.ClickModeProperty, ClickMode.Release);
      commonStyle.Add(BaseCalendarDayButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(BaseCalendarDayButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(BaseCalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextLabel);
      commonStyle.Add(BaseCalendarDayButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      commonStyle.Add(BaseCalendarDayButton.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(BaseCalendarDayButton.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(BaseCalendarDayButton.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      commonStyle.Add(BaseCalendarDayButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
      commonStyle.Add(BaseCalendarDayButton.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      commonStyle.Add(BaseCalendarDayButton.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(BaseCalendarDayButton.WidthProperty, CalendarTokenResourceKey.CellWidth);
      commonStyle.Add(BaseCalendarDayButton.HeightProperty, CalendarTokenResourceKey.CellHeight);
      commonStyle.Add(BaseCalendarDayButton.MarginProperty, CalendarTokenResourceKey.CellMargin);

      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.LineHeightProperty, CalendarTokenResourceKey.CellLineHeight);
      commonStyle.Add(contentStyle);

      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(BaseCalendarDayButton.BackgroundProperty, CalendarTokenResourceKey.CellHoverBg);
      commonStyle.Add(hoverStyle);

      var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
      inactiveStyle.Add(BaseCalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      commonStyle.Add(inactiveStyle);
      
      var todayStyle = new Style(selector => selector.Nesting().Class(BaseCalendarDayButton.TodayPC));
      todayStyle.Add(BaseCalendarDayButton.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      commonStyle.Add(todayStyle);
      
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(BaseCalendarDayButton.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      selectedStyle.Add(BaseCalendarDayButton.ForegroundProperty, GlobalTokenResourceKey.ColorWhite);
      selectedStyle.Add(BaseCalendarDayButton.BorderThicknessProperty, new Thickness(0));
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
   }
}