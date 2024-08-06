using AtomUI.Theme.Styling;
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
      enabledStyle.Add(Button.BackgroundProperty, ButtonResourceKey.DefaultBg);
      enabledStyle.Add(Button.BorderBrushProperty, ButtonResourceKey.DefaultBorderColor);
      enabledStyle.Add(Button.ForegroundProperty, ButtonResourceKey.DefaultColor);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, ButtonResourceKey.DefaultHoverBorderColor);
         hoverStyle.Add(Button.ForegroundProperty, ButtonResourceKey.DefaultHoverColor);
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, ButtonResourceKey.DefaultActiveBorderColor);
         pressedStyle.Add(Button.ForegroundProperty, ButtonResourceKey.DefaultActiveColor);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorError);
      dangerStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorActive);
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
      ghostStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorTextLightSolid);
      ghostStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorTextLightSolid);
      ghostStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorTransparent);
       
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorPrimaryHover);
         hoverStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         ghostStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorPrimaryActive);
         pressedStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive);
         ghostStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorTransparent);
      dangerStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorError);
      dangerStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorActive);
         dangerStyle.Add(pressedStyle);
      }
      ghostStyle.Add(dangerStyle);
      enabledStyle.Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(Button.BorderBrushProperty, ButtonResourceKey.BorderColorDisabled);
      disabledStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      Add(disabledStyle);
   }
}