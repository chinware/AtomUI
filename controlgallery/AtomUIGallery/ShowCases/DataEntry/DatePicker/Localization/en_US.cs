using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.DatePicker;

[LanguageProvider(LanguageCode.en_US, DatePickerShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Click DatePicker, and then we could select or input a date in panel.";
    public const string BindingTitle = "SelectedDateTime binding";
    public const string BindingDescription = "SelectedDateTime, RangeStartSelectedDate, and RangeEndSelectedDate synchronize with the ViewModel without explicitly setting Binding Mode=TwoWay.";
    public const string PickerDisplayDateTitle = "Popup display date";
    public const string PickerDisplayDateDescription = "Open the popup panel at a specific display date without committing a selected value.";
    public const string RangePickerTitle = "Range Picker";
    public const string RangePickerDescription = "Set range picker type by picker prop.";
    public const string NeedConfirmTitle = "Need Confirm";
    public const string NeedConfirmDescription = "DatePicker will automatically determine whether to show a confirm button according to the picker property. You can also set the needConfirm property to determine whether to show a confirm button. When needConfirm is set, the user must click the confirm button to complete the selection. Otherwise, the selection will be submitted when the picker loses focus or selects a date.";
    public const string ChooseTimeTitle = "Choose Time";
    public const string ChooseTimeDescription = "This property provides an additional time selection. When showTime is an Object, its properties will be passed on to the built-in TimePicker.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "A disabled state of the DatePicker. You can also set as array to disable one of input.";
    public const string ThreeSizesTitle = "Three Sizes";
    public const string ThreeSizesDescription = "The input box comes in three sizes: small, middle and large. The middle size will be used if size is omitted.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to DatePicker with status, which could be error or warning.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Bordered-less style component.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "You can manually specify the position of the popup via placement.";
    public const string P2PlaceholderTextSelectDate = "Select date";
    public const string P2PlaceholderTextSelectWeek = "Select week";
    public const string P2PlaceholderTextSelectMonth = "Select month";
    public const string P2PlaceholderTextSelectQuarter = "Select quarter";
    public const string P2PlaceholderTextSelectYear = "Select year";
    public const string P2SecondaryPlaceholderTextEndDate = "End date";
    public const string P2PlaceholderTextSelectTime = "Select time";
    public const string P2PlaceholderTextStartDate = "Start date";
    public const string P2PlaceholderTextStartWeek = "Start week";
    public const string P2SecondaryPlaceholderTextEndWeek = "End week";
    public const string P2PlaceholderTextStartMonth = "Start month";
    public const string P2SecondaryPlaceholderTextEndMonth = "End month";
    public const string P2PlaceholderTextStartQuarter = "Start quarter";
    public const string P2SecondaryPlaceholderTextEndQuarter = "End quarter";
    public const string P2PlaceholderTextStartYear = "Start year";
    public const string P2SecondaryPlaceholderTextEndYear = "End year";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2TextExpandDirection = "Picker size:";
    public const string P2ContentLarge = "Large";
    public const string P2ContentDefault = "Default";
    public const string P2ContentSmall = "Small";
    public const string P2ContentCustom = "Custom";
    public const string P2TextPlacement = "Placement:";
    public const string P2TextSelectedDateTime = "Selected value:";
    public const string P2TextSelectedDateRange = "Selected range:";
    public const string P2ContentSetTomorrow = "Set tomorrow";
    public const string P2ContentSetThisWeek = "Set this week";
    public const string P2ContentClear = "Clear";
    public const string P2ContentTopleft = "TopLeft";
    public const string P2ContentTopright = "TopRight";
    public const string P2ContentBottomleft = "BottomLeft";
    public const string P2ContentBottomright = "BottomRight";
    public const string PageSubtitle = "Select dates, ranges, and optional times from calendar panels.";
    public const string PageDescription = "DatePicker supports single and range selection, confirmation flows, time selection, disabled states, size variants, validation status, visual variants, and custom popup placement.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string ScenarioExamples = "Examples";

}
