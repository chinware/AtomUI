using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls.Switch;

[ControlThemeProvider]
public class ToggleSwitchTheme : ControlTheme
{
   public const string SwitchKnobPart = "PART_SwitchKnob";
   public const string MainContainerPart = "PART_MainContainer";
   
   public ToggleSwitchTheme()
      : base(typeof(ToggleSwitch))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<ToggleSwitch>((toggleSwitch, scope) =>
      {
         var layout = new Canvas()
         {
            Name = MainContainerPart
         };
         var switchKnob = new SwitchKnob()
         {
            Name = SwitchKnobPart
         };
         CreateTemplateParentBinding(switchKnob, SwitchKnob.IsCheckedStateProperty, ToggleSwitch.IsCheckedProperty);
         switchKnob.RegisterInNameScope(scope);
         
         layout.Children.Add(switchKnob);
         layout.RegisterInNameScope(scope);
         return layout;
      });
   }

   protected override void BuildStyles()
   {
      this.Add(ToggleSwitch.ForegroundProperty, GlobalResourceKey.ColorTextLightSolid);
      this.Add(ToggleSwitch.TrackPaddingProperty, ToggleSwitchResourceKey.TrackPadding);
      BuildSizeTypeStyle();
      BuildEnabledStyle();

      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsEnabledProperty, false));
      disabledStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchResourceKey.SwitchDisabledOpacity);
      Add(disabledStyle);

      var loadingStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsLoadingProperty, true));
      loadingStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchResourceKey.SwitchDisabledOpacity);
      Add(loadingStyle);
   }
   
   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      enabledStyle.Add(ToggleSwitch.SwitchOpacityProperty, 1d);
      var checkedStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsCheckedProperty, true));
      checkedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, ToggleSwitchResourceKey.SwitchColor);
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalResourceKey.ColorPrimaryHover);
         checkedStyle.Add(hoverStyle);
      }
      enabledStyle.Add(checkedStyle);
      
      var unCheckedStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsCheckedProperty, false));
      unCheckedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalResourceKey.ColorTextQuaternary);
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalResourceKey.ColorTextTertiary);
         unCheckedStyle.Add(hoverStyle);
      }
      enabledStyle.Add(unCheckedStyle);
      Add(enabledStyle);
   }

   private void BuildSizeTypeStyle()
   {
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(ToggleSwitch.FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSizeSM);
      {
         var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
         knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchResourceKey.HandleSizeSM);
         knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchResourceKey.HandleSizeSM);
         smallSizeStyle.Add(knobSizeStyle);
      }
      smallSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchResourceKey.InnerMaxMarginSM);
      smallSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchResourceKey.InnerMinMarginSM);
      smallSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchResourceKey.TrackHeightSM);
      smallSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchResourceKey.TrackMinWidthSM);
      smallSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchResourceKey.IconSizeSM);
      
      Add(smallSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(ToggleSwitch.FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSize);
      {
         var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
         knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchResourceKey.HandleSize);
         knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchResourceKey.HandleSize);
         middleSizeStyle.Add(knobSizeStyle);
      }
      middleSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchResourceKey.InnerMaxMargin);
      middleSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchResourceKey.InnerMinMargin);
      middleSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchResourceKey.TrackHeight);
      middleSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchResourceKey.TrackMinWidth);
      middleSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchResourceKey.IconSize);
      Add(middleSizeStyle);
      
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(ToggleSwitch.FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSize);
      {
         var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
         knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchResourceKey.HandleSize);
         knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchResourceKey.HandleSize);
         largeSizeStyle.Add(knobSizeStyle);
      }
      largeSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchResourceKey.InnerMaxMargin);
      largeSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchResourceKey.InnerMinMargin);
      largeSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchResourceKey.TrackHeight);
      largeSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchResourceKey.TrackMinWidth);
      largeSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchResourceKey.IconSize);
      Add(largeSizeStyle);

      {
         var switchKnobStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
         switchKnobStyle.Add(SwitchKnob.KnobBackgroundColorProperty, ToggleSwitchResourceKey.HandleBg);
         switchKnobStyle.Add(SwitchKnob.KnobBoxShadowProperty, ToggleSwitchResourceKey.HandleShadow);
         switchKnobStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, GlobalResourceKey.ColorTextQuaternary);
         var checkedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SwitchKnob.IsCheckedStateProperty, true));
         checkedStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, ToggleSwitchResourceKey.SwitchColor);
         switchKnobStyle.Add(checkedStyle);
         Add(switchKnobStyle);
      }
   }
}