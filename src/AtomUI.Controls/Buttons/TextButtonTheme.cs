using AtomUI.Styling;
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
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent)));
      enabledStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultColor)));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(ButtonResourceKey.TextHoverBg)));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgTextActive)));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError)));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBgHover)));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBgActive)));
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      Add(enabledStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      Add(disabledStyle);
   }
}