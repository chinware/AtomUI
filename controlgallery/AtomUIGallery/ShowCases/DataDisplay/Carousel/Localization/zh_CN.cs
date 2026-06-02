using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Carousel;

[LanguageProvider(LanguageCode.zh_CN, CarouselShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础用法。";
    public const string CardShapePositionTitle = "卡片形态位置";
    public const string CardShapePositionDescription = "标签位置可为左、右、上或下。在移动端会自动切换到顶部。";
    public const string AutoScrollTitle = "自动滚动";
    public const string AutoScrollDescription = "定时滚动到下一张卡片或图片。";
    public const string FadeInTitle = "淡入切换";
    public const string FadeInDescription = "幻灯片使用淡入效果进行过渡。";
    public const string SwitchArrowsTitle = "切换箭头";
    public const string SwitchArrowsDescription = "显示用于切换的箭头。";
    public const string DotsProgressTitle = "指示点进度";
    public const string DotsProgressDescription = "显示指示点的进度。";

    protected override Type GetResourceKindType() => typeof(CarouselShowCaseLangResourceKind);
}
