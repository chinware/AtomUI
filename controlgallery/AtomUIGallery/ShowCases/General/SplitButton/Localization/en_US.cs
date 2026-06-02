using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.SplitButton;

[LanguageProvider(LanguageCode.en_US, SplitButtonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic SplitButton.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "AtomUI supports three sizes of buttons: small, default, and large. If a large or small button is desired, set the size property to large or small respectively. Omit the size property for a button with the default size.";
    public const string DangerButtonsTitle = "Danger Buttons";
    public const string DangerButtonsDescription = "danger is a button property after antd 4.0.";
    public const string CustomIconTitle = "Custom Icon";
    public const string CustomIconDescription = "Custom flyout button icon.";
    public const string FlyoutTriggerTypeTitle = "Flyout trigger type";
    public const string FlyoutTriggerTypeDescription = "Support two trigger types.";

    protected override Type GetResourceKindType() => typeof(SplitButtonShowCaseLangResourceKind);
}
