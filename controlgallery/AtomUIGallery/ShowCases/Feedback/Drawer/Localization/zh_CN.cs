using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Drawer;

[LanguageProvider(LanguageCode.zh_CN, DrawerShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
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
    public const string PresetSizeDescription = "Drawer 默认宽度（或高度）为 378px，并提供 736px 的大尺寸预设。";

    protected override Type GetResourceKindType() => typeof(DrawerShowCaseLangResourceKind);
}
