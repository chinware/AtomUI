using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.en_US, MasonryShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Lay out children of unequal heights into balanced columns.";
    public const string PageDescription =
        "Masonry arranges cards, images, or arbitrary controls into a shortest-column waterfall layout, with fixed or adaptive column counts and row/column gaps.";
    public const string ComponentCategory = "Layout";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyColumnCount = "Fixed number of columns. When greater than 0 it overrides adaptive calculation.";
    public const string ApiPropertyColumnInfo = "Responsive column count. When the current breakpoint is configured, it takes precedence over ColumnCount.";
    public const string ApiPropertyMinColumnWidth = "Minimum target column width used to compute the adaptive column count.";
    public const string ApiPropertyMaxColumnCount = "Upper bound for the adaptive column count to avoid too many columns on wide screens.";
    public const string ApiPropertyColumnGap = "Horizontal spacing between columns.";
    public const string ApiPropertyRowGap = "Vertical spacing between rows.";
    public const string ApiPropertyGutter = "Responsive horizontal and vertical spacing. When the current breakpoint is configured, it takes precedence over ColumnGap and RowGap.";
    public const string ApiPropertyItemsSource = "Binds a data collection; the ItemsControl base generates a container per item.";
    public const string ApiPropertyItemTemplate = "Data template used to render each bound data item.";
    public const string ApiPropertyMasonryColumn = "Optional item-container attached property that pins a child to a specific column; falls back to the shortest column when null.";
    public const string ApiPropertyMasonrySpan = "Item-container attached property controlling how a child spans columns. Auto places one column, Full spans the entire width.";
    public const string ApiEventLayoutChanged = "Raised when the effective column assignment of children changes, dispatched after the layout pass.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusNotApplicable = "Not applicable";
    public const string TokenNameNoComponentToken = "Masonry has no component-specific design tokens; it relies on shared layout and spacing tokens.";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage. Set the number of columns with ColumnCount and the spacing with ColumnGap and RowGap.";
    public const string ResponsiveTitle = "Responsive";
    public const string ResponsiveDescription = "Use responsive values to adapt to different screen widths. ColumnInfo controls the column count at each breakpoint, and Gutter controls the spacing.";
    public const string ImageTitle = "Image";
    public const string ImageDescription = "Positions are adjusted dynamically as images load.";
    public const string DynamicTitle = "Dynamic";
    public const string DynamicDescription = "Demonstrate how masonry layout updates dynamically. Use item.column to keep items in place.";
    public const string DynamicAddItemLabel = "Add Item";

    protected override Type GetResourceKindType() => typeof(MasonryShowCaseLangResourceKind);
}
