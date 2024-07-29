using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SliderThumbTheme : BaseControlTheme
{
   public SliderThumbTheme() : base(typeof(SliderThumb))
   {
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(SliderThumb.FocusableProperty, true);
      commonStyle.Add(SliderThumb.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      commonStyle.Add(SliderThumb.BorderThicknessProperty, SliderResourceKey.ThumbCircleBorderThickness);
      commonStyle.Add(SliderThumb.BorderBrushProperty, SliderResourceKey.ThumbCircleBorderColor);
      commonStyle.Add(SliderThumb.OutlineBrushProperty, SliderResourceKey.ThumbOutlineColor);
      commonStyle.Add(SliderThumb.OutlineThicknessProperty, new Thickness(0));
      commonStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderResourceKey.ThumbCircleSize);
      commonStyle.Add(SliderThumb.WidthProperty, SliderResourceKey.ThumbSize);
      commonStyle.Add(SliderThumb.HeightProperty, SliderResourceKey.ThumbSize);
      commonStyle.Add(SliderThumb.ZIndexProperty, SliderThumb.NormalZIndex);

      var hoverOrFocusStyle = new Style(selector => Selectors.Or(selector.Nesting().Class(StdPseudoClass.PointerOver),
                                                                 selector.Nesting().Class(StdPseudoClass.Focus)));
      hoverOrFocusStyle.Add(SliderThumb.BorderThicknessProperty, SliderResourceKey.ThumbCircleBorderThicknessHover);
      hoverOrFocusStyle.Add(SliderThumb.BorderBrushProperty, SliderResourceKey.ThumbCircleBorderActiveColor);
      hoverOrFocusStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderResourceKey.ThumbCircleSizeHover);
      hoverOrFocusStyle.Add(SliderThumb.OutlineThicknessProperty, SliderResourceKey.ThumbOutlineThickness);
      commonStyle.Add(hoverOrFocusStyle);
      
      var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Focus));
      focusStyle.Add(SliderThumb.ZIndexProperty, SliderThumb.FocusZIndex);
      commonStyle.Add(focusStyle);
      
      Add(commonStyle);
   }
}