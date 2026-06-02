using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Grid;

[LanguageProvider(LanguageCode.zh_CN, GridShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
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

    protected override Type GetResourceKindType() => typeof(GridShowCaseLangResourceKind);
}
