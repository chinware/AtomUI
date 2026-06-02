using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Drawer;

[LanguageProvider(LanguageCode.en_US, DrawerShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic drawer.";
    public const string MultiLevelTitle = "Multi-level drawer";
    public const string MultiLevelDescription = "Open a new drawer on top of an existing drawer to handle multi branch tasks.";
    public const string ExtraAndFooterTitle = "Extra and Footer";
    public const string ExtraAndFooterDescription = "Set the header extra area and footer area.";
    public const string NoMaskTitle = "No Mask";
    public const string NoMaskDescription = "Without a mask.";
    public const string CustomPlacementTitle = "Custom Placement";
    public const string CustomPlacementDescription = "The Drawer can appear from any edge of the screen.";
    public const string RenderInCurrentAreaTitle = "Render in current area";
    public const string RenderInCurrentAreaDescription = "Render in current area.";
    public const string PresetSizeTitle = "Preset size";
    public const string PresetSizeDescription = "The default width (or height) of Drawer is 378px, and there is a preset large size 736px.";

    protected override Type GetResourceKindType() => typeof(DrawerShowCaseLangResourceKind);
}
