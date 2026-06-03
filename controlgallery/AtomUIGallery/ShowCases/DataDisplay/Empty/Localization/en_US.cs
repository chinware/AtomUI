using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Empty;

[LanguageProvider(LanguageCode.en_US, EmptyShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "AtomUI supports three sizes of buttons: small, default and large.";
    public const string CustomizeTitle = "Customize";
    public const string CustomizeDescription = "Customize image source, image size, description and extra content.";
    public const string NoDescriptionTitle = "No description";
    public const string NoDescriptionDescription = "Simplest Usage with no description.";
    public const string P2DescriptionCustomizeDescription = "Customize Description";
    public const string P2ContentCreateNow = "Create Now";

    protected override Type GetResourceKindType() => typeof(EmptyShowCaseLangResourceKind);
}
