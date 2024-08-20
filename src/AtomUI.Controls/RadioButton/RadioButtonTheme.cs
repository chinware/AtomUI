using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RadioButtonTheme : BaseControlTheme
{
   public RadioButtonTheme()
      : base(typeof(RadioButton))
   {
   }
   
   protected override void BuildStyles()
   {
      this.Add(RadioButton.RadioSizeProperty, RadioButtonTokenResourceKey.RadioSize);
      this.Add(RadioButton.PaddingInlineProperty, GlobalTokenResourceKey.PaddingXS);
      this.Add(RadioButton.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      this.Add(RadioButton.DotSizeValueProperty, RadioButtonTokenResourceKey.DotSize);
      this.Add(RadioButton.DotPaddingProperty, RadioButtonTokenResourceKey.DotPadding);
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
   
   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, false));
      disableStyle.Add(RadioButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disableStyle.Add(RadioButton.RadioBackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
      disableStyle.Add(RadioButton.RadioInnerBackgroundProperty, RadioButtonTokenResourceKey.DotColorDisabled);
      Add(disableStyle);
   }

   private void BuildEnabledStyle()
   {
      
       var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, true));
      enabledStyle.Add(RadioButton.RadioInnerBackgroundProperty, RadioButtonTokenResourceKey.RadioColor);

      
      // 选中
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(RadioButton.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      checkedStyle.Add(RadioButton.RadioBackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
      
      enabledStyle.Add(checkedStyle);
      
      // 没选中
      var unCheckedStyle = new Style(selector => selector.Nesting().Not(x=> x.Nesting().Class(StdPseudoClass.Checked)));
      unCheckedStyle.Add(RadioButton.RadioBackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      var unCheckedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      unCheckedHoverStyle.Add(RadioButton.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      unCheckedStyle.Add(unCheckedHoverStyle);
      
      enabledStyle.Add(unCheckedStyle);
      
      Add(enabledStyle);
   }
}