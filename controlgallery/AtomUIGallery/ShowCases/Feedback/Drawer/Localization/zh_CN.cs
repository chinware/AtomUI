using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Drawer;

[LanguageProvider(LanguageCode.zh_CN, DrawerShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础抽屉。";
    public const string MultiLevelTitle = "多层抽屉";
    public const string MultiLevelDescription = "在已有抽屉上打开新的抽屉，用于处理多分支任务。";
    public const string ExtraAndFooterTitle = "额外区域和页脚";
    public const string ExtraAndFooterDescription = "设置头部额外区域和页脚区域。";
    public const string NoMaskTitle = "无遮罩";
    public const string NoMaskDescription = "不显示遮罩。";
    public const string CustomPlacementTitle = "自定义位置";
    public const string CustomPlacementDescription = "抽屉可以从屏幕任意边缘弹出。";
    public const string RenderInCurrentAreaTitle = "在当前区域渲染";
    public const string RenderInCurrentAreaDescription = "在当前区域中渲染。";
    public const string PresetSizeTitle = "预设尺寸";
    public const string PresetSizeDescription = "抽屉默认宽度（或高度）为 378px，并提供 736px 的大尺寸预设。";
    public const string P2TitleBasicDrawer = "基础抽屉";
    public const string P2TextSomeContents = "一些内容...";
    public const string P2TitleFirstLevelDrawer = "一级抽屉";
    public const string P2TitleTwoLevelDrawer = "二级抽屉";
    public const string P2TextPlacement = "弹出位置：";
    public const string P2ContentLeft = "左侧";
    public const string P2ContentTop = "顶部";
    public const string P2ContentRight = "右侧";
    public const string P2ContentBottom = "底部";
    public const string P2ContentTwoLevelDrawer = "二级抽屉";
    public const string P2ContentCancel = "取消";
    public const string P2ContentOk = "确定";
    public const string P2ContentEdit = "编辑";
    public const string P2ContentUpload = "上传";
    public const string P2ContentDelete = "删除";
    public const string P2TextRenderInThis = "在此区域渲染";
    public const string P2ContentOpenDefaultSizeN378px = "打开默认尺寸 (378px)";
    public const string P2ContentOpenLargeSizeN736px = "打开大尺寸 (736px)";
    public const string P2ContentOpenCustomSizeN400px = "打开自定义尺寸 (400px)";
    public const string P2ContentOpenCustomSizeN50 = "打开自定义尺寸 (50%)";

    public const string P2ContentOpen = "打开";

    protected override Type GetResourceKindType() => typeof(DrawerShowCaseLangResourceKind);
}
