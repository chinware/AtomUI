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

    protected override Type GetResourceKindType() => typeof(SkeletonShowCaseLangResourceKind);
}
