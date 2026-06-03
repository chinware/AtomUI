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
    public const string P2PlaceholderTextSearch = "Search";
    public const string P2FilterValue = "a";
    public const string P2TextSelectionMode = "Selection Mode:";
    public const string P2ContentSingle = "Single";
    public const string P2ContentMultiple = "Multiple";
    public const string P2ContentToggle = "Toggle";
    public const string P2ColorBlue = "Blue";
    public const string P2ColorGreen = "Green";
    public const string P2ColorRed = "Red";
    public const string P2ColorYellow = "Yellow";
    public const string P2ColorOrange = "Orange";
    public const string P2ColorPurple = "Purple";
    public const string P2ColorPink = "Pink";
    public const string P2ColorBrown = "Brown";
    public const string P2ColorWhite = "White";
    public const string P2ColorBlack = "Black";
    public const string P2ColorGray = "Gray";
    public const string P2ColorTurquoise = "Turquoise";
    public const string P2ColorViolet = "Violet";
    public const string P2ColorMagenta = "Magenta";
    public const string P2ColorMaroon = "Maroon";
    public const string P2ColorNavy = "Navy";
    public const string P2ColorBeige = "Beige";
    public const string P2ColorCyan = "Cyan";
    public const string P2ColorLavender = "Lavender";
    public const string P2ColorOlive = "Olive";
    public const string P2GroupBasicColors = "Basic Colors";
    public const string P2GroupNeutralColors = "Neutral Colors";
    public const string P2GroupSpecificShades = "Specific Shades";
    public const string P2ContentRacingCarSpraysBurningFuelIntoCrowd = "Racing car sprays burning fuel into crowd.";
    public const string P2ContentJapanesePrincessToWedCommoner = "Japanese princess to wed commoner.";
    public const string P2ContentAustralianWalksN100kmAfterOutbackCrash = "Australian walks 100km after outback crash.";
    public const string P2ContentManChargedOverMissingWeddingGirl = "Man charged over missing wedding girl.";
    public const string P2ContentLosAngelesBattlesHugeWildfires = "Los Angeles battles huge wildfires.";
    public const string P2ContentDynamicItem = "Dynamic item";
    public const string P2ContentPaginationItemFormat = "Content {0}";

    public const string P2ContentAddItem = "Add Item";

    public const string P2ContentRemoveItem = "Remove Item";

    protected override Type GetResourceKindType() => typeof(ListShowCaseLangResourceKind);
}
