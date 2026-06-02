using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Grid;

[LanguageProvider(LanguageCode.en_US, GridShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicGridTitle = "Basic Grid";
    public const string BasicGridDescription = "Basic 24-column layout.";
    public const string GutterTitle = "Gutter";
    public const string GutterDescription = "Horizontal, vertical, and responsive gutter.";
    public const string OffsetTitle = "Offset";
    public const string OffsetDescription = "Move columns to the right.";
    public const string PushPullTitle = "Push & Pull";
    public const string PushPullDescription = "Change column order with push/pull.";
    public const string JustifyTitle = "Justify";
    public const string JustifyDescription = "Sub-element alignment on the main axis.";
    public const string AlignTitle = "Align";
    public const string AlignDescription = "Sub-element alignment on the cross axis.";
    public const string OrderTitle = "Order";
    public const string OrderDescription = "Change column order.";
    public const string ColInfoTitle = "ColInfo";
    public const string ColInfoDescription = "Use ColInfo for responsive overrides.";

    protected override Type GetResourceKindType() => typeof(GridShowCaseLangResourceKind);
}
