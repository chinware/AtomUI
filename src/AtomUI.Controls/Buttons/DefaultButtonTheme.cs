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
      
      this.Add(Button.BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness);
      BuildIconStyle();
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, ButtonTokenResourceKey.DefaultBg);
      enabledStyle.Add(Button.BorderBrushProperty, ButtonTokenResourceKey.DefaultBorderColor);
      enabledStyle.Add(Button.ForegroundProperty, ButtonTokenResourceKey.DefaultColor);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, ButtonTokenResourceKey.DefaultHoverBorderColor);
         hoverStyle.Add(Button.ForegroundProperty, ButtonTokenResourceKey.DefaultHoverColor);
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, ButtonTokenResourceKey.DefaultActiveBorderColor);
         pressedStyle.Add(Button.ForegroundProperty, ButtonTokenResourceKey.DefaultActiveColor);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
      dangerStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorActive);
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
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, ButtonTokenResourceKey.DefaultColor);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, ButtonTokenResourceKey.DefaultActiveColor);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, ButtonTokenResourceKey.DefaultHoverColor);
         Add(iconStyle);
      }
      
      // ghost 状态
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextLightSolid);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorPrimaryActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
         ghostStyle.Add(iconStyle);
      }
      Add(ghostStyle);
      
      var isDangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorError);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorErrorActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         isDangerStyle.Add(iconStyle);
      }
      Add(isDangerStyle);

   }

   private void BuildEnabledGhostStyle(Style enabledStyle)
   {
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      // 正常状态
      ghostStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorTextLightSolid);
      ghostStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorTextLightSolid);
      ghostStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
       
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
         hoverStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
         ghostStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryActive);
         pressedStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryActive);
         ghostStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      dangerStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
      dangerStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorActive);
         dangerStyle.Add(pressedStyle);
      }
      ghostStyle.Add(dangerStyle);
      enabledStyle.Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(Button.BorderBrushProperty, ButtonTokenResourceKey.BorderColorDisabled);
      disabledStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
      Add(disabledStyle);
   }
}