using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Descriptions;

[LanguageProvider(LanguageCode.zh_CN, DescriptionsShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string BorderTitle = "边框";
    public const string BorderDescription = "带边框和背景色的 Descriptions。";
    public const string CustomSizeTitle = "自定义尺寸";
    public const string CustomSizeDescription = "自定义尺寸以适配不同容器。";
    public const string ResponsiveTitle = "响应式";
    public const string ResponsiveDescription = "响应式配置可以在小屏设备上更好展示。";
    public const string VerticalTitle = "垂直布局";
    public const string VerticalDescription = "最简单的用法。";
    public const string VerticalBorderTitle = "垂直带边框";
    public const string VerticalBorderDescription = "带边框和背景色的 Descriptions。";
    public const string RowTitle = "整行展示";
    public const string RowDescription = "整行展示。";

    protected override Type GetResourceKindType() => typeof(DescriptionsShowCaseLangResourceKind);
}
