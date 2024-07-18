using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class SegmentedItemBoxTheme : ControlTheme
{
   public SegmentedItemBoxTheme()
      : base(typeof(SegmentedItemBox))
   {
   }
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<SegmentedItemBox>((box, scope) =>
      {
         var border = new Border()
         {
            Child = box.Item
         };
         CreateTemplateParentBinding(border, Border.BackgroundProperty, SegmentedItemBox.BackgroundProperty);
         CreateTemplateParentBinding(border, Border.CornerRadiusProperty, SegmentedItemBox.CornerRadiusProperty);
         return border;
      });
   }

   protected override void BuildStyles()
   {
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      largeSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalResourceKey.FontSizeLG);
      Add(largeSizeStyle);
      
      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      middleSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalResourceKey.FontSize);
      Add(middleSizeStyle);
      
      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
      smallSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalResourceKey.FontSize);
      Add(smallSizeStyle);
      
      // 没有被选择的正常状态
      var normalStyle = new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.IsEnabledProperty, true));
      
      // 选中状态
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedResourceKey.ItemSelectedColor);
      selectedStyle.Add(SegmentedItemBox.BackgroundProperty, GlobalResourceKey.ColorTransparent);
      normalStyle.Add(selectedStyle);
      
      // 没有被选中的状态
      var notSelectedStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
      notSelectedStyle.Add(SegmentedItemBox.BackgroundProperty, GlobalResourceKey.ColorTransparent);
      notSelectedStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedResourceKey.ItemColor);
      
      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(SegmentedItemBox.BackgroundProperty, SegmentedResourceKey.ItemHoverBg);
      hoverStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedResourceKey.ItemHoverColor);
      notSelectedStyle.Add(hoverStyle);
      
      // Pressed 状态
      var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
      pressedStyle.Add(SegmentedItemBox.BackgroundProperty, SegmentedResourceKey.ItemActiveBg);
      notSelectedStyle.Add(pressedStyle);
      
      normalStyle.Add(notSelectedStyle);
      Add(normalStyle);
      BuildDisabledStyle();
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(SegmentedItemBox.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}