using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Segmented;

[LanguageProvider(LanguageCode.zh_CN, SegmentedShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string BlockSegmentedTitle = "块级分段控制器";
    public const string BlockSegmentedDescription = "block 属性会让 Segmented 适配父容器宽度。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "禁用状态的 Segmented。";
    public const string ThreeSizesTitle = "Segmented 三种尺寸";
    public const string ThreeSizesDescription = "Segmented 有三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
    public const string IconOnlyTitle = "仅图标";
    public const string IconOnlyDescription = "为 Segmented 项设置图标但不设置标签。";
    public const string WithIconTitle = "带图标";
    public const string WithIconDescription = "为 Segmented 项设置图标。";

    protected override Type GetResourceKindType() => typeof(SegmentedShowCaseLangResourceKind);
}
