using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Grid;

[LanguageProvider(LanguageCode.zh_CN, GridShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioSpacing = "间距";
    public const string ScenarioAlignment = "对齐";
    public const string ScenarioOrder = "排序";
    public const string ScenarioColInfo = "ColInfo";

    public const string BasicGridTitle = "基础栅格";
    public const string BasicGridDescription = "基础的 24 栅格布局。";
    public const string GutterTitle = "间距";
    public const string GutterDescription = "水平、垂直和响应式间距。";
    public const string OffsetTitle = "偏移";
    public const string OffsetDescription = "将列向右移动。";
    public const string PushPullTitle = "推拉排序";
    public const string PushPullDescription = "使用 push/pull 改变列顺序。";
    public const string JustifyTitle = "主轴对齐";
    public const string JustifyDescription = "子元素在主轴上的对齐方式。";
    public const string AlignTitle = "交叉轴对齐";
    public const string AlignDescription = "子元素在交叉轴上的对齐方式。";
    public const string OrderTitle = "排序";
    public const string OrderDescription = "改变列顺序。";
    public const string ColInfoTitle = "ColInfo";
    public const string ColInfoDescription = "使用 ColInfo 进行响应式覆盖。";
    public const string P2TextCol = "col";
    public const string P2TextColN12 = "列-12";
    public const string P2TextColN8 = "列-8";
    public const string P2TextColN6 = "列-6";
    public const string P2TextHorizontal = "水平";
    public const string P2TextResponsive = "Responsive";
    public const string P2TextVertical = "垂直";
    public const string P2TextGutterString = "Gutter (string)";
    public const string P2TextColN8ColOffsetN8 = "列-8 列-offset-8";
    public const string P2TextColN6ColOffsetN6 = "列-6 列-offset-6";
    public const string P2TextColN12ColOffsetN6 = "列-12 列-offset-6";
    public const string P2TextColN6ColOffsetN18 = "列-6 列-offset-18";
    public const string P2TextColN18ColPushN6 = "列-18 列-push-6";
    public const string P2TextColN6ColPullN18 = "列-6 列-pull-18";
    public const string P2TextSubElementAlignLeft = "sub-element align left";
    public const string P2TextColN4 = "列-4";
    public const string P2TextSubElementAlignCenter = "sub-element align center";
    public const string P2TextSubElementAlignEnd = "sub-element align end";
    public const string P2TextSubElementAlignBetween = "sub-element align between";
    public const string P2TextSubElementAlignAround = "sub-element align around";
    public const string P2TextSubElementAlignEvenly = "sub-element align evenly";
    public const string P2TextAlignTop = "Align Top";
    public const string P2TextAlignMiddle = "Align Middle";
    public const string P2TextAlignBottom = "Align Bottom";
    public const string P2TextNormal = "普通";
    public const string P2TextN4ColOrderN1 = "4 col-order-1";
    public const string P2TextN3ColOrderN2 = "3 col-order-2";
    public const string P2TextN2ColOrderN3 = "2 col-order-3";
    public const string P2TextN1ColOrderN4 = "1 col-order-4";
    public const string P2TextN3ColOrderResponsive = "3 col-order-responsive";
    public const string P2TextN4ColOrderResponsive = "4 col-order-responsive";
    public const string P2TextN2ColOrderResponsive = "2 col-order-responsive";
    public const string P2TextN1ColOrderResponsive = "1 col-order-responsive";
    public const string P2TextBaseMdLgOverrides = "Base + Md/Lg overrides";
    public const string P2TextColN1WithColinfo = "列-1 with ColInfo";
    public const string P2TextColN2WithColinfo = "列-2 with ColInfo";
    public const string P2TextColN3WithColinfo = "列-3 with ColInfo";
    public const string P2TextColN4WithColinfo = "列-4 with ColInfo";
    public const string P2TextSharedResources = "Shared resources";
    public const string P2TextResColN1 = "res-col-1";
    public const string P2TextResColN2 = "res-col-2";
    public const string P2TextResColN3 = "res-col-3";
    public const string P2TextResColN4 = "res-col-4";

    protected override Type GetResourceKindType() => typeof(GridShowCaseLangResourceKind);
}
