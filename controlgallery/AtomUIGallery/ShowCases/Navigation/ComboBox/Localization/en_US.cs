using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ComboBox;

[LanguageProvider(LanguageCode.en_US, ComboBoxShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "A selection input for choosing from a compact popup list.";
    public const string PageDescription = "ComboBox combines input-style layout with dropdown selection, supporting item templates, add-ons, prefixes, suffixes, validation status, and size variants.";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic button spinner.";
    public const string ItemsSourceTitle = "Generate ComboBoxItem by ItemsSource";
    public const string ItemsSourceDescription = "Generate structure based on ItemsSource and template.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled button spinner.";
    public const string ThreeSizesTitle = "Three sizes of Input";
    public const string ThreeSizesDescription = "There are three sizes of an ComboBox: large (40px), default (32px) and small (24px).";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Input.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string PrefixSuffixTitle = "prefix and suffix";
    public const string PrefixSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Input with status, which could be error or warning.";
    public const string P2PlaceholderTextPleaseSelect = "Please select";
    public const string P2ContentPoemLine1 = "床前明月光";
    public const string P2ContentPoemLine2 = "疑是地上霜";
    public const string P2ContentPoemLine3 = "举头望明月";
    public const string P2ContentPoemLine4 = "低头思故乡";

    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyItemsSource = "Collection used to generate popup items.";
    public const string ApiPropertySelectedItem = "Currently selected item.";
    public const string ApiPropertySelectedIndex = "Index of the selected item.";
    public const string ApiPropertyPlaceholderText = "Placeholder shown when no item is selected.";
    public const string ApiPropertyLeftAddOn = "Content displayed outside the input area on the left.";
    public const string ApiPropertyRightAddOn = "Content displayed outside the input area on the right.";
    public const string ApiPropertyContentLeftAddOn = "Content displayed inside the input area before the selection.";
    public const string ApiPropertyContentRightAddOn = "Content displayed inside the input area after the selection.";
    public const string ApiPropertySizeType = "Controls the ComboBox size.";
    public const string ApiPropertyStyleVariant = "Controls outlined, filled, and borderless visual variants.";
    public const string ApiPropertyStatus = "Applies validation status styling.";
    public const string ApiPropertyIsAllowClear = "Allows the current selection to be cleared.";
    public const string ApiPropertyOptionFontSize = "Overrides popup option font size.";
    public const string ApiPropertyDropDownDisplayPageSize = "Controls how many options are shown before scrolling.";
    public const string ApiPropertyShouldUseOverlayPopup = "Controls whether the dropdown uses the overlay popup host.";
    public const string ApiPropertyIsMotionEnabled = "Enables control motion when the theme allows it.";

    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameControlWidth = "Default control width inherited from ButtonSpinner.";
    public const string TokenNameHandleWidth = "Dropdown handle width.";
    public const string TokenNameHandleIconSize = "Dropdown handle icon size.";
    public const string TokenNameHandleBg = "Dropdown handle background color.";
    public const string TokenNameHandleActiveBg = "Dropdown handle active background color.";
    public const string TokenNameHandleHoverColor = "Dropdown handle hover foreground color.";
    public const string TokenNameHandleBorderColor = "Dropdown handle border color.";
    public const string TokenNameFilledHandleBg = "Dropdown handle background for filled variant.";
    public const string TokenNameInputFontSize = "Default input font size inherited from LineEdit.";
    public const string TokenNameInputFontSizeLG = "Large input font size inherited from LineEdit.";
    public const string TokenNameInputFontSizeSM = "Small input font size inherited from LineEdit.";
    public const string TokenNamePopupContentPadding = "Dropdown popup content padding.";
    public const string TokenNameItemColor = "Option text color.";
    public const string TokenNameItemHoverColor = "Option hover text color.";
    public const string TokenNameItemSelectedColor = "Selected option text color.";
    public const string TokenNameItemDisabledColor = "Disabled option text color.";
    public const string TokenNameItemBgColor = "Option background color.";
    public const string TokenNameItemHoverBgColor = "Option hover background color.";
    public const string TokenNameItemSelectedBgColor = "Selected option background color.";
    public const string TokenNameItemPadding = "Option content padding.";
    public const string TokenNameItemMargin = "Option margin.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(ComboBoxShowCaseLangResourceKind);
}
