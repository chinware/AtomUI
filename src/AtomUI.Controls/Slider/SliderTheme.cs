using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SliderTheme : ControlTheme
{
   public const string TrackPart = "PART_Track";
   public const string StartThumbPart = "PART_StartThumb";
   public const string EndThumbPart = "PART_EndThumb";
   
   public SliderTheme() : base(typeof(Slider))
   {
   }
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<Slider>((slider, scope) =>
      {
         var startSliderThumb = new SliderThumb
         {
            Name = StartThumbPart,
         };
         ToolTip.SetPlacement(startSliderThumb, PlacementMode.Top);
         ToolTip.SetShowDelay(startSliderThumb, 0);
         startSliderThumb.RegisterInNameScope(scope);
         
         var endSliderThumb = new SliderThumb
         {
            Name = EndThumbPart,
         };
         endSliderThumb.RegisterInNameScope(scope);
         ToolTip.SetPlacement(endSliderThumb, PlacementMode.Top);
         ToolTip.SetShowDelay(endSliderThumb, 0);
         
         var sliderTrack = new SliderTrack
         {
            Name = TrackPart,
            StartSliderThumb = startSliderThumb,
            EndSliderThumb = endSliderThumb
         };
         
         CreateTemplateParentBinding(sliderTrack, SliderTrack.IsDirectionReversedProperty, Slider.IsDirectionReversedProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.MinimumProperty, Slider.MinimumProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.MaximumProperty, Slider.MaximumProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.OrientationProperty, Slider.OrientationProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.ValueProperty, Slider.ValueProperty, BindingMode.TwoWay);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.RangeValueProperty, Slider.RangeValueProperty, BindingMode.TwoWay);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.IsRangeModeProperty, Slider.IsRangeModeProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.MarkLabelFontFamilyProperty, Slider.FontFamilyProperty);
         CreateTemplateParentBinding(sliderTrack, SliderTrack.MarkLabelFontSizeProperty, Slider.FontSizeProperty);
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
      sliderStyle.Add(Slider.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      sliderStyle.Add(Slider.VerticalAlignmentProperty, VerticalAlignment.Top);
      sliderStyle.Add(Slider.FocusableProperty, false);
      var verticalStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Vertical));
      verticalStyle.Add(Slider.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      verticalStyle.Add(Slider.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      sliderStyle.Add(verticalStyle);
   }

   private void BuildSliderTrackStyle(Style sliderStyle)
   {
      var sliderTrackStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>());
      sliderTrackStyle.Add(SliderTrack.TrackGrooveBrushProperty, SliderResourceKey.RailBg);
      sliderTrackStyle.Add(SliderTrack.TrackBarBrushProperty, SliderResourceKey.TrackBg);
      sliderTrackStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderResourceKey.MarkBorderColor);
      sliderTrackStyle.Add(SliderTrack.MarkBorderActiveBrushProperty, SliderResourceKey.MarkBorderColorActive);
      sliderStyle.Add(sliderTrackStyle);
      
      var sliderStyleHover = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      var thumbStyle = new Style(selector => selector.Nesting().Template().OfType<SliderThumb>().PropertyEquals(SliderThumb.IsFocusedProperty, false)
                                                     .Not(x => x.Class(StdPseudoClass.PointerOver)));
      thumbStyle.Add(SliderThumb.BorderBrushProperty, SliderResourceKey.ThumbCircleBorderHoverColor);
      sliderStyleHover.Add(thumbStyle);
      sliderStyle.Add(sliderStyleHover);
     

      var sliderTrackHorizontalStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Horizontal));
      sliderTrackHorizontalStyle.Add(SliderTrack.PaddingProperty, SliderResourceKey.SliderPaddingHorizontal);
      sliderStyle.Add(sliderTrackHorizontalStyle);

      var sliderTrackVerticalStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Vertical));
      sliderTrackVerticalStyle.Add(SliderTrack.PaddingProperty, SliderResourceKey.SliderPaddingVertical);
      sliderStyle.Add(sliderTrackVerticalStyle);
      
      var sliderTrackHoverStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.PointerOver));
      sliderTrackHoverStyle.Add(SliderTrack.TrackGrooveBrushProperty, SliderResourceKey.RailHoverBg);
      sliderTrackHoverStyle.Add(SliderTrack.TrackBarBrushProperty, SliderResourceKey.TrackHoverBg);
      sliderTrackHoverStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderResourceKey.MarkBorderColorHover);
      sliderTrackHoverStyle.Add(SliderTrack.CursorProperty, new Cursor(StandardCursorType.Hand));
      sliderStyle.Add(sliderTrackHoverStyle);
   }

   private void BuildDisabledStyle(Style sliderStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      
      var sliderTrackStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>());
      sliderTrackStyle.Add(SliderTrack.TrackBarBrushProperty, SliderResourceKey.TrackBgDisabled);
      sliderTrackStyle.Add(SliderTrack.MarkBorderActiveBrushProperty, SliderResourceKey.ThumbCircleBorderColorDisabled);
      sliderTrackStyle.Add(SliderTrack.MarkBorderBrushProperty, SliderResourceKey.ThumbCircleBorderColorDisabled);
      disabledStyle.Add(sliderTrackStyle);
      var thumbStyle = new Style(selector => selector.Nesting().Template().OfType<SliderThumb>());
      thumbStyle.Add(SliderThumb.BorderBrushProperty, SliderResourceKey.ThumbCircleBorderColorDisabled);
      disabledStyle.Add(thumbStyle);
      sliderStyle.Add(disabledStyle);
   }
}