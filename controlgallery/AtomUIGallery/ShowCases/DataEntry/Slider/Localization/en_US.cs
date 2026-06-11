using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.en_US, SliderShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic slider. When range is true, display as dual thumb mode. When disable is true, the slider will not be interactable.";
    public const string CustomizeTooltipTitle = "Customize tooltip";
    public const string CustomizeTooltipDescription = "Use tooltip.formatter to format content of Tooltip. If tooltip.formatter is null, hide it.";
    public const string VerticalTitle = "Vertical";
    public const string VerticalDescription = "The vertical Slider.";
    public const string GraduatedSliderTitle = "Graduated slider";
    public const string GraduatedSliderDescription = "Using marks property to mark a graduated slider, use value or defaultValue to specify the position of thumb. When included is false, means that different thumbs are coordinative. when step is null, users can only slide the thumbs onto marks.";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";

    protected override Type GetResourceKindType() => typeof(SliderShowCaseLangResourceKind);
}
