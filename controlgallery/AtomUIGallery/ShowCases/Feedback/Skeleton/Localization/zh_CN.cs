using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Skeleton;

[LanguageProvider(LanguageCode.zh_CN, SkeletonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的 Skeleton 用法。";
    public const string ComplexCombinationTitle = "复杂组合";
    public const string ComplexCombinationDescription = "包含头像和多段文本的复杂组合。";
    public const string ActiveAnimationTitle = "动态效果";
    public const string ActiveAnimationDescription = "显示动态加载效果。";
    public const string ButtonAvatarInputImageNodeTitle = "按钮/头像/输入框/图片/节点";
    public const string ButtonAvatarInputImageNodeDescription = "Skeleton Button、Avatar、Input、Image 和 Node。";
    public const string ContainsSubComponentTitle = "包含子组件";
    public const string ContainsSubComponentDescription = "Skeleton 包含子组件。";

    protected override Type GetResourceKindType() => typeof(SkeletonShowCaseLangResourceKind);
}
