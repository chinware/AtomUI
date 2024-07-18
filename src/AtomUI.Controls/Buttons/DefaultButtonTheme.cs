using AtomUI.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DefaultButtonTheme : BaseButtonTheme
{
   public const string ID = "DefaultButton";
   
   public DefaultButtonTheme()
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
      enabledStyle.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBg)));
      enabledStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBorderColor)));
      enabledStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultColor)));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultHoverBorderColor)));
         hoverStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultHoverColor)));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultActiveBorderColor)));
         pressedStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultActiveColor)));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError)));
      dangerStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError)));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover)));
         hoverStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover)));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive)));
         pressedStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive)));
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      Add(enabledStyle);

      BuildEnabledGhostStyle();
   }

   private void BuildEnabledGhostStyle()
   {
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      // 正常状态
      ghostStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextLightSolid)));
      ghostStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextLightSolid)));
      ghostStyle.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent)));
       
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
         hoverStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
         ghostStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryActive)));
         pressedStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryActive)));
         ghostStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent)));
      
      ghostStyle.Add(dangerStyle);
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      disabledStyle.Add(new Setter(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.BorderColorDisabled)));
      disabledStyle.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainerDisabled)));
      Add(disabledStyle);
   }
}