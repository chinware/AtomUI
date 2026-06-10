using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Segmented;

[LanguageProvider(LanguageCode.zh_TW, SegmentedShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string BlockSegmentedTitle = "塊級分段控制器";
    public const string BlockSegmentedDescription = "block 屬性會讓 Segmented 適配父容器寬度。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "禁用狀態的 Segmented。";
    public const string ThreeSizesTitle = "Segmented 三種尺寸";
    public const string ThreeSizesDescription = "Segmented 有三種尺寸：大號（40px）、默認（32px）和小號（24px）。";
    public const string IconOnlyTitle = "僅圖標";
    public const string IconOnlyDescription = "為 Segmented 項設置圖標但不設置標籤。";
    public const string WithIconTitle = "帶圖標";
    public const string WithIconDescription = "為 Segmented 項設置圖標。";
    public const string P2ContentDaily = "每日";
    public const string P2ContentWeekly = "每周";
    public const string P2ContentMonthly = "每月";
    public const string P2ContentQuarterly = "每季度";
    public const string P2ContentYearly = "每年";
    public const string P2ContentLongtextLongtextLongtextLongtext = "長文本-長文本-長文本-長文本";
    public const string P2ContentMap = "地圖";
    public const string P2ContentTransit = "公交";
    public const string P2ContentSatellite = "衛星";
    public const string P2ContentList = "列表";
    public const string P2ContentAva = "Ava";

    public const string P2ContentKanban = "看板";

    public const string P2ContentAtomUI = "AtomUI";

    protected override Type GetResourceKindType() => typeof(SegmentedShowCaseLangResourceKind);
}

