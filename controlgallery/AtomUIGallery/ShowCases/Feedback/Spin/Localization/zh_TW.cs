using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Spin;

[LanguageProvider(LanguageCode.zh_TW, SpinShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "簡單的加載狀態。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "小號 SpinIndicator 用於加載文本，默認尺寸用於卡片級區塊加載，大號用於頁面加載。";
    public const string CustomIndicatorTitle = "自定義加載指示器";
    public const string CustomIndicatorDescription = "使用自定義加載指示器。";
    public const string CustomizedDescriptionTitle = "自定義描述";
    public const string CustomizedDescriptionDescription = "自定義描述。";
    public const string EmbeddedModeTitle = "嵌入模式";
    public const string EmbeddedModeDescription = "將內容嵌入 Spin 後會進入加載狀態。";
    public const string P2DescriptionFurtherDetailsAboutTheContextOfThisAlert = "關於這條提示上下文的更多詳細信息。";
    public const string P2TextLoadingState = "加載狀態：";
    public const string P2TipLoading = "加載中...";

    public const string P2MessageAlertMessageTitle = "提示消息標題";

    protected override Type GetResourceKindType() => typeof(SpinShowCaseLangResourceKind);
}

