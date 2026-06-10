using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ColorPicker;

[LanguageProvider(LanguageCode.en_US, ColorPickerShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic Usage.";
    public const string TriggerSizeTitle = "Trigger size";
    public const string TriggerSizeDescription = "Ant Design supports three trigger sizes: small, default and large.If a large or small trigger is desired, set the size property to either large or small.respectively. Omit the size property for a trigger with the default size.";
    public const string LineGradientTitle = "Line Gradient";
    public const string LineGradientDescription = "Set the color to a single or a gradient color via mode.";
    public const string RenderingTriggerTextTitle = "Rendering Trigger Text";
    public const string RenderingTriggerTextDescription = "Renders the default text of the trigger, effective when showText is true. When customizing text, you can use showText as a function to return custom text.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Set to disabled state.";
    public const string DisabledAlphaTitle = "Disabled Alpha";
    public const string DisabledAlphaDescription = "Disabled color alpha.";
    public const string ClearColorTitle = "Clear Color";
    public const string ClearColorDescription = "Clear Color.";
    public const string ControlledModeTitle = "controlled mode";
    public const string ControlledModeDescription = "Set the component to controlled mode. Will lock the display color if controlled by.";
    public const string CustomTriggerEventTitle = "Custom Trigger Event";
    public const string CustomTriggerEventDescription = "Triggers event for customizing color panels, provide options click and hover.";
    public const string ColorFormatTitle = "Color Format";
    public const string ColorFormatDescription = "Encoding formats, support HEX, HSB, RGB.";
    public const string PresetColorsTitle = "Preset Colors";
    public const string PresetColorsDescription = "Set the presets color of the color picker.";

    protected override Type GetResourceKindType() => typeof(ColorPickerShowCaseLangResourceKind);
}
