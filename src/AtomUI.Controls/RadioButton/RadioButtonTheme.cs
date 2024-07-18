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
      Add(new Setter(RadioButton.RadioSizeProperty, new DynamicResourceExtension(RadioButtonResourceKey.RadioSize)));
      Add(new Setter(RadioButton.PaddingInlineProperty, new DynamicResourceExtension(GlobalResourceKey.PaddingXS)));
      Add(new Setter(RadioButton.RadioBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBorder)));
      
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
   
   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, false));
      disableStyle.Add(new Setter(RadioButton.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      disableStyle.Add(new Setter(RadioButton.RadioBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainerDisabled)));
      disableStyle.Add(new Setter(RadioButton.RadioInnerBackgroundProperty, new DynamicResourceExtension(RadioButtonResourceKey.DotColorDisabled)));
      Add(disableStyle);
   }

   private void BuildEnabledStyle()
   {
      
       var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(RadioButton.IsEnabledProperty, true));
      enabledStyle.Add(new Setter(RadioButton.RadioInnerBackgroundProperty, new DynamicResourceExtension(RadioButtonResourceKey.RadioColor)));

      
      // 选中
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(new Setter(RadioButton.RadioBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      checkedStyle.Add(new Setter(RadioButton.RadioBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      
      enabledStyle.Add(checkedStyle);
      
      // 没选中
      var unCheckedStyle = new Style(selector => selector.Nesting().Not(x=> x.Nesting().Class(StdPseudoClass.Checked)));
      unCheckedStyle.Add(new Setter(RadioButton.RadioBackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBgContainer)));
      var unCheckedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      unCheckedHoverStyle.Add(new Setter(RadioButton.RadioBorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
      unCheckedStyle.Add(unCheckedHoverStyle);
      
      enabledStyle.Add(unCheckedStyle);
      
      Add(enabledStyle);
   }
}