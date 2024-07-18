using AtomUI.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RadioButtonTheme : ControlTheme
{
   public RadioButtonTheme()
      : base(typeof(RadioButton))
   {
   }
   
   protected override void BuildStyles()
   {
      this.Add(RadioButton.RadioSizeProperty, RadioButtonResourceKey.RadioSize);
      this.Add(RadioButton.PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      this.Add(RadioButton.RadioBorderBrushProperty, GlobalResourceKey.ColorBorder);
      
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
   
   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, false));
      disableStyle.Add(RadioButton.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disableStyle.Add(RadioButton.RadioBackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disableStyle.Add(RadioButton.RadioInnerBackgroundProperty, RadioButtonResourceKey.DotColorDisabled);
      Add(disableStyle);
   }

   private void BuildEnabledStyle()
   {
      
       var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, true));
      enabledStyle.Add(RadioButton.RadioInnerBackgroundProperty, RadioButtonResourceKey.RadioColor);

      
      // 选中
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(RadioButton.RadioBorderBrushProperty, GlobalResourceKey.ColorPrimary);
      checkedStyle.Add(RadioButton.RadioBackgroundProperty, GlobalResourceKey.ColorPrimary);
      
      enabledStyle.Add(checkedStyle);
      
      // 没选中
      var unCheckedStyle = new Style(selector => selector.Nesting().Not(x=> x.Nesting().Class(StdPseudoClass.Checked)));
      unCheckedStyle.Add(RadioButton.RadioBackgroundProperty, GlobalResourceKey.ColorBgContainer);
      var unCheckedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      unCheckedHoverStyle.Add(RadioButton.RadioBorderBrushProperty, GlobalResourceKey.ColorPrimary);
      unCheckedStyle.Add(unCheckedHoverStyle);
      
      enabledStyle.Add(unCheckedStyle);
      
      Add(enabledStyle);
   }
}