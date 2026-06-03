using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TabStrip;

[LanguageProvider(LanguageCode.zh_CN, TabStripShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string TabStripBasicTitle = "基础用法";
    public const string TabStripBasicDescription = "默认激活第一个标签项。";
    public const string TabStripItemsSourceTitle = "通过 ItemSource 生成 TabStripItem";
    public const string TabStripItemsSourceDescription = "基于数据源和项目模板添加 TabStripItem。";
    public const string TabStripDisabledTitle = "禁用标签";
    public const string TabStripDisabledDescription = "禁用某个标签项。";
    public const string TabStripCenteredTitle = "居中显示";
    public const string TabStripCenteredDescription = "标签项居中显示。";
    public const string TabStripIconTitle = "带图标";
    public const string TabStripIconDescription = "带图标的标签项。";
    public const string TabStripSlideTitle = "滑动";
    public const string TabStripSlideDescription = "为了容纳更多标签，标签可以左右滑动（或上下滑动）。";
    public const string TabStripCardTypeTitle = "卡片式标签";
    public const string TabStripCardTypeDescription = "另一种标签类型，不支持垂直模式。";
    public const string TabStripClosableTitle = "可关闭标签";
    public const string TabStripClosableDescription = "支持可关闭的标签设置。";
    public const string TabStripPositionTitle = "位置";
    public const string TabStripPositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabStripCardShapePositionTitle = "卡片形态位置";
    public const string TabStripCardShapePositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabStripSizeTitle = "尺寸";
    public const string TabStripSizeDescription = "大尺寸标签通常用于页头，小尺寸可用于模态框。";
    public const string TabStripAddCloseTitle = "新增和关闭标签";
    public const string TabStripAddCloseDescription = "隐藏默认加号图标，并为自定义触发器绑定事件。";
    public const string P2TextTabPosition = "标签位置：";
    public const string P2ContentTop = "顶部";
    public const string P2ContentBottom = "底部";
    public const string P2ContentLeft = "左侧";
    public const string P2ContentRight = "右侧";
    public const string P2ContentSmall = "小号";
    public const string P2ContentMiddle = "中号";
    public const string P2ContentLarge = "大号";
    public const string P2ContentTabN1 = "标签页 1";
    public const string P2ContentTabN2 = "标签页 2";
    public const string P2ContentTabN3 = "标签页 3";
    public const string P2ContentTabN4 = "标签页 4";
    public const string P2ContentTabN5 = "标签页 5";
    public const string P2ContentTabN6 = "标签页 6";
    public const string P2ContentTabN7 = "标签页 7";
    public const string P2ContentTabN8 = "标签页 8";
    public const string P2ContentTabN9 = "标签页 9";
    public const string P2ContentTabN10 = "标签页 10";
    public const string P2ContentTabN11 = "标签页 11";
    public const string P2ContentTabN12 = "标签页 12";
    public const string P2ContentTabN13 = "标签页 13";
    public const string P2ContentTabN14 = "标签页 14";
    public const string P2ContentTabN15 = "标签页 15";
    public const string P2ContentTabN16 = "标签页 16";
    public const string P2ContentTabN17 = "标签页 17";
    public const string P2ContentTabN18 = "标签页 18";
    public const string P2ContentTabN19 = "标签页 19";
    public const string P2ContentTabN20 = "标签页 20";
    public const string P2ContentTabN21 = "标签页 21";
    public const string P2ContentTabN22 = "标签页 22";
    public const string P2ContentTabN23 = "标签页 23";
    public const string P2ContentNewTabFormat = "新增标签 {0}";
    public const string P2TextTabContent = "标签内容";

    protected override Type GetResourceKindType() => typeof(TabStripShowCaseLangResourceKind);
}
