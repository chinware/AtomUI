using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Skeleton;

[LanguageProvider(LanguageCode.zh_TW, SkeletonShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的 Skeleton 用法。";
    public const string ComplexCombinationTitle = "複雜組合";
    public const string ComplexCombinationDescription = "包含頭像和多段文本的複雜組合。";
    public const string ActiveAnimationTitle = "動態效果";
    public const string ActiveAnimationDescription = "顯示動態加載效果。";
    public const string ButtonAvatarInputImageNodeTitle = "按鈕/頭像/輸入框/圖片/節點";
    public const string ButtonAvatarInputImageNodeDescription = "Skeleton Button、Avatar、Input、Image 和 Node。";
    public const string ContainsSubComponentTitle = "包含子組件";
    public const string ContainsSubComponentDescription = "Skeleton 包含子組件。";
    public const string P2TextActive = "動態效果：";
    public const string P2TextButtonAndInputBlock = "按鈕和輸入框塊：";
    public const string P2TextSize = "尺寸：";
    public const string P2ContentDefault = "默認";
    public const string P2ContentLarge = "大號";
    public const string P2ContentSmall = "小號";
    public const string P2TextButtonShape = "按鈕形狀：";
    public const string P2ContentSquare = "方形";
    public const string P2ContentRound = "圓角";
    public const string P2ContentCircle = "圓形";
    public const string P2TextAvatarShape = "頭像形狀：";
    public const string P2TextAntDesignADesignLanguage = "Ant Design，一套設計語言";
    public const string P2TextWeSupplyASeriesOfDesignPrinciplesPractical = "我們提供一系列設計原則、實用模式和高質量設計資源（Sketch 和 Axure），幫助人們高效而優雅地創建產品原型。";
    public const string P2ContentShowSkeleton = "顯示骨架屏";

    protected override Type GetResourceKindType() => typeof(SkeletonShowCaseLangResourceKind);
}

