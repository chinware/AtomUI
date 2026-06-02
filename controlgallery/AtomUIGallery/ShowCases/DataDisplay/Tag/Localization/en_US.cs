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

    protected override Type GetResourceKindType() => typeof(TagShowCaseLangResourceKind);
}
