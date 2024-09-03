using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class OptionButtonTheme : BaseControlTheme
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
      largeSizeStyle.Add(OptionButton.MinHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
      largeSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSizeLG);
      largeSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonTokenResourceKey.PaddingLG);
      largeSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
      Add(largeSizeStyle);
      
      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(OptionButton.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
      middleSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSize);
      middleSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonTokenResourceKey.Padding);
      middleSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      Add(middleSizeStyle);
      
      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(OptionButton.MinHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
      smallSizeStyle.Add(OptionButton.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSizeSM);
      smallSizeStyle.Add(OptionButton.PaddingProperty, OptionButtonTokenResourceKey.PaddingSM);
      smallSizeStyle.Add(OptionButton.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      
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
      outlineStyle.Add(OptionButton.BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness);
      var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
      inOptionGroupStyle.Add(OptionButton.BorderThicknessProperty, new Thickness(0));
      outlineStyle.Add(inOptionGroupStyle);
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
         checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonTokenResourceKey.ButtonSolidCheckedColor);
         checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonSolidCheckedBackground);
         
         // Hover 状态
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonSolidCheckedHoverBackground);
         checkedStyle.Add(hoverStyle);
      
         // Pressed 状态
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
         pressedStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonSolidCheckedActiveBackground);
         checkedStyle.Add(pressedStyle);
         
         enabledStyle.Add(checkedStyle);
      }
      
      // 没选中状态
      {
         var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.UnChecked));
         unCheckedStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorText);
         unCheckedStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);

         unCheckedStyle.Add(inOptionGroupStyle);
         
         // Hover 状态
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
         unCheckedStyle.Add(hoverStyle);
      
         // Pressed 状态
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
         pressedStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryActive);
         unCheckedStyle.Add(pressedStyle);
         
         enabledStyle.Add(unCheckedStyle);
      }
      
      solidStyle.Add(enabledStyle);
   }
   
   private void BuildSolidDisabledStyle(Style solidStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, false));
      
      disabledStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      disabledStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
      
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
      checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
      disabledStyle.Add(checkedStyle);
      
      solidStyle.Add(disabledStyle);
   }

   private void BuildOutlineEnabledStyle(Style outlineStyle)
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, true));
      
      // 选中状态
      {
         var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
         checkedStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
         checkedStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimary);
         checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonBackground);
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);

         checkedStyle.Add(inOptionGroupStyle);
         
         enabledStyle.Add(checkedStyle);
         outlineStyle.Add(enabledStyle);
      }
      // 没选中状态
      {
         enabledStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
         enabledStyle.Add(OptionButton.ForegroundProperty, OptionButtonTokenResourceKey.ButtonColor);
         enabledStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedBackground);
         
         var inOptionGroupStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
         inOptionGroupStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
         enabledStyle.Add(inOptionGroupStyle);
      }
      
      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
      hoverStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
      enabledStyle.Add(hoverStyle);
      
      // Pressed 状态
      var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
      pressedStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryActive);
      pressedStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorPrimaryActive);
      enabledStyle.Add(pressedStyle);
   }
   
   private void BuildOutlineDisabledStyle(Style outlineStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(OptionButton.IsEnabledProperty, false));

      disabledStyle.Add(OptionButton.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      disabledStyle.Add(OptionButton.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(OptionButton.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
      
      var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
      checkedStyle.Add(OptionButton.ForegroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
      checkedStyle.Add(OptionButton.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedBgDisabled);
      disabledStyle.Add(checkedStyle);
      
      outlineStyle.Add(disabledStyle);
   }
}