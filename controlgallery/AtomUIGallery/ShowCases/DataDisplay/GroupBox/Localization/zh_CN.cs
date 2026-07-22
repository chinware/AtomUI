using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.zh_CN, GroupBoxShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "GroupBox 控件的基础用法。";
    public const string AutoHeightTitle = "自动高度";
    public const string AutoHeightDescription = "GroupBox 未设置高度时会根据 Header、Padding 和内容期望尺寸自动撑开。";
    public const string HeaderPositionTitle = "标题位置";
    public const string HeaderPositionDescription = "GroupBox 标题支持左、中、右三种位置。";
    public const string HeaderStyleTitle = "标题样式";
    public const string HeaderStyleDescription = "GroupBox 标题支持自定义颜色和字体等属性。";
    public const string HeaderIconTitle = "标题图标";
    public const string HeaderIconDescription = "GroupBox 标题支持指定图标。";
    public const string P2HeaderTitleTitleInfo = "标题信息";
    public const string P2TextContentOfGroupBox = "分组框内容";
    public const string AutoHeightContentOverview = "下面的 GroupBox 没有设置 Height，内容区域会随着文本行数自动增长。";
    public const string AutoHeightContentDetail = "当内容来自 StackPanel、Grid 或显式尺寸控件时，GroupBox 会使用内容的 DesiredSize 计算整体高度。";
    public const string AutoHeightContentFooter = "如果父容器设置了固定高度或 MaxHeight，则仍然会按 Avalonia 布局约束进行裁剪或滚动。";
    public const string ScenarioExamples = "示例";
    public const string PageSubtitle = "用带标题的边框容器组织相关内容。";
    public const string PageDescription =
        "GroupBox 通过带边框的容器包裹相关内容，并支持配置标题、位置、图标和字体样式。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";

}
