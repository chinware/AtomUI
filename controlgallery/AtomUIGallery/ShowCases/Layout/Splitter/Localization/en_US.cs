using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Splitter;

[LanguageProvider(LanguageCode.en_US, SplitterShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Initial sizes with min/max constraints.";
    public const string HorizontalTitle = "Horizontal";
    public const string HorizontalDescription = "Top-bottom splitter with collapsible icons.";
    public const string CompositeTitle = "Composite";
    public const string CompositeDescription = "Nested horizontal splitter inside a vertical layout.";
    public const string ResizableDisabledTitle = "Resizable Disabled";
    public const string ResizableDisabledDescription = "If either side disables resizable, dragging is blocked.";
    public const string ShowCollapsibleIconTitle = "ShowCollapsibleIcon";
    public const string ShowCollapsibleIconDescription = "Three panels with hover and always-visible collapsible icons.";
    public const string MultiPanelsTitle = "Multi Panels";
    public const string MultiPanelsDescription = "Multiple panels in vertical layout.";
    public const string LazyTitle = "Lazy";
    public const string LazyDescription = "Lazy rendering mode: sizes update on drag release.";

    protected override Type GetResourceKindType() => typeof(SplitterShowCaseLangResourceKind);
}
