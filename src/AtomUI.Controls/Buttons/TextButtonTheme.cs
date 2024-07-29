using AtomUI.Theme.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TextButtonTheme : BaseButtonTheme
{
   public const string ID = "TextButton";
   
   public TextButtonTheme()
      : base(typeof(Button))
   {
   }
   
   public override string? ThemeResourceKey()
   {
      return ID;
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      BuildIconStyle();
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorTransparent);
      enabledStyle.Add(Button.ForegroundProperty, ButtonResourceKey.DefaultColor);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, ButtonResourceKey.TextHoverBg);
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorBgTextActive);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorErrorBgHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorErrorBgActive);
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      Add(enabledStyle);
   }
   
   private void BuildIconStyle()
   {
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, ButtonResourceKey.DefaultColor);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, ButtonResourceKey.DefaultColor);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, ButtonResourceKey.DefaultColor);
         Add(iconStyle);
      }
      
      var isDangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         isDangerStyle.Add(iconStyle);
         Add(isDangerStyle);
      }
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}