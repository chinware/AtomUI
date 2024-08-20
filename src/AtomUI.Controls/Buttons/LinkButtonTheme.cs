using AtomUI.Theme.Styling;
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
         iconStyle.Add(PathIcon.DisabledFilledBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorLink);
         iconStyle.Add(PathIcon.SelectedFilledBrushProperty, ButtonTokenResourceKey.DefaultActiveColor);
         iconStyle.Add(PathIcon.ActiveFilledBrushProperty, ButtonTokenResourceKey.DefaultHoverColor);
         Add(iconStyle);
      }
      
      // ghost 状态
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorLink);
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

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(ButtonTokenResourceKey.DefaultBg));
      enabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorLink));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorLinkHover));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorLinkActive));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorError));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorErrorHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorErrorActive));
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
      ghostStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorTransparent));
      
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalTokenResourceKey.ColorTextDisabled));
      Add(disabledStyle);
   }
}