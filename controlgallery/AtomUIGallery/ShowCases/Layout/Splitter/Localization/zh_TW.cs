using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Splitter;

[LanguageProvider(LanguageCode.zh_TW, SplitterShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "帶最小/最大約束的初始尺寸。";
    public const string HorizontalTitle = "水平分割";
    public const string HorizontalDescription = "帶可折疊圖標的上下分割器。";
    public const string CompositeTitle = "組合佈局";
    public const string CompositeDescription = "在垂直佈局中嵌套水平分割器。";
    public const string ResizableDisabledTitle = "禁用拖拽調整";
    public const string ResizableDisabledDescription = "任意一側禁用 resizable 時，拖拽會被阻止。";
    public const string ShowCollapsibleIconTitle = "顯示折疊圖標";
    public const string ShowCollapsibleIconDescription = "三個面板展示 hover 和始終可見的折疊圖標。";
    public const string MultiPanelsTitle = "多個面板";
    public const string MultiPanelsDescription = "垂直佈局中的多個面板。";
    public const string LazyTitle = "延遲模式";
    public const string LazyDescription = "延遲渲染模式：拖拽釋放後更新尺寸。";
    public const string P2TextFirst = "第一項";
    public const string P2TextSecond = "第二項";
    public const string P2TextTop = "頂部";
    public const string P2TextBottom = "底部";
    public const string P2TextLeft = "左側";
    public const string P2TextResizable = "Resizable";
    public const string P2TextNotResizable = "Not Resizable";
    public const string P2TextShowcollapsibleicon = "ShowCollapsibleIcon:";
    public const string P2ContentHover = "懸停";
    public const string P2ContentTrue = "True";
    public const string P2ContentFalse = "False";
    public const string P2TextThird = "第三項";
    public const string P2TextA = "A";
    public const string P2TextB = "B";
    public const string P2TextC = "C";
    public const string P2TextD = "D";

    protected override Type GetResourceKindType() => typeof(SplitterShowCaseLangResourceKind);
}

