using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Carousel;

[LanguageProvider(LanguageCode.zh_TW, CarouselShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎用法。";
    public const string CardShapePositionTitle = "卡片形態位置";
    public const string CardShapePositionDescription = "標籤位置可為左、右、上或下。在移動端會自動切換到頂部。";
    public const string AutoScrollTitle = "自動滾動";
    public const string AutoScrollDescription = "定時滾動到下一張卡片或圖片。";
    public const string FadeInTitle = "淡入切換";
    public const string FadeInDescription = "幻燈片使用淡入效果進行過渡。";
    public const string SwitchArrowsTitle = "切換箭頭";
    public const string SwitchArrowsDescription = "顯示用於切換的箭頭。";
    public const string DotsProgressTitle = "指示點進度";
    public const string DotsProgressDescription = "顯示指示點的進度。";
    public const string P2TextPaginationPosition = "Pagination Position:";
    public const string P2ContentTop = "頂部";
    public const string P2ContentBottom = "底部";
    public const string P2ContentLeft = "左側";
    public const string P2ContentRight = "右側";

    protected override Type GetResourceKindType() => typeof(CarouselShowCaseLangResourceKind);
}

