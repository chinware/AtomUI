using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_CN, PaginationShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用页码、页大小、总数和快速跳转浏览长列表。";
    public const string PageDescription = "Pagination 将大型数据集拆分为可预测的页面，支持对齐方式、页大小选择、快速跳转、总数信息、迷你尺寸以及简洁只读或可编辑模式。";
    public const string BasicTitle = "基础分页";
    public const string BasicDescription = "基础分页。";
    public const string BindingTitle = "受控绑定";
    public const string BindingDescription = "CurrentPage 和 PageSize 不需要显式设置 Mode=TwoWay；点击页码和修改页大小都会写回 ViewModel。";
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
    public const string P2TextCurrentPage = "CurrentPage：";
    public const string P2TextPageSize = "PageSize：";
    public const string P2ContentSetPage = "设置第 5 页 / 20";
    public const string P2ContentReset = "重置";

}
