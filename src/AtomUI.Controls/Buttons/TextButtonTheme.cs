using AtomUI.Theme.Styling;
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
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      enabledStyle.Add(Button.ForegroundProperty, ButtonTokenResourceKey.DefaultColor);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, ButtonTokenResourceKey.TextHoverBg);
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorBgTextActive);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorErrorBgHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorErrorBgActive);
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      Add(enabledStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}