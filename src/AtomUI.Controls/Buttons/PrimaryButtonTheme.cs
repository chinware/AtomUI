using AtomUI.Theme.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Colors = Avalonia.Media.Colors;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PrimaryButtonTheme : BaseButtonTheme
{
   public const string ID = "PrimaryButton";
   
   public PrimaryButtonTheme()
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
      enabledStyle.Add(Button.ForegroundProperty, ButtonTokenResourceKey.PrimaryColor);
      enabledStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
         enabledStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorPrimaryActive);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorErrorHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorErrorActive);
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);
      Add(enabledStyle);

      BuildEnabledGhostStyle();
   }

   private void BuildEnabledGhostStyle()
   {
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      ghostStyle.Add(Button.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
      ghostStyle.Add(Button.BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness);
      // 正常状态
      ghostStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorPrimary);
      ghostStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
       
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
      dangerStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorError);
      dangerStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
      
      // 危险按钮状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorActive);
         dangerStyle.Add(pressedStyle);
      }
      
      ghostStyle.Add(dangerStyle);
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness);
      disabledStyle.Add(Button.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(Button.BorderBrushProperty, ButtonTokenResourceKey.BorderColorDisabled);
      disabledStyle.Add(Button.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
      Add(disabledStyle);
   }
}