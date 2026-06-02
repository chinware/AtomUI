using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_CN, PaginationShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础分页";
    public const string BasicDescription = "基础分页。";
    public const string AlignTitle = "对齐方式";
    public const string AlignDescription = "支持左对齐、居中对齐和右对齐三种对齐方式。";
    public const string MoreTitle = "更多页码";
    public const string MoreDescription = "更多页码。";
    public const string MiniSizeTitle = "迷你尺寸";
    public const string MiniSizeDescription = "迷你尺寸分页。";
    public const string TotalNumberTitle = "总数";
    public const string TotalNumberDescription = "可以通过设置 showTotal 展示数据总量。";
    public const string SimpleModeTitle = "简洁模式";
    public const string SimpleModeDescription = "简洁模式。";

    protected override Type GetResourceKindType() => typeof(PaginationShowCaseLangResourceKind);
}
