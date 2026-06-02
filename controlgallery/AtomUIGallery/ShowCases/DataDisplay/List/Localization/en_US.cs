using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.List;

[LanguageProvider(LanguageCode.en_US, ListShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Basic usage example.";
    public const string SelectionTitle = "Selection";
    public const string SelectionDescription = "You can set single selection, multiple selection or no selection.";
    public const string GroupTitle = "Group";
    public const string GroupDescription = "You can group the data based on conditions.";
    public const string ItemDisabledTitle = "Item disabled";
    public const string ItemDisabledDescription = "Disabled item.";
    public const string EmptyTitle = "Empty";
    public const string EmptyDescription = "show empty indicator when no data.";
    public const string FilterTitle = "Filter";
    public const string FilterDescription = "You can filter the data based on criteria; here we've filtered for items containing 'a'.";
    public const string OrderedTitle = "Ordered";
    public const string OrderedDescription = "We can sort according to specified attributes.";
    public const string SimpleListBoxTitle = "Simple listbox control";
    public const string SimpleListBoxDescription = "Basic usage example.";
    public const string SimpleListBoxItemsSourceTitle = "Simple listbox control";
    public const string SimpleListBoxItemsSourceDescription = "Basic ItemsSource and template usage example.";
    public const string SearchableTitle = "Searchable";
    public const string SearchableDescription = "Searchable List.";
    public const string PaginationListTitle = "Pagination list";
    public const string PaginationListDescription = "Pagination list.";

    protected override Type GetResourceKindType() => typeof(ListShowCaseLangResourceKind);
}
