using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class HeadTextButtonTheme : BaseControlTheme
{
   private const string ContentPart = "PART_Content";
   
   public HeadTextButtonTheme() : base(typeof(HeadTextButton)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<HeadTextButton>((headTextButton, scope) =>
      {
         var content = new ContentPresenter()
         {
            Name = ContentPart,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
         };
         CreateTemplateParentBinding(content, ContentPresenter.ContentProperty, HeadTextButton.ContentProperty);
         CreateTemplateParentBinding(content, ContentPresenter.ContentTemplateProperty, HeadTextButton.ContentTemplateProperty);
         CreateTemplateParentBinding(content, ContentPresenter.BackgroundProperty, HeadTextButton.BackgroundProperty);
         return content;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(HeadTextButton.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      commonStyle.Add(HeadTextButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      commonStyle.Add(HeadTextButton.FontWeightProperty, FontWeight.SemiBold);
      commonStyle.Add(HeadTextButton.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(HeadTextButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(HeadTextButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimary);
      commonStyle.Add(hoverStyle);
      
      Add(commonStyle);
   }
}