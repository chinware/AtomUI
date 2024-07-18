﻿using AtomUI.Styling;
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
      BuildEnabledStyle();
      BuildDisabledStyle();
   }

   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      
      // 正常状态
      enabledStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(ButtonResourceKey.PrimaryColor));
      enabledStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover));
         enabledStyle.Add(hoverStyle);
      }
      
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryActive));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
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
      
      // 正常状态
      ghostStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary));
      ghostStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary));
       
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
      dangerStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      dangerStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError));
      
      // 危险按钮状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         hoverStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorderHover));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Add(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         pressedStyle.Add(Button.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive));
         dangerStyle.Add(pressedStyle);
      }
      
      ghostStyle.Add(dangerStyle);
      Add(ghostStyle);
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