using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Slider;

[LanguageProvider(LanguageCode.zh_TW, SliderShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎滑塊。range 為 true 時顯示為雙滑塊模式；disable 為 true 時滑塊不可交互。";
    public const string CustomizeTooltipTitle = "自定義提示";
    public const string CustomizeTooltipDescription = "使用 tooltip.formatter 格式化 Tooltip 內容；當 tooltip.formatter 為 null 時隱藏提示。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直滑塊。";
    public const string GraduatedSliderTitle = "帶刻度的滑塊";
    public const string GraduatedSliderDescription = "使用 marks 屬性標記帶刻度滑塊，使用 value 或 defaultValue 指定滑塊位置。當 included 為 false 時，不同滑塊相互獨立；當 step 為 null 時，用戶只能將滑塊拖到刻度上。";
    public const string P2TextEnabled = "Enabled:";
    public const string P2TextIncludedTrue = "included=true";
    public const string P2TextIncludedFalse = "included=false";

    protected override Type GetResourceKindType() => typeof(SliderShowCaseLangResourceKind);
}

