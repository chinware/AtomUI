using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Empty;

[LanguageProvider(LanguageCode.zh_CN, EmptyShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "AtomUI 支持小号、默认和大号三种尺寸。";
    public const string CustomizeTitle = "自定义";
    public const string CustomizeDescription = "自定义图片来源、图片尺寸、描述和额外内容。";
    public const string NoDescriptionTitle = "无描述";
    public const string NoDescriptionDescription = "不带描述的最简单用法。";

    protected override Type GetResourceKindType() => typeof(EmptyShowCaseLangResourceKind);
}
