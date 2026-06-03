using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Avatar;

[LanguageProvider(LanguageCode.en_US, AvatarShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Three sizes and two shapes are available.";
    public const string TypeTitle = "Type";
    public const string TypeDescription = "Image, Icon and letter are supported, and the latter two kinds of avatar can have custom colors and background colors.";
    public const string AutoSetFontSizeTitle = "Autoset Font Size";
    public const string AutoSetFontSizeDescription = "For letter type Avatar, when the letters are too long to display, the font size can be automatically adjusted according to the width of the Avatar. You can also use gap to set the unit distance between left and right sides.";
    public const string AvatarGroupTitle = "Avatar.Group";
    public const string AvatarGroupDescription = "Avatar group display.";
    public const string P2ContentU = "U";
    public const string P2ContentUser = "USER";
    public const string P2ContentChangeuser = "Change user";
    public const string P2ContentChangegap = "Change gap";
    public const string P2ContentK = "K";
    public const string P2ContentA = "A";

    protected override Type GetResourceKindType() => typeof(AvatarShowCaseLangResourceKind);
}
