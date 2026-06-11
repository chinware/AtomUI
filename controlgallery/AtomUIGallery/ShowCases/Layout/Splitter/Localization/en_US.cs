using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Splitter;

[LanguageProvider(LanguageCode.en_US, SplitterShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
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
    public const string P2TextFirst = "First";
    public const string P2TextSecond = "Second";
    public const string P2TextTop = "Top";
    public const string P2TextBottom = "Bottom";
    public const string P2TextLeft = "Left";
    public const string P2TextResizable = "Resizable";
    public const string P2TextNotResizable = "Not Resizable";
    public const string P2TextShowcollapsibleicon = "ShowCollapsibleIcon:";
    public const string P2ContentHover = "Hover";
    public const string P2ContentTrue = "True";
    public const string P2ContentFalse = "False";
    public const string P2TextThird = "Third";
    public const string P2TextA = "A";
    public const string P2TextB = "B";
    public const string P2TextC = "C";
    public const string P2TextD = "D";

    protected override Type GetResourceKindType() => typeof(SplitterShowCaseLangResourceKind);
}
