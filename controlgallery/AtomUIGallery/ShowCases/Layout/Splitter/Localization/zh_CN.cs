using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Splitter;

[LanguageProvider(LanguageCode.zh_CN, SplitterShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(SplitterShowCaseLangResourceKind);
}
