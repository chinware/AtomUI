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
internal class CalendarButtonTheme  : BaseControlTheme
{
   private const string ContentPart = "PART_Content";
   
   public CalendarButtonTheme()
      : base(typeof(CalendarButton))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<CalendarButton>((calendarButton, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPart,
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, CalendarButton.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, CalendarButton.ForegroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty, CalendarButton.BackgroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, CalendarButton.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty, CalendarButton.BorderBrushProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty, CalendarButton.BorderThicknessProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, CalendarButton.FontSizeProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, CalendarButton.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, CalendarButton.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalAlignmentProperty, CalendarButton.HorizontalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalAlignmentProperty, CalendarButton.VerticalAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, CalendarButton.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, CalendarButton.VerticalContentAlignmentProperty);
       
         return contentPresenter;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      commonStyle.Add(CalendarButton.ClickModeProperty, ClickMode.Release);
      commonStyle.Add(CalendarButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(CalendarButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(CalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextLabel);
      commonStyle.Add(CalendarButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      commonStyle.Add(CalendarButton.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(CalendarButton.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(CalendarButton.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      commonStyle.Add(CalendarButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
      commonStyle.Add(CalendarButton.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      commonStyle.Add(CalendarButton.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(CalendarButton.HeightProperty, CalendarTokenResourceKey.CellHeight);
      commonStyle.Add(CalendarButton.MarginProperty, CalendarTokenResourceKey.CellMargin);
      
      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.LineHeightProperty, CalendarTokenResourceKey.CellLineHeight);
      commonStyle.Add(contentStyle);
      
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(CalendarButton.BackgroundProperty, CalendarTokenResourceKey.CellHoverBg);
      commonStyle.Add(hoverStyle);
      
      var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
      inactiveStyle.Add(CalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      commonStyle.Add(inactiveStyle);
            
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(CalendarButton.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      selectedStyle.Add(CalendarButton.ForegroundProperty, GlobalTokenResourceKey.ColorWhite);
      selectedStyle.Add(CalendarButton.BorderThicknessProperty, new Thickness(0));
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
   }
}