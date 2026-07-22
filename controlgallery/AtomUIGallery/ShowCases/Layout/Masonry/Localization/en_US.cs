using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.en_US, MasonryShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string PageSubtitle = "Lay out children of unequal heights into balanced columns.";
    public const string PageDescription =
        "Masonry arranges cards, images, or arbitrary controls into a shortest-column waterfall layout, with fixed or adaptive column counts and row/column gaps.";
    public const string ComponentCategory = "Layout";
    public const string ComponentStatusStable = "Stable";
    public const string ApiEventLayoutChanged = "Raised when the effective column assignment of children changes, dispatched after the layout pass.";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage. Set the number of columns with ColumnCount and the spacing with ColumnGap and RowGap.";
    public const string ResponsiveTitle = "Responsive";
    public const string ResponsiveDescription = "Use responsive values to adapt to different screen widths. ColumnInfo controls the column count at each breakpoint, and Gutter controls the spacing.";
    public const string ImageTitle = "Image";
    public const string ImageDescription = "Positions are adjusted dynamically as images load.";
    public const string DynamicTitle = "Dynamic";
    public const string DynamicDescription = "Demonstrate how masonry layout updates dynamically. Use item.column to keep items in place.";
    public const string DynamicAddItemLabel = "Add Item";

}
