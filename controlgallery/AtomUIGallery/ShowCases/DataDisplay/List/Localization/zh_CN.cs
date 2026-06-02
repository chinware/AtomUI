using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.List;

[LanguageProvider(LanguageCode.zh_CN, ListShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法示例。";
    public const string SelectionTitle = "选择";
    public const string SelectionDescription = "可以设置单选、多选或不可选择。";
    public const string GroupTitle = "分组";
    public const string GroupDescription = "可以根据条件对数据分组。";
    public const string ItemDisabledTitle = "禁用项";
    public const string ItemDisabledDescription = "禁用列表项。";
    public const string EmptyTitle = "空状态";
    public const string EmptyDescription = "无数据时显示空状态指示。";
    public const string FilterTitle = "筛选";
    public const string FilterDescription = "可以按条件筛选数据；这里筛选包含 'a' 的项。";
    public const string OrderedTitle = "排序";
    public const string OrderedDescription = "可以根据指定属性排序。";
    public const string SimpleListBoxTitle = "简单列表框控件";
    public const string SimpleListBoxDescription = "基础用法示例。";
    public const string SimpleListBoxItemsSourceTitle = "简单列表框控件";
    public const string SimpleListBoxItemsSourceDescription = "基础 ItemsSource 和模板用法示例。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的列表。";
    public const string PaginationListTitle = "分页列表";
    public const string PaginationListDescription = "分页列表。";

    protected override Type GetResourceKindType() => typeof(ListShowCaseLangResourceKind);
}
