using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SliderTheme : BaseControlTheme
{
    public const string TrackPart = "PART_Track";
    public const string StartThumbPart = "PART_StartThumb";
    public const string EndThumbPart = "PART_EndThumb";

    public SliderTheme() : base(typeof(Slider))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Slider>((slider, scope) =>
        {
            var startSliderThumb = new SliderThumb
            {
                Name = StartThumbPart
            };
            CreateTemplateParentBinding(startSliderThumb, SliderThumb.IsMotionEnabledProperty,
                Slider.IsMotionEnabledProperty);
            Avalonia.Controls.ToolTip.SetPlacement(startSliderThumb, PlacementMode.Top);
            Avalonia.Controls.ToolTip.SetShowDelay(startSliderThumb, 0);
            startSliderThumb.RegisterInNameScope(scope);

            var endSliderThumb = new SliderThumb
            {
                Name = EndThumbPart
            };
            CreateTemplateParentBinding(endSliderThumb, SliderThumb.IsMotionEnabledProperty,
                Slider.IsMotionEnabledProperty);
            
            endSliderThumb.RegisterInNameScope(scope);
            Avalonia.Controls.ToolTip.SetPlacement(endSliderThumb, PlacementMode.Top);
            Avalonia.Controls.ToolTip.SetShowDelay(endSliderThumb, 0);

            var sliderTrack = new SliderTrack
            {
                Name             = TrackPart,
                StartSliderThumb = startSliderThumb,
                EndSliderThumb   = endSliderThumb
            };

            CreateTemplateParentBinding(sliderTrack, SliderTrack.IsMotionEnabledProperty,
                Slider.IsMotionEnabledProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.IsEnabledProperty,
                Slider.IsEnabledProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.IsDirectionReversedProperty,
                Slider.IsDirectionReversedProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.MinimumProperty, RangeBase.MinimumProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.MaximumProperty, RangeBase.MaximumProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.OrientationProperty, Slider.OrientationProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.ValueProperty, RangeBase.ValueProperty,
                BindingMode.TwoWay);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.RangeValueProperty, Slider.RangeValueProperty,
                BindingMode.TwoWay);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.IsRangeModeProperty, Slider.IsRangeModeProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.MarkLabelFontFamilyProperty,
                TemplatedControl.FontFamilyProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.MarkLabelFontSizeProperty,
                TemplatedControl.FontSizeProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.MarksProperty, Slider.MarksProperty);
            CreateTemplateParentBinding(sliderTrack, SliderTrack.IncludedProperty, Slider.IncludedProperty);

            sliderTrack.RegisterInNameScope(scope);
            return sliderTrack;
        });
    }

    protected override void BuildStyles()
    {
        var sliderStyle = new Style(selector => selector.Nesting());
        BuildCommonStyle(sliderStyle);
        BuildSliderTrackStyle(sliderStyle);
        BuildDisabledStyle(sliderStyle);
        Add(sliderStyle);
    }

    private void BuildCommonStyle(Style sliderStyle)
    {
        sliderStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        sliderStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
        sliderStyle.Add(InputElement.FocusableProperty, false);
        var verticalStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Vertical));
        verticalStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        verticalStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        sliderStyle.Add(verticalStyle);
    }

    private void BuildSliderTrackStyle(Style sliderStyle)
    {
        var sliderTrackStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>());
        sliderTrackStyle.Add(SliderTrack.TrackGrooveBrushProperty, SliderTokenKey.RailBg);
        sliderTrackStyle.Add(SliderTrack.TrackBarBrushProperty, SliderTokenKey.TrackBg);
        sliderTrackStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderTokenKey.MarkBorderColor);
        sliderTrackStyle.Add(SliderTrack.MarkBorderActiveBrushProperty, SliderTokenKey.MarkBorderColorActive);
        sliderStyle.Add(sliderTrackStyle);

        var sliderStyleHover = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        var thumbStyle = new Style(selector => selector
                                               .Nesting().Template().OfType<SliderThumb>()
                                               .PropertyEquals(InputElement.IsFocusedProperty, false)
                                               .Not(x => x.Class(StdPseudoClass.PointerOver)));
        thumbStyle.Add(TemplatedControl.BorderBrushProperty, SliderTokenKey.ThumbCircleBorderHoverColor);
        sliderStyleHover.Add(thumbStyle);
        sliderStyle.Add(sliderStyleHover);

        var sliderTrackHorizontalStyle = new Style(selector =>
            selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Horizontal));
        sliderTrackHorizontalStyle.Add(SliderTrack.PaddingProperty, SliderTokenKey.SliderPaddingHorizontal);
        sliderStyle.Add(sliderTrackHorizontalStyle);

        var sliderTrackVerticalStyle = new Style(selector =>
            selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Vertical));
        sliderTrackVerticalStyle.Add(SliderTrack.PaddingProperty, SliderTokenKey.SliderPaddingVertical);
        sliderStyle.Add(sliderTrackVerticalStyle);

        var sliderTrackHoverStyle = new Style(selector =>
            selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.PointerOver));
        sliderTrackHoverStyle.Add(SliderTrack.TrackGrooveBrushProperty, SliderTokenKey.RailHoverBg);
        sliderTrackHoverStyle.Add(SliderTrack.TrackBarBrushProperty, SliderTokenKey.TrackHoverBg);
        sliderTrackHoverStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderTokenKey.MarkBorderColorHover);
        sliderTrackHoverStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        sliderStyle.Add(sliderTrackHoverStyle);
    }

    private void BuildDisabledStyle(Style sliderStyle)
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));

        var sliderTrackStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>());
        sliderTrackStyle.Add(SliderTrack.TrackBarBrushProperty, SliderTokenKey.TrackBgDisabled);
        sliderTrackStyle.Add(SliderTrack.MarkBorderActiveBrushProperty, SliderTokenKey.ThumbCircleBorderColorDisabled);
        sliderTrackStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderTokenKey.ThumbCircleBorderColorDisabled);
        disabledStyle.Add(sliderTrackStyle);
        var thumbStyle = new Style(selector => selector.Nesting().Template().OfType<SliderThumb>());
        thumbStyle.Add(TemplatedControl.BorderBrushProperty, SliderTokenKey.ThumbCircleBorderColorDisabled);
        disabledStyle.Add(thumbStyle);
        sliderStyle.Add(disabledStyle);
    }
}