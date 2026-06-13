using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CheckBox;

[LanguageProvider(LanguageCode.en_US, CheckBoxShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest use.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled checkbox.";
    public const string ControlledCheckboxTitle = "Controlled Checkbox";
    public const string ControlledCheckboxDescription = "Communicated with other components.";
    public const string CheckboxGroupTitle = "Checkbox Group";
    public const string CheckboxGroupDescription = "Generate a group of checkboxes from an array.";
    public const string CheckAllTitle = "Check all";
    public const string CheckAllDescription = "The indeterminate property can help you to achieve a 'check all' effect.";
    public const string UseWithGridTitle = "Use with Grid";
    public const string UseWithGridDescription = "We can use Checkbox and Grid in Checkbox.Group, to implement complex layout.";
    public const string PageSubtitle = "Collect binary or multiple choices from users.";
    public const string PageDescription = "CheckBox supports checked, unchecked, indeterminate, disabled, controlled, grouped, and check-all states for forms and option lists.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyIsChecked = "Gets or sets whether the checkbox is checked, unchecked, or indeterminate.";
    public const string ApiPropertyIsThreeState = "Allows the checkbox to use an indeterminate state when enabled.";
    public const string ApiPropertyContent = "Content displayed next to the checkbox indicator.";
    public const string ApiPropertyCommand = "Command invoked when the checkbox is clicked.";
    public const string ApiPropertyIsMotionEnabled = "Enables motion for checkbox state transitions.";
    public const string ApiPropertyIsWaveSpiritEnabled = "Enables the click wave feedback effect.";
    public const string ApiPropertyItemsSource = "Data source used by CheckBoxGroup to generate checkbox items.";
    public const string ApiPropertyCheckedItems = "Selected item collection maintained by CheckBoxGroup.";
    public const string ApiPropertyItemSpacing = "Horizontal spacing between CheckBoxGroup items.";
    public const string ApiPropertyLineSpacing = "Vertical spacing between wrapped CheckBoxGroup lines.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string TokenNameCheckIndicatorSize = "Size of the checkbox indicator.";
    public const string TokenNameCheckedMarkSize = "Size of the checked mark inside the indicator.";
    public const string TokenNameIndicatorTristateMarkSize = "Size of the indeterminate mark.";
    public const string TokenNameTextMargin = "Margin between the indicator and text content.";
    public const string P2ContentCheckbox = "Checkbox";
    public const string P2ContentUnchecked = "UnChecked";
    public const string P2ContentIndeterminate = "Indeterminate";
    public const string P2ContentChecked = "Checked";
    public const string P2ContentCheck = "Check";
    public const string P2ContentUncheck = "UnCheck";
    public const string P2ContentEnable = "Enable";
    public const string P2ContentDisable = "Disable";
    public const string P2ContentEnabled = "Enabled";
    public const string P2ContentDisabled = "Disabled";
    public const string P2ControlledStatusFormat = "{0}-{1}";
    public const string P2ContentApple = "Apple";
    public const string P2ContentPear = "Pear";
    public const string P2ContentOrange = "Orange";
    public const string P2ContentCheckAll = "Check all";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";

    protected override Type GetResourceKindType() => typeof(CheckBoxShowCaseLangResourceKind);
}
