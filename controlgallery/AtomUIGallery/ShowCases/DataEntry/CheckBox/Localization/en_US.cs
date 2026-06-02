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

    protected override Type GetResourceKindType() => typeof(CheckBoxShowCaseLangResourceKind);
}
