using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Skeleton;

[LanguageProvider(LanguageCode.en_US, SkeletonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Skeleton usage.";
    public const string ComplexCombinationTitle = "Complex combination";
    public const string ComplexCombinationDescription = "Complex combination with avatar and multiple paragraphs.";
    public const string ActiveAnimationTitle = "Active Animation";
    public const string ActiveAnimationDescription = "Display active animation.";
    public const string ButtonAvatarInputImageNodeTitle = "Button/Avatar/Input/Image/Node";
    public const string ButtonAvatarInputImageNodeDescription = "Skeleton Button, Avatar, Input, Image and Node.";
    public const string ContainsSubComponentTitle = "Contains sub component";
    public const string ContainsSubComponentDescription = "Skeleton contains sub component.";
    public const string P2TextActive = "Active:";
    public const string P2TextButtonAndInputBlock = "Button and Input Block:";
    public const string P2TextSize = "Size:";
    public const string P2ContentDefault = "Default";
    public const string P2ContentLarge = "Large";
    public const string P2ContentSmall = "Small";
    public const string P2TextButtonShape = "Button Shape:";
    public const string P2ContentSquare = "Square";
    public const string P2ContentRound = "Round";
    public const string P2ContentCircle = "Circle";
    public const string P2TextAvatarShape = "Avatar Shape:";
    public const string P2TextAntDesignADesignLanguage = "Ant Design, a design language";
    public const string P2TextWeSupplyASeriesOfDesignPrinciplesPractical = "We supply a series of design principles, practical patterns and high quality design resources (Sketch and Axure), to help people create their product prototypes beautifully and efficiently.";
    public const string P2ContentShowSkeleton = "Show Skeleton";

    protected override Type GetResourceKindType() => typeof(SkeletonShowCaseLangResourceKind);
}
