using AtomUI.Theme.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
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
      BuildIconStyle();
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      
      // 正常状态
      enabledStyle.Add(Button.ForegroundProperty, ButtonResourceKey.PrimaryColor);
      enabledStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorPrimary);
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorPrimaryHover);
         enabledStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorPrimaryActive);
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorError);
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorErrorHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorErrorActive);
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
      ghostStyle.Add(Button.BorderThicknessProperty, GlobalResourceKey.BorderThickness);
      // 正常状态
      ghostStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorPrimary);
      ghostStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorPrimary);
       
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
      dangerStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorError);
      dangerStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorError);
      
      // 危险按钮状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorBorderHover);
         hoverStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorErrorActive);
         pressedStyle.Add(Button.BorderBrushProperty, GlobalResourceKey.ColorErrorActive);
         dangerStyle.Add(pressedStyle);
      }
      
      ghostStyle.Add(dangerStyle);
      Add(ghostStyle);
   }

   private void BuildIconStyle()
   {
      // 普通状态
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, ButtonResourceKey.PrimaryColor);
         Add(iconStyle);
      }
      // ghost 状态
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorPrimary);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         ghostStyle.Add(iconStyle);
      }
      var isDangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         isDangerStyle.Add(iconStyle);
      }
      ghostStyle.Add(isDangerStyle);
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.BorderThicknessProperty, GlobalResourceKey.BorderThickness);
      disabledStyle.Add(Button.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(Button.BorderBrushProperty, ButtonResourceKey.BorderColorDisabled);
      disabledStyle.Add(Button.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      Add(disabledStyle);
   }
}