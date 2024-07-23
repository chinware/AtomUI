using AtomUI.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LinkButtonTheme : BaseButtonTheme
{   
   public const string ID = "LinkButton";
   public LinkButtonTheme()
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
      BuildIconStyle();
      BuildDisabledStyle();
   }

   private void BuildIconStyle()
   {
      // 普通状态
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorLink);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, ButtonResourceKey.DefaultActiveColor);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, ButtonResourceKey.DefaultHoverColor);
         Add(iconStyle);
      }
      
      // ghost 状态
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorLink);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         ghostStyle.Add(iconStyle);
      }
      Add(ghostStyle);
      
      var isDangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         isDangerStyle.Add(iconStyle);
      }
      Add(isDangerStyle);
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBg));
      enabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLink));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLinkHover));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLinkActive));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
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
      ghostStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent));
      
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled));
      Add(disabledStyle);
   }
}