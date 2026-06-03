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
    public const string P2HeaderCut = "Cut";
    public const string P2HeaderCopy = "Copy";
    public const string P2HeaderDelete = "Delete";
    public const string P2ContentHoverMe = "Hover me";
    public const string P2ContentLarge = "Large";
    public const string P2ContentMiddle = "Middle";
    public const string P2ContentSmall = "Small";
    public const string P2ContentDefault = "Default";
    public const string P2ContentPrimary = "Primary";
    public const string P2ContentClickMe = "Click Me";

    protected override Type GetResourceKindType() => typeof(SplitButtonShowCaseLangResourceKind);
}
