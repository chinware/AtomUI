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
      
      this.Add(Button.BorderThicknessProperty, GlobalResourceKey.BorderThickness);
      BuildIconStyle();
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBg));
      enabledStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBorderColor));
      enabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultColor));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultHoverBorderColor));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultHoverColor));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultActiveBorderColor));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultActiveColor));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      dangerStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      BuildEnabledGhostStyle(enabledStyle);
      Add(enabledStyle);
   }

   private void BuildIconStyle()
   {
      // 普通状态
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, ButtonResourceKey.DefaultColor);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, ButtonResourceKey.DefaultActiveColor);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, ButtonResourceKey.DefaultHoverColor);
         Add(iconStyle);
      }
      
      // ghost 状态
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorTextLightSolid);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         ghostStyle.Add(iconStyle);
      }
      Add(ghostStyle);
      
      var isDangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         isDangerStyle.Add(iconStyle);
      }
      Add(isDangerStyle);

   }

   private void BuildEnabledGhostStyle(Style enabledStyle)
   {
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      // 正常状态
      ghostStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextLightSolid));
      ghostStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextLightSolid));
      ghostStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent));
       
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover));
         hoverStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover));
         ghostStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryActive));
         pressedStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryActive));
         ghostStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent));
      dangerStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      dangerStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         dangerStyle.Add(pressedStyle);
      }
      ghostStyle.Add(dangerStyle);
      enabledStyle.Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled));
      disabledStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(ButtonResourceKey.BorderColorDisabled));
      disabledStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainerDisabled));
      Add(disabledStyle);
   }
}