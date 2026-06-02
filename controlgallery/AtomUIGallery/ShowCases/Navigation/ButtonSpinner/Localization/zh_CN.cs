using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

[LanguageProvider(LanguageCode.zh_CN, ButtonSpinnerShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础的按钮微调器。";
    public const string ThreeSizesTitle = "三种尺寸";
    public const string ThreeSizesDescription = "按钮微调器提供三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
    public const string VariantsTitle = "不同形态";
    public const string VariantsDescription = "输入框的不同形态。";
    public const string DisabledTitle = "禁用状态";
    public const string DisabledDescription = "禁用状态下的输入框形态。";
    public const string PrePostTabTitle = "前置/后置标签";
    public const string PrePostTabDescription = "前置和后置标签的使用示例。";
    public const string PrefixSuffixTitle = "前缀和后缀";
    public const string PrefixSuffixDescription = "在输入框内部添加前缀或后缀图标。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为输入框添加错误或警告状态。";

    protected override Type GetResourceKindType() => typeof(ButtonSpinnerShowCaseLangResourceKind);
}
