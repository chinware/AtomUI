using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.en_US, SliderShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Select a numeric value or range from a continuous or graduated track.";
    public const string PageDescription = "Slider supports single-value and range selection, horizontal or vertical orientation, snap-to-tick behavior, formatted tooltips, marks, included tracks, disabled states, and keyboard interaction.";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic slider. When range is true, display as dual thumb mode. When disable is true, the slider will not be interactable.";
    public const string RangeValueBindingTitle = "RangeValue binding";
    public const string RangeValueBindingDescription = "RangeValue now defaults to TwoWay binding, so range changes update the ViewModel without an explicit binding mode.";
    public const string CustomizeTooltipTitle = "Customize tooltip";
    public const string CustomizeTooltipDescription = "Use tooltip.formatter to format content of Tooltip. If tooltip.formatter is null, hide it.";
    public const string VerticalTitle = "Vertical";
    public const string VerticalDescription = "The vertical Slider.";
    public const string GraduatedSliderTitle = "Graduated slider";
    public const string GraduatedSliderDescription = "Using marks property to mark a graduated slider, use value or defaultValue to specify the position of thumb. When included is false, means that different thumbs are coordinative. when step is null, users can only slide the thumbs onto marks.";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";
    public const string P2TextBoundRangeValue = "Bound range:";
    public const string P2ContentSetRange = "Set 35-85";
    public const string P2ContentClear = "Clear";

}
