using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Spin;

[LanguageProvider(LanguageCode.zh_CN, SpinShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "简单的加载状态。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "小号 SpinIndicator 用于加载文本，默认尺寸用于卡片级区块加载，大号用于页面加载。";
    public const string CustomIndicatorTitle = "自定义加载指示器";
    public const string CustomIndicatorDescription = "使用自定义加载指示器。";
    public const string CustomizedDescriptionTitle = "自定义描述";
    public const string CustomizedDescriptionDescription = "自定义描述。";
    public const string EmbeddedModeTitle = "嵌入模式";
    public const string EmbeddedModeDescription = "将内容嵌入 Spin 后会进入加载状态。";
    public const string P2DescriptionFurtherDetailsAboutTheContextOfThisAlert = "关于这条提示上下文的更多详细信息。";
    public const string P2TextLoadingState = "加载状态：";
    public const string P2TipLoading = "加载中...";

    public const string P2MessageAlertMessageTitle = "提示消息标题";

    protected override Type GetResourceKindType() => typeof(SpinShowCaseLangResourceKind);
}
