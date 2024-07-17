using AtomUI.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class CheckBoxTheme : ControlTheme
{
   public CheckBoxTheme()
      : base(typeof(CheckBox))
   {
   }
   
   protected override void BuildStyles()
   {
      Add(new Setter(CheckBox.CheckIndicatorSizeProperty, new DynamicResourceExtension(CheckBoxResourceKey.CheckIndicatorSize)));
      Add(new Setter(CheckBox.PaddingInlineProperty, new DynamicResourceExtension(GlobalResourceKey.PaddingXS)));
      Add(new Setter(CheckBox.IndicatorBorderRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusSM)));
      Add(new Setter(CheckBox.IndicatorTristateMarkSizeProperty, new DynamicResourceExtension(CheckBoxResourceKey.IndicatorTristateMarkSize)));
      Add(new Setter(CheckBox.IndicatorTristateMarkBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
 
   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsEnabledProperty, false));
      disableStyle.Add(new Setter(CheckBox.IndicatorBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainerDisabled)));
      disableStyle.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBorder)));
      disableStyle.Add(new Setter(CheckBox.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      
      var checkedStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsCheckedProperty, true));
      checkedStyle.Setters.Add(new Setter(CheckBox.IndicatorCheckedMarkBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      disableStyle.Add(checkedStyle);

      var indeterminateStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsCheckedProperty, null));
      indeterminateStyle.Setters.Add(new Setter(CheckBox.IndicatorTristateMarkBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      disableStyle.Add(indeterminateStyle);
      Add(disableStyle);
   }
   
   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsEnabledProperty, true));
      enabledStyle.Add(new Setter(CheckBox.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorText)));
      enabledStyle.Add(new Setter(CheckBox.IndicatorBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainer)));
      enabledStyle.Add(new Setter(CheckBox.IndicatorCheckedMarkBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainer)));
      enabledStyle.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBorder)));
      
      // 选中
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Setters.Add(new Setter(CheckBox.IndicatorBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      checkedStyle.Setters.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      
      // 选中 hover
      var checkedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      checkedHoverStyle.Setters.Add(new Setter(CheckBox.IndicatorBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
      checkedHoverStyle.Setters.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
      checkedStyle.Add(checkedHoverStyle);
      enabledStyle.Add(checkedStyle);
      
      // 没选中
      var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      unCheckedStyle.Setters.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
      enabledStyle.Add(unCheckedStyle);
      
      // 中间状态
      var indeterminateStyle = new Style(selector => selector.Nesting().Class($"{StdPseudoClass.Indeterminate}{StdPseudoClass.PointerOver}"));
      indeterminateStyle.Setters.Add(new Setter(CheckBox.IndicatorBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimaryHover)));
      enabledStyle.Add(indeterminateStyle);
      
      Add(enabledStyle);
   }
}