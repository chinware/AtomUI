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
    public const string P2TitleBasicDrawer = "Basic Drawer";
    public const string P2TextSomeContents = "Some contents...";
    public const string P2TitleFirstLevelDrawer = "First-level Drawer";
    public const string P2TitleTwoLevelDrawer = "Two-level Drawer";
    public const string P2TextPlacement = "Placement:";
    public const string P2ContentLeft = "Left";
    public const string P2ContentTop = "Top";
    public const string P2ContentRight = "Right";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentTwoLevelDrawer = "Two-level drawer";
    public const string P2ContentCancel = "Cancel";
    public const string P2ContentOk = "Ok";
    public const string P2ContentEdit = "Edit";
    public const string P2ContentUpload = "Upload";
    public const string P2ContentDelete = "Delete";
    public const string P2TextRenderInThis = "Render in this";
    public const string P2ContentOpenDefaultSizeN378px = "Open Default Size (378px)";
    public const string P2ContentOpenLargeSizeN736px = "Open Large Size (736px)";
    public const string P2ContentOpenCustomSizeN400px = "Open Custom Size (400px)";
    public const string P2ContentOpenCustomSizeN50 = "Open Custom Size (50%)";

    public const string P2ContentOpen = "Open";

    protected override Type GetResourceKindType() => typeof(DrawerShowCaseLangResourceKind);
}
