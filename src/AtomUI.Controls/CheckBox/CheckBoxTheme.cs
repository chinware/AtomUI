using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CheckBoxTheme : BaseControlTheme
{
   public CheckBoxTheme()
      : base(typeof(CheckBox))
   {
   }
   
   protected override void BuildStyles()
   {
      this.Add(CheckBox.CheckIndicatorSizeProperty, CheckBoxResourceKey.CheckIndicatorSize);
      this.Add(CheckBox.PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      this.Add(CheckBox.IndicatorBorderRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      this.Add(CheckBox.IndicatorTristateMarkSizeProperty, CheckBoxResourceKey.IndicatorTristateMarkSize);
      this.Add(CheckBox.IndicatorTristateMarkBrushProperty, GlobalResourceKey.ColorPrimary);
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
 
   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsEnabledProperty, false));
      disableStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disableStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
      disableStyle.Add(CheckBox.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      
      var checkedStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsCheckedProperty, true));
      checkedStyle.Add(CheckBox.IndicatorCheckedMarkBrushProperty, GlobalResourceKey.ColorTextDisabled);
      disableStyle.Add(checkedStyle);

      var indeterminateStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsCheckedProperty, null));
      indeterminateStyle.Add(CheckBox.IndicatorTristateMarkBrushProperty, GlobalResourceKey.ColorTextDisabled);
      disableStyle.Add(indeterminateStyle);
      Add(disableStyle);
   }
   
   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(CheckBox.IsEnabledProperty, true));
      enabledStyle.Add(CheckBox.ForegroundProperty, GlobalResourceKey.ColorText);
      enabledStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainer);
      enabledStyle.Add(CheckBox.IndicatorCheckedMarkBrushProperty, GlobalResourceKey.ColorBgContainer);
      enabledStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
      
      // 选中
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimary);
      checkedStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimary);
      
      // 选中 hover
      var checkedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      checkedHoverStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimaryHover);
      checkedHoverStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      checkedStyle.Add(checkedHoverStyle);
      enabledStyle.Add(checkedStyle);
      
      // 没选中
      var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      unCheckedStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      enabledStyle.Add(unCheckedStyle);
      
      // 中间状态
      var indeterminateStyle = new Style(selector => selector.Nesting().Class($"{StdPseudoClass.Indeterminate}{StdPseudoClass.PointerOver}"));
      indeterminateStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      enabledStyle.Add(indeterminateStyle);
      
      Add(enabledStyle);
   }
}