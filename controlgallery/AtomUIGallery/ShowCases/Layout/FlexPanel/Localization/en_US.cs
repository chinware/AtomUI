using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.FlexPanel;

[LanguageProvider(LanguageCode.en_US, FlexPanelShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicLayoutTitle = "Basic Layout";
    public const string BasicLayoutDescription = "The basic usage.";
    public const string AlignmentTitle = "Alignment";
    public const string AlignmentDescription = "Set justify and align.";
    public const string GapTitle = "Gap";
    public const string GapDescription = "Set spacing between items.";
    public const string AlignSelfTitle = "Align Self";
    public const string AlignSelfDescription = "Override alignment per item with Flex.AlignSelf.";
    public const string AutoWrapTitle = "Auto Wrap";
    public const string AutoWrapDescription = "Items wrap automatically.";
    public const string OrderTitle = "Order";
    public const string OrderDescription = "Change item order with Flex.Order.";
    public const string BasisTitle = "Basis";
    public const string BasisDescription = "Control initial size with Flex.Basis.";
    public const string FlexGrowTitle = "Flex Grow";
    public const string FlexGrowDescription = "Distribute free space with Flex.Grow.";
    public const string FlexShrinkTitle = "Flex Shrink (Weighted)";
    public const string FlexShrinkDescription = "Shrink is weighted by each item's base size.";
    public const string CombinationTitle = "Combination";
    public const string CombinationDescription = "Nested flex panels for complex layouts.";
    public const string PlaygroundTitle = "Flex Playground";
    public const string PlaygroundDescription = "Adjust all flex properties.";

    protected override Type GetResourceKindType() => typeof(FlexPanelShowCaseLangResourceKind);
}
