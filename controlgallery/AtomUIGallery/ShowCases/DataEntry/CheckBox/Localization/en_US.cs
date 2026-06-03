using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CheckBox;

[LanguageProvider(LanguageCode.en_US, CheckBoxShowCase.LanguageId)]
internal class en_US : LanguageProvider
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
