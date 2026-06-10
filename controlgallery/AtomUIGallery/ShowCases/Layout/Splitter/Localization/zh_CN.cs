using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Splitter;

[LanguageProvider(LanguageCode.zh_CN, SplitterShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "带最小/最大约束的初始尺寸。";
    public const string HorizontalTitle = "水平分割";
    public const string HorizontalDescription = "带可折叠图标的上下分割器。";
    public const string CompositeTitle = "组合布局";
    public const string CompositeDescription = "在垂直布局中嵌套水平分割器。";
    public const string ResizableDisabledTitle = "禁用拖拽调整";
    public const string ResizableDisabledDescription = "任意一侧禁用 resizable 时，拖拽会被阻止。";
    public const string ShowCollapsibleIconTitle = "显示折叠图标";
    public const string ShowCollapsibleIconDescription = "三个面板展示 hover 和始终可见的折叠图标。";
    public const string MultiPanelsTitle = "多个面板";
    public const string MultiPanelsDescription = "垂直布局中的多个面板。";
    public const string LazyTitle = "延迟模式";
    public const string LazyDescription = "延迟渲染模式：拖拽释放后更新尺寸。";
    public const string P2TextFirst = "第一项";
    public const string P2TextSecond = "第二项";
    public const string P2TextTop = "顶部";
    public const string P2TextBottom = "底部";
    public const string P2TextLeft = "左侧";
    public const string P2TextResizable = "Resizable";
    public const string P2TextNotResizable = "Not Resizable";
    public const string P2TextShowcollapsibleicon = "ShowCollapsibleIcon:";
    public const string P2ContentHover = "悬停";
    public const string P2ContentTrue = "True";
    public const string P2ContentFalse = "False";
    public const string P2TextThird = "第三项";
    public const string P2TextA = "A";
    public const string P2TextB = "B";
    public const string P2TextC = "C";
    public const string P2TextD = "D";

    protected override Type GetResourceKindType() => typeof(SplitterShowCaseLangResourceKind);
}
