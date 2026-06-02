using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Carousel;

[LanguageProvider(LanguageCode.en_US, CarouselShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage.";
    public const string CardShapePositionTitle = "Card Shape Position";
    public const string CardShapePositionDescription = "Tab's position: left, right, top or bottom. Will auto switch to top in mobile.";
    public const string AutoScrollTitle = "Scroll automatically";
    public const string AutoScrollDescription = "Timing of scrolling to the next card/picture.";
    public const string FadeInTitle = "Fade in";
    public const string FadeInDescription = "Slides use fade for transition.";
    public const string SwitchArrowsTitle = "Arrows for switching";
    public const string SwitchArrowsDescription = "Show the arrows for switching.";
    public const string DotsProgressTitle = "Progress of dots";
    public const string DotsProgressDescription = "Show progress of dots.";

    protected override Type GetResourceKindType() => typeof(CarouselShowCaseLangResourceKind);
}
