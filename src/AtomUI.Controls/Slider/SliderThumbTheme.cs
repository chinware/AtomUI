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
      commonStyle.Add(SliderThumb.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(SliderThumb.BorderThicknessProperty, SliderTokenResourceKey.ThumbCircleBorderThickness);
      commonStyle.Add(SliderThumb.BorderBrushProperty, SliderTokenResourceKey.ThumbCircleBorderColor);
      commonStyle.Add(SliderThumb.OutlineBrushProperty, SliderTokenResourceKey.ThumbOutlineColor);
      commonStyle.Add(SliderThumb.OutlineThicknessProperty, new Thickness(0));
      commonStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderTokenResourceKey.ThumbCircleSize);
      commonStyle.Add(SliderThumb.WidthProperty, SliderTokenResourceKey.ThumbSize);
      commonStyle.Add(SliderThumb.HeightProperty, SliderTokenResourceKey.ThumbSize);
      commonStyle.Add(SliderThumb.ZIndexProperty, SliderThumb.NormalZIndex);

      var hoverOrFocusStyle = new Style(selector => Selectors.Or(selector.Nesting().Class(StdPseudoClass.PointerOver),
                                                                 selector.Nesting().Class(StdPseudoClass.Focus)));
      hoverOrFocusStyle.Add(SliderThumb.BorderThicknessProperty, SliderTokenResourceKey.ThumbCircleBorderThicknessHover);
      hoverOrFocusStyle.Add(SliderThumb.BorderBrushProperty, SliderTokenResourceKey.ThumbCircleBorderActiveColor);
      hoverOrFocusStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderTokenResourceKey.ThumbCircleSizeHover);
      hoverOrFocusStyle.Add(SliderThumb.OutlineThicknessProperty, SliderTokenResourceKey.ThumbOutlineThickness);
      commonStyle.Add(hoverOrFocusStyle);
      
      var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Focus));
      focusStyle.Add(SliderThumb.ZIndexProperty, SliderThumb.FocusZIndex);
      commonStyle.Add(focusStyle);
      
      Add(commonStyle);
   }
}