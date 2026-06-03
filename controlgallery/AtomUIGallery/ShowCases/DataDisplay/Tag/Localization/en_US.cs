using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tag;

[LanguageProvider(LanguageCode.en_US, TagShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Usage of basic Tag, and it could be IsClosable and customize close button by set closeIcon property,will display default close button when closeIcon is setting to true. IsClosable Tag supports onClose events.";
    public const string ColorfulTagTitle = "Colorful Tag";
    public const string ColorfulTagDescription = "We preset a series of colorful tag styles for use in different situations. You can also set it to a hex color string for custom color.";
    public const string StatusTagTitle = "Status Tag";
    public const string StatusTagDescription = "We preset five different colors, you can set color property such as success,processing,error,default and warning to indicate specific status.";
    public const string IconTitle = "Icon";
    public const string IconDescription = "Tag components can contain an Icon. This is done by setting the icon property or placing an Icon component within the Tag. If you want specific control over the positioning and placement of the Icon, then that should be done by placing the Icon component within the Tag rather than using the icon property.";
    public const string BorderlessTitle = "borderless";
    public const string BorderlessDescription = "borderless";
    public const string P2ContentTagN1 = "Tag 1";
    public const string P2ContentLink = "Link";
    public const string P2ContentPreventDefault = "Prevent Default";
    public const string P2ContentTagN2 = "Tag 2";
    public const string P2TextPresets = "Presets";
    public const string P2ContentMagenta = "magenta";
    public const string P2ContentRed = "red";
    public const string P2ContentVolcano = "volcano";
    public const string P2ContentOrange = "orange";
    public const string P2ContentGold = "gold";
    public const string P2ContentLime = "lime";
    public const string P2ContentGreen = "green";
    public const string P2ContentCyan = "cyan";
    public const string P2ContentBlue = "blue";
    public const string P2ContentGeekblue = "geekblue";
    public const string P2ContentPurple = "purple";
    public const string P2TextCustom = "Custom";
    public const string P2TextWithoutIcon = "Without icon";
    public const string P2ContentSuccess = "success";
    public const string P2ContentProcessing = "processing";
    public const string P2ContentError = "error";
    public const string P2ContentWarning = "warning";
    public const string P2ContentDefault = "default";
    public const string P2ContentTwitter = "Twitter";
    public const string P2ContentYoutube = "Youtube";
    public const string P2ContentFacebook = "Facebook";
    public const string P2ContentLinkedin = "Linkedin";
    public const string P2ContentTag1 = "Tag1";
    public const string P2ContentTag2 = "Tag2";
    public const string P2ContentTag3 = "Tag3";
    public const string P2ContentTag4 = "Tag4";

    public const string P2TextMaterialIcon = "Material icon";

    protected override Type GetResourceKindType() => typeof(TagShowCaseLangResourceKind);
}
