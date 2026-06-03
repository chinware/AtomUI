using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Button;

[LanguageProvider(LanguageCode.en_US, ButtonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string TypeTitle = "Type";
    public const string TypeDescription = "There are primary button, default button, dashed button, text button and link button in antd.";
    public const string ButtonShapeTitle = "Button Shape";
    public const string ButtonShapeDescription = "Supported button shape display, such as primary, default, dashed and text, etc.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "AtomUI supports three sizes of buttons: small, default and large. If a large or small button is desired, set the size property to either large or small respectively. Omit the size property for a button with the default size.";
    public const string IconTitle = "Icon";
    public const string IconDescription = "You can add an icon through the icon property and adjust the position of the icon using iconPosition.";
    public const string LoadingTitle = "Loading";
    public const string LoadingDescription = "A loading indicator can be added to a button by setting the loading property on the Button.";
    public const string BlockButtonTitle = "Block Button";
    public const string BlockButtonDescription = "block property will make the button fit to its parent width.";
    public const string DangerButtonsTitle = "Danger Buttons";
    public const string DangerButtonsDescription = "danger is a property of button after antd 4.0.";
    public const string GhostButtonTitle = "Ghost Button";
    public const string GhostButtonDescription = "ghost property will make button's background transparent, it is commonly used in colored background.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "To mark a button as disabled, add the disabled property to the Button.";
    public const string P2ContentPrimaryButton = "Primary Button";
    public const string P2ContentDefaultButton = "Default Button";
    public const string P2ContentDashed = "Dashed";
    public const string P2ContentTextButton = "Text Button";
    public const string P2ContentLinkButton = "Link Button";
    public const string P2ContentPrimary = "Primary";
    public const string P2ContentDefault = "Default";
    public const string P2ContentText = "Text";
    public const string P2ContentLink = "Link";
    public const string P2ContentAa = "AA";
    public const string P2TextExpandDirection = "Button size:";
    public const string P2ContentLarge = "Large";
    public const string P2ContentSmall = "Small";
    public const string P2ContentDownload = "Download";
    public const string P2ContentSearch = "Search";
    public const string P2ContentLoading = "Loading";
    public const string P2ContentClickMe = "Click me!";
    public const string P2ContentDanger = "Danger";
    public const string P2ContentPrimaryDisabled = "Primary(disabled)";
    public const string P2ContentDefaultDisabled = "Default(disabled)";
    public const string P2ContentDashedDisabled = "Dashed(disabled)";
    public const string P2ContentTextDisabled = "Text(disabled)";
    public const string P2ContentLinkDisabled = "Link(disabled)";
    public const string P2ContentDangerPrimary = "Danger Primary";
    public const string P2ContentDangerPrimaryDisabled = "Danger Primary(disabled)";
    public const string P2ContentDangerDefault = "Danger Default";
    public const string P2ContentDangerDefaultDisabled = "Danger Default(disabled)";
    public const string P2ContentDangerText = "Danger Text";
    public const string P2ContentDangerTextDisabled = "Danger Text(disabled)";
    public const string P2ContentDangerLink = "Danger Link";
    public const string P2ContentDangerLinkDisabled = "Danger Link(disabled)";
    public const string P2ContentGhost = "Ghost";
    public const string P2ContentGhostDisabled = "Ghost(disabled)";

    protected override Type GetResourceKindType() => typeof(ButtonShowCaseLangResourceKind);
}
