using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DatePicker;

[LanguageProvider(LanguageCode.en_US, DatePickerShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Click DatePicker, and then we could select or input a date in panel.";
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
    public const string P2SecondaryPlaceholderTextEndDate = "End date";
    public const string P2PlaceholderTextSelectTime = "Select time";
    public const string P2PlaceholderTextStartDate = "Start date";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2TextExpandDirection = "Picker size:";
    public const string P2ContentLarge = "Large";
    public const string P2ContentDefault = "Default";
    public const string P2ContentSmall = "Small";
    public const string P2TextPlacement = "Placement:";
    public const string P2ContentTopleft = "TopLeft";
    public const string P2ContentTopright = "TopRight";
    public const string P2ContentBottomleft = "BottomLeft";
    public const string P2ContentBottomright = "BottomRight";

    protected override Type GetResourceKindType() => typeof(DatePickerShowCaseLangResourceKind);
}
