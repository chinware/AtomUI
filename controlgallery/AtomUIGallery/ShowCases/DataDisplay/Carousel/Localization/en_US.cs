using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Carousel;

[LanguageProvider(LanguageCode.en_US, CarouselShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Display a rotating set of content panels in a limited space.";
    public const string PageDescription = "Carousel cycles through images, cards, or promotion panels. It supports autoplay, pagination positions, fade transitions, navigation arrows, infinite scrolling, progress indicators, and swipe gestures.";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyIsShowNavButtons = "Controls whether previous and next navigation buttons are visible.";
    public const string ApiPropertyIsAutoPlay = "Automatically advances to the next page after the configured interval.";
    public const string ApiPropertyAutoPlaySpeed = "Interval used between automatic page changes.";
    public const string ApiPropertyPaginationPosition = "Controls where the pagination indicators are placed.";
    public const string ApiPropertyIsShowPagination = "Controls whether pagination indicators are visible.";
    public const string ApiPropertyIsShowTransitionProgress = "Shows progress inside the pagination indicator during autoplay.";
    public const string ApiPropertyIsInfinite = "Allows the carousel to loop from the last page back to the first.";
    public const string ApiPropertyPageTransitionDuration = "Duration used by the page transition animation.";
    public const string ApiPropertyTransitionEffect = "Selects the page transition effect.";
    public const string ApiPropertyIsMotionEnabled = "Controls motion effects for supported carousel interactions.";
    public const string ApiPropertyIsSwipeEnabled = "Enables pointer swipe gestures for page switching.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameIndicatorWidth = "Default pagination indicator width.";
    public const string TokenNameIndicatorHeight = "Pagination indicator height.";
    public const string TokenNameIndicatorGap = "Spacing between pagination indicators.";
    public const string TokenNamePaginationOffset = "Distance from the pagination indicators to the carousel edge.";
    public const string TokenNameIndicatorActiveWidth = "Width of the active pagination indicator.";
    public const string TokenNameArrowSize = "Size of the navigation arrow icons.";
    public const string TokenNameArrowOffset = "Distance from navigation arrows to the carousel edge.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
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
    public const string P2TextPaginationPosition = "Pagination Position:";
    public const string P2ContentTop = "Top";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentLeft = "Left";
    public const string P2ContentRight = "Right";

    protected override Type GetResourceKindType() => typeof(CarouselShowCaseLangResourceKind);
}
