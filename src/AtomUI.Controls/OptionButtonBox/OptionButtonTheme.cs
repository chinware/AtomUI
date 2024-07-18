using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class OptionButtonTheme : ControlTheme
{
   public OptionButtonTheme()
      : base(typeof(OptionButton))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<OptionButton>((button, scope) =>
      {
         // TODO 暂时没有支持带 Icon，后面支持
         var stackPanel = new StackPanel()
         {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
         };
         var label = new Label()
         {
            Padding = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
         };

         stackPanel.Children.Add(label);
         CreateTemplateParentBinding(label, Label.ContentProperty, OptionButton.TextProperty);
         return stackPanel;
      });
   }
   
   protected override void BuildStyles()
   {
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(OptionButton.MinHeightProperty, GlobalResourceKey.ControlHeightLG);
      largeSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonResourceKey.ContentFontSizeLG);
      largeSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonResourceKey.PaddingLG);
      largeSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      Add(largeSizeStyle);
      
      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(OptionButton.MinHeightProperty, GlobalResourceKey.ControlHeight);
      middleSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonResourceKey.ContentFontSize);
      middleSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonResourceKey.Padding);
      middleSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      Add(middleSizeStyle);
      
      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(OptionButton.MinHeightProperty, GlobalResourceKey.ControlHeightSM);
      smallSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonResourceKey.ContentFontSizeSM);
      smallSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonResourceKey.PaddingSM);
      smallSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      
      Add(smallSizeStyle);

      BuildSolidStyle();
      BuildOutlineStyle();
   }

   private void BuildSolidStyle()
   {
      var solidStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.ButtonStyleProperty, OptionButtonStyle.Solid));
      BuildSolidEnabledStyle(solidStyle);
      BuildSolidDisabledStyle(solidStyle);
      Add(solidStyle);
   }
   
   private void BuildOutlineStyle()
   {
      var outlineStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.ButtonStyleProperty, OptionButtonStyle.Outline));
      BuildOutlineEnabledStyle(outlineStyle);
      BuildOutlineDisabledStyle(outlineStyle);
      Add(outlineStyle);
   }

   private void BuildSolidEnabledStyle(Style solidStyle)
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, true));
      // 选中状态
      {
         var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
         checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonResourceKey.ButtonSolidCheckedColor);
         checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonSolidCheckedBackground);
         
         // Hover 状态
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonSolidCheckedHoverBackground);
         checkedStyle.Add(hoverStyle);
      
         // Pressed 状态
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
         pressedStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonSolidCheckedActiveBackground);
         checkedStyle.Add(pressedStyle);
         
         enabledStyle.Add(checkedStyle);
      }
      
      // 没选中状态
      {
         var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.UnChecked));
         unCheckedStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorText);
         unCheckedStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorTransparent);

         unCheckedStyle.Add(inOptionGroupStyle);
         
         // Hover 状态
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorPrimaryHover);
         unCheckedStyle.Add(hoverStyle);
      
         // Pressed 状态
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
         pressedStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorPrimaryActive);
         unCheckedStyle.Add(pressedStyle);
         
         enabledStyle.Add(unCheckedStyle);
      }
      
      solidStyle.Add(enabledStyle);
   }
   
   private void BuildSolidDisabledStyle(Style solidStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, false));
      
      disabledStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      disabledStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonResourceKey.ButtonCheckedColorDisabled);
      checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonCheckedColorDisabled);
      disabledStyle.Add(checkedStyle);
      
      solidStyle.Add(disabledStyle);
   }

   private void BuildOutlineEnabledStyle(Style outlineStyle)
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, true));
      
      // 选中状态
      {
         var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
         checkedStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorPrimary);
         checkedStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorPrimary);
         checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonBackground);
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorTransparent);

         checkedStyle.Add(inOptionGroupStyle);
         
         enabledStyle.Add(checkedStyle);
         outlineStyle.Add(enabledStyle);
      }
      // 没选中状态
      {
         enabledStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorBorder);
         enabledStyle.Add(OptionButton.ForegroundProperty, OptionButtonResourceKey.ButtonColor);
         enabledStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBackground);
         
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorTransparent);
         enabledStyle.Add(inOptionGroupStyle);
      }
      
      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      hoverStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorPrimaryHover);
      enabledStyle.Add(hoverStyle);
      
      // Pressed 状态
      var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
      pressedStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive);
      pressedStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorPrimaryActive);
      enabledStyle.Add(pressedStyle);
   }
   
   private void BuildOutlineDisabledStyle(Style outlineStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, false));

      disabledStyle.Add(OptionButton.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      disabledStyle.Add(OptionButton.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(OptionButton.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonResourceKey.ButtonCheckedColorDisabled);
      checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBgDisabled);
      disabledStyle.Add(checkedStyle);
      
      outlineStyle.Add(disabledStyle);
   }
}