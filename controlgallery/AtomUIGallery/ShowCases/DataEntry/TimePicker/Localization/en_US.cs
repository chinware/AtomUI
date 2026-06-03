using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TimePicker;

[LanguageProvider(LanguageCode.en_US, TimePickerShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Click TimePicker, and then we could select or input a time in panel.";
    public const string HourFormatsTitle = "12-hour and 24-hour formats";
    public const string HourFormatsDescription = "TimePicker supports two time types, 12-hour and 24-hour.";
    public const string ThreeSizesTitle = "Three Sizes";
    public const string ThreeSizesDescription = "The input box comes in three sizes: large, middle and small. Large is used in the form, while the medium size is the default.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "A disabled state of the TimePicker.";
    public const string IntervalOptionTitle = "Interval option";
    public const string IntervalOptionDescription = "Show stepped options by MinuteIncrement SecondIncrement.";
    public const string TwelveHoursTitle = "12 hours";
    public const string TwelveHoursDescription = "TimePicker of 12 hours format, with default format h:mm:ss a.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Bordered-less style component.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to TimePicker with status, which could be error or warning.";
    public const string TimeRangePickerTitle = "Time Range Picker";
    public const string TimeRangePickerDescription = "Use time range picker with RangeTimePicker.";
    public const string P2PlaceholderTextSelectTime = "Select time";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextStartTime = "Start time";
    public const string P2SecondaryPlaceholderTextEndTime = "End time";

    protected override Type GetResourceKindType() => typeof(TimePickerShowCaseLangResourceKind);
}
