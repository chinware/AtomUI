using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Drawer;

[LanguageProvider(LanguageCode.zh_TW, DrawerShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎抽屜。";
    public const string MultiLevelTitle = "多層抽屜";
    public const string MultiLevelDescription = "在已有抽屜上打開新的抽屜，用於處理多分支任務。";
    public const string ExtraAndFooterTitle = "額外區域和頁腳";
    public const string ExtraAndFooterDescription = "設置頭部額外區域和頁腳區域。";
    public const string NoMaskTitle = "無遮罩";
    public const string NoMaskDescription = "不顯示遮罩。";
    public const string CustomPlacementTitle = "自定義位置";
    public const string CustomPlacementDescription = "抽屜可以從屏幕任意邊緣彈出。";
    public const string RenderInCurrentAreaTitle = "在當前區域渲染";
    public const string RenderInCurrentAreaDescription = "在當前區域中渲染。";
    public const string PresetSizeTitle = "預設尺寸";
    public const string PresetSizeDescription = "抽屜默認寬度（或高度）為 378px，並提供 736px 的大尺寸預設。";
    public const string P2TitleBasicDrawer = "基礎抽屜";
    public const string P2TextSomeContents = "一些內容...";
    public const string P2TitleFirstLevelDrawer = "一級抽屜";
    public const string P2TitleTwoLevelDrawer = "二級抽屜";
    public const string P2TextPlacement = "彈出位置：";
    public const string P2ContentLeft = "左側";
    public const string P2ContentTop = "頂部";
    public const string P2ContentRight = "右側";
    public const string P2ContentBottom = "底部";
    public const string P2ContentTwoLevelDrawer = "二級抽屜";
    public const string P2ContentCancel = "取消";
    public const string P2ContentOk = "確定";
    public const string P2ContentEdit = "編輯";
    public const string P2ContentUpload = "上傳";
    public const string P2ContentDelete = "刪除";
    public const string P2TextRenderInThis = "在此區域渲染";
    public const string P2ContentOpenDefaultSizeN378px = "打開默認尺寸 (378px)";
    public const string P2ContentOpenLargeSizeN736px = "打開大尺寸 (736px)";
    public const string P2ContentOpenCustomSizeN400px = "打開自定義尺寸 (400px)";
    public const string P2ContentOpenCustomSizeN50 = "打開自定義尺寸 (50%)";

    public const string P2ContentOpen = "打開";

    protected override Type GetResourceKindType() => typeof(DrawerShowCaseLangResourceKind);
}

