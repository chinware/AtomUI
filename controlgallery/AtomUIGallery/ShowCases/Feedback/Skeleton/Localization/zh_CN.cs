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
    public const string P2TextActive = "动态效果：";
    public const string P2TextButtonAndInputBlock = "按钮和输入框块：";
    public const string P2TextSize = "尺寸：";
    public const string P2ContentDefault = "默认";
    public const string P2ContentLarge = "大号";
    public const string P2ContentSmall = "小号";
    public const string P2TextButtonShape = "按钮形状：";
    public const string P2ContentSquare = "方形";
    public const string P2ContentRound = "圆角";
    public const string P2ContentCircle = "圆形";
    public const string P2TextAvatarShape = "头像形状：";
    public const string P2TextAntDesignADesignLanguage = "Ant Design，一套设计语言";
    public const string P2TextWeSupplyASeriesOfDesignPrinciplesPractical = "我们提供一系列设计原则、实用模式和高质量设计资源（Sketch 和 Axure），帮助人们高效而优雅地创建产品原型。";
    public const string P2ContentShowSkeleton = "显示骨架屏";

    protected override Type GetResourceKindType() => typeof(SkeletonShowCaseLangResourceKind);
}
