using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.FloatButton;

[LanguageProvider(LanguageCode.zh_TW, FloatButtonShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string TypeTitle = "類型";
    public const string TypeDescription = "通過 type 屬性改變 FloatButton 的類型。";
    public const string ShapeTitle = "形狀";
    public const string ShapeDescription = "通過 shape 屬性改變 FloatButton 的形狀。";
    public const string TooltipTitle = "帶提示的 FloatButton";
    public const string TooltipDescription = "設置 tooltip 屬性可以顯示帶提示的 FloatButton。";
    public const string DescriptionTitle = "描述";
    public const string DescriptionDescription = "設置 description 屬性可以顯示帶描述的 FloatButton。";
    public const string GroupTitle = "FloatButton 組合";
    public const string GroupDescription = "多個按鈕一起使用時推薦使用 FloatButtonGroup。通過設置 FloatButton.Group 的 shape 屬性可以改變組合形狀，組合形狀會覆蓋內部 FloatButton 的形狀。";
    public const string MenuModeTitle = "菜單模式";
    public const string MenuModeDescription = "通過 trigger 打開菜單模式，可選擇 hover 或 click。";
    public const string ControlledModeTitle = "受控模式";
    public const string ControlledModeDescription = "通過 open 將組件設置為受控模式，需要與 trigger 一起使用。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "自定義動畫彈出位置，提供 top、right、bottom、left 四種預設位置，默認是 top。";
    public const string BadgeTitle = "徽標";
    public const string BadgeDescription = "帶 Badge 的 FloatButton。";
    public const string BackTopTitle = "回到頂部";
    public const string BackTopDescription = "BackTop 可以方便地回到頁面頂部。";
    public const string P2TooltipSinceN5N25N0 = "自 5.25.0 起";
    public const string P2TooltipDocuments = "文檔";
    public const string P2DescriptionHelpInfo = "幫助信息";
    public const string P2TextScrollToBottom = "滾動到底部";

    protected override Type GetResourceKindType() => typeof(FloatButtonShowCaseLangResourceKind);
}

