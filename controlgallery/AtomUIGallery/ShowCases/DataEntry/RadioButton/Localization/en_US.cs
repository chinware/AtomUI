using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.RadioButton;

[LanguageProvider(LanguageCode.en_US, RadioButtonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest use.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled state of RadioButton.";
    public const string RadioGroupTitle = "Radio Group";
    public const string RadioGroupDescription = "A group of radio components.";
    public const string VerticalRadioGroupTitle = "Vertical Radio Group";
    public const string VerticalRadioGroupDescription = "Vertical Radio Group.";
    public const string ItemsSourceRadioGroupTitle = "Radio Group by Items source";
    public const string ItemsSourceRadioGroupDescription = "Radio Group.";
    public const string OptionButtonTitle = "Option Button";
    public const string OptionButtonDescription = "OptionButton Group.";
    public const string OptionButtonWithIconTitle = "Option Button with icon";
    public const string OptionButtonWithIconDescription = "OptionButton Group with icon.";
    public const string OptionStyleTitle = "option style";
    public const string OptionStyleDescription = "The combination of option button style.";
    public const string SolidOptionButtonTitle = "Solid option button";
    public const string SolidOptionButtonDescription = "Solid option button style.";
    public const string SizeTypeTitle = "Size type";
    public const string SizeTypeDescription = "There are three sizes available: large, medium, and small. It can coordinate with input box.";

    protected override Type GetResourceKindType() => typeof(RadioButtonShowCaseLangResourceKind);
}
