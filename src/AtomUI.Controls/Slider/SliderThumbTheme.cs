using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
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
        commonStyle.Add(InputElement.FocusableProperty, true);
        commonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        commonStyle.Add(TemplatedControl.BorderThicknessProperty, SliderTokenKey.ThumbCircleBorderThickness);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SliderTokenKey.ThumbCircleBorderColor);
        commonStyle.Add(SliderThumb.OutlineBrushProperty, SliderTokenKey.ThumbOutlineColor);
        commonStyle.Add(SliderThumb.OutlineThicknessProperty, new Thickness(0));
        commonStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderTokenKey.ThumbCircleSize);
        commonStyle.Add(Layoutable.WidthProperty, SliderTokenKey.ThumbSize);
        commonStyle.Add(Layoutable.HeightProperty, SliderTokenKey.ThumbSize);
        commonStyle.Add(Visual.ZIndexProperty, SliderThumb.NormalZIndex);

        var hoverOrFocusStyle = new Style(selector => Selectors.Or(selector.Nesting().Class(StdPseudoClass.PointerOver),
            selector.Nesting().Class(StdPseudoClass.Focus)));
        hoverOrFocusStyle.Add(TemplatedControl.BorderThicknessProperty,
            SliderTokenKey.ThumbCircleBorderThicknessHover);
        hoverOrFocusStyle.Add(TemplatedControl.BorderBrushProperty,
            SliderTokenKey.ThumbCircleBorderActiveColor);
        hoverOrFocusStyle.Add(SliderThumb.ThumbCircleSizeProperty, SliderTokenKey.ThumbCircleSizeHover);
        hoverOrFocusStyle.Add(SliderThumb.OutlineThicknessProperty, SliderTokenKey.ThumbOutlineThickness);
        commonStyle.Add(hoverOrFocusStyle);

        var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Focus));
        focusStyle.Add(Visual.ZIndexProperty, SliderThumb.FocusZIndex);
        commonStyle.Add(focusStyle);

        Add(commonStyle);
    }
}