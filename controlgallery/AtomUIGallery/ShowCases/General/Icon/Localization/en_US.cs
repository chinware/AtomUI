using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.en_US, IconShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ComponentCategory = "General";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Browse AtomUI Ant Design icons by theme style.";
    public const string PageDescription = "Icon provides the Ant Design icon set for AtomUI controls and applications. Use the tabs to switch between outlined, filled, and two-tone icon themes, then search within the selected gallery.";
    public const string InfoNamespaceLabel = "Namespace:";
    public const string InfoPackageLabel = "Package:";
    public const string InfoBaseClassLabel = "Base class:";
    public const string P2HeaderOutlined = "Outlined";
    public const string P2HeaderFilled = "Filled";
    public const string P2HeaderTwoTone = "Two Tone";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}
