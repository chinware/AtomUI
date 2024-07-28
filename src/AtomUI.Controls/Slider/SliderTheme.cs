using AtomUI.Styling;
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
         startSliderThumb.RegisterInNameScope(scope);
         
         var endSliderThumb = new SliderThumb
         {
            Name = EndThumbPart,
         };
         endSliderThumb.RegisterInNameScope(scope);
         
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
         
         sliderTrack.RegisterInNameScope(scope);
         return sliderTrack;
      });
   }

   protected override void BuildStyles()
   {
      var sliderStyle = new Style(selector => selector.Nesting());
      BuildCommonStyle(sliderStyle);
      BuildSliderTrackStyle(sliderStyle);
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
      sliderStyle.Add(sliderTrackStyle);

      var sliderTrackHorizontalStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Horizontal));
      sliderTrackHorizontalStyle.Add(SliderTrack.PaddingProperty, SliderResourceKey.SliderPaddingHorizontal);
      sliderStyle.Add(sliderTrackHorizontalStyle);

      var sliderTrackVerticalStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.Vertical));
      sliderTrackVerticalStyle.Add(SliderTrack.PaddingProperty, SliderResourceKey.SliderPaddingVertical);
      sliderStyle.Add(sliderTrackVerticalStyle);
      
      var sliderTrackHoverStyle = new Style(selector => selector.Nesting().Template().OfType<SliderTrack>().Class(StdPseudoClass.PointerOver));
      sliderTrackHoverStyle.Add(SliderTrack.TrackGrooveBrushProperty, SliderResourceKey.RailHoverBg);
      sliderTrackHoverStyle.Add(SliderTrack.TrackBarBrushProperty, SliderResourceKey.TrackHoverBg);
      sliderTrackHoverStyle.Add(SliderTrack.CursorProperty, new Cursor(StandardCursorType.Hand));
      sliderStyle.Add(sliderTrackHoverStyle);
   }
}