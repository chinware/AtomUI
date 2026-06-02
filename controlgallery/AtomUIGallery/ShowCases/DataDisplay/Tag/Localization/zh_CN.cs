using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tag;

[LanguageProvider(LanguageCode.zh_CN, TagShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础 Tag 用法。可通过 IsClosable 设置为可关闭，并通过 closeIcon 属性自定义关闭按钮；closeIcon 设置为 true 时显示默认关闭按钮。IsClosable Tag 支持 onClose 事件。";
    public const string ColorfulTagTitle = "多彩标签";
    public const string ColorfulTagDescription = "预设了一系列多彩标签样式，可用于不同场景。也可以设置十六进制颜色字符串来自定义颜色。";
    public const string StatusTagTitle = "状态标签";
    public const string StatusTagDescription = "预设了五种不同颜色，可以设置 success、processing、error、default 和 warning 等 color 属性表示具体状态。";
    public const string IconTitle = "图标";
    public const string IconDescription = "Tag 组件可以包含图标。可以通过设置 icon 属性，或在 Tag 内放置 Icon 组件实现。如果需要精确控制图标的位置和布局，应在 Tag 内放置 Icon 组件，而不是使用 icon 属性。";
    public const string BorderlessTitle = "无边框";
    public const string BorderlessDescription = "无边框。";

    protected override Type GetResourceKindType() => typeof(TagShowCaseLangResourceKind);
}
