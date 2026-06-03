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
    public const string P2TextCol = "col";
    public const string P2TextColN12 = "col-12";
    public const string P2TextColN8 = "col-8";
    public const string P2TextColN6 = "col-6";
    public const string P2TextHorizontal = "Horizontal";
    public const string P2TextResponsive = "Responsive";
    public const string P2TextVertical = "Vertical";
    public const string P2TextGutterString = "Gutter (string)";
    public const string P2TextColN8ColOffsetN8 = "col-8 col-offset-8";
    public const string P2TextColN6ColOffsetN6 = "col-6 col-offset-6";
    public const string P2TextColN12ColOffsetN6 = "col-12 col-offset-6";
    public const string P2TextColN6ColOffsetN18 = "col-6 col-offset-18";
    public const string P2TextColN18ColPushN6 = "col-18 col-push-6";
    public const string P2TextColN6ColPullN18 = "col-6 col-pull-18";
    public const string P2TextSubElementAlignLeft = "sub-element align left";
    public const string P2TextColN4 = "col-4";
    public const string P2TextSubElementAlignCenter = "sub-element align center";
    public const string P2TextSubElementAlignEnd = "sub-element align end";
    public const string P2TextSubElementAlignBetween = "sub-element align between";
    public const string P2TextSubElementAlignAround = "sub-element align around";
    public const string P2TextSubElementAlignEvenly = "sub-element align evenly";
    public const string P2TextAlignTop = "Align Top";
    public const string P2TextAlignMiddle = "Align Middle";
    public const string P2TextAlignBottom = "Align Bottom";
    public const string P2TextNormal = "Normal";
    public const string P2TextN4ColOrderN1 = "4 col-order-1";
    public const string P2TextN3ColOrderN2 = "3 col-order-2";
    public const string P2TextN2ColOrderN3 = "2 col-order-3";
    public const string P2TextN1ColOrderN4 = "1 col-order-4";
    public const string P2TextN3ColOrderResponsive = "3 col-order-responsive";
    public const string P2TextN4ColOrderResponsive = "4 col-order-responsive";
    public const string P2TextN2ColOrderResponsive = "2 col-order-responsive";
    public const string P2TextN1ColOrderResponsive = "1 col-order-responsive";
    public const string P2TextBaseMdLgOverrides = "Base + Md/Lg overrides";
    public const string P2TextColN1WithColinfo = "col-1 with ColInfo";
    public const string P2TextColN2WithColinfo = "col-2 with ColInfo";
    public const string P2TextColN3WithColinfo = "col-3 with ColInfo";
    public const string P2TextColN4WithColinfo = "col-4 with ColInfo";
    public const string P2TextSharedResources = "Shared resources";
    public const string P2TextResColN1 = "res-col-1";
    public const string P2TextResColN2 = "res-col-2";
    public const string P2TextResColN3 = "res-col-3";
    public const string P2TextResColN4 = "res-col-4";

    protected override Type GetResourceKindType() => typeof(GridShowCaseLangResourceKind);
}
