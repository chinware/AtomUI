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
internal class BaseCalendarButtonTheme  : BaseControlTheme
{
   private const string ContentPart = "PART_Content";
   
   public BaseCalendarButtonTheme()
      : base(typeof(BaseCalendarButton))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<BaseCalendarButton>((calendarButton, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPart,
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, BaseCalendarButton.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, BaseCalendarButton.ForegroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty, BaseCalendarButton.BackgroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, BaseCalendarButton.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty, BaseCalendarButton.BorderBrushProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty, BaseCalendarButton.BorderThicknessProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, BaseCalendarButton.FontSizeProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, BaseCalendarButton.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, BaseCalendarButton.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalAlignmentProperty, BaseCalendarButton.HorizontalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalAlignmentProperty, BaseCalendarButton.VerticalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, BaseCalendarButton.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, BaseCalendarButton.VerticalContentAlignmentProperty);
       
         return contentPresenter;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      commonStyle.Add(BaseCalendarButton.ClickModeProperty, ClickMode.Release);
      commonStyle.Add(BaseCalendarButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(BaseCalendarButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(BaseCalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextLabel);
      commonStyle.Add(BaseCalendarButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      commonStyle.Add(BaseCalendarButton.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(BaseCalendarButton.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(BaseCalendarButton.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      commonStyle.Add(BaseCalendarButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
      commonStyle.Add(BaseCalendarButton.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      commonStyle.Add(BaseCalendarButton.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(BaseCalendarButton.HeightProperty, CalendarTokenResourceKey.CellHeight);
      commonStyle.Add(BaseCalendarButton.MarginProperty, CalendarTokenResourceKey.CellMargin);
      
      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.LineHeightProperty, CalendarTokenResourceKey.CellLineHeight);
      commonStyle.Add(contentStyle);
      
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(BaseCalendarButton.BackgroundProperty, CalendarTokenResourceKey.CellHoverBg);
      commonStyle.Add(hoverStyle);
      
      var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
      inactiveStyle.Add(BaseCalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      commonStyle.Add(inactiveStyle);
            
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(BaseCalendarButton.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      selectedStyle.Add(BaseCalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorWhite);
      selectedStyle.Add(BaseCalendarButton.BorderThicknessProperty, new Thickness(0));
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
   }
}