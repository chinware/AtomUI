using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class SegmentedItemBoxTheme : BaseControlTheme
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
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(SegmentedItemBox.TrackPaddingProperty, SegmentedTokenResourceKey.TrackPadding);
      // 没有被选择的正常状态
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.IsEnabledProperty, true));
      
      // 选中状态
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedTokenResourceKey.ItemSelectedColor);
      selectedStyle.Add(SegmentedItemBox.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      enabledStyle.Add(selectedStyle);
      
      // 没有被选中的状态
      var notSelectedStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
      notSelectedStyle.Add(SegmentedItemBox.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      notSelectedStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedTokenResourceKey.ItemColor);
      
      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(SegmentedItemBox.BackgroundProperty, SegmentedTokenResourceKey.ItemHoverBg);
      hoverStyle.Add(SegmentedItemBox.ForegroundProperty, SegmentedTokenResourceKey.ItemHoverColor);
      notSelectedStyle.Add(hoverStyle);
      
      // Pressed 状态
      var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
      pressedStyle.Add(SegmentedItemBox.BackgroundProperty, SegmentedTokenResourceKey.ItemActiveBg);
      notSelectedStyle.Add(pressedStyle);
      
      enabledStyle.Add(notSelectedStyle);
      commonStyle.Add(enabledStyle);
      Add(commonStyle);

      BuildSizeTypeStyle();
      BuildDisabledStyle();
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      largeSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
      largeSizeStyle.Add(SegmentedItemBox.ControlHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
      largeSizeStyle.Add(SegmentedItemBox.SegmentedItemPaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
      Add(largeSizeStyle);
      
      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      middleSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      middleSizeStyle.Add(SegmentedItemBox.ControlHeightProperty, GlobalTokenResourceKey.ControlHeight);
      middleSizeStyle.Add(SegmentedItemBox.SegmentedItemPaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
      Add(middleSizeStyle);
      
      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItemBox.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(SegmentedItemBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
      smallSizeStyle.Add(SegmentedItemBox.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      smallSizeStyle.Add(SegmentedItemBox.ControlHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
      smallSizeStyle.Add(SegmentedItemBox.SegmentedItemPaddingProperty, SegmentedTokenResourceKey.SegmentedItemPaddingSM);
      
      Add(smallSizeStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(SegmentedItemBox.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}