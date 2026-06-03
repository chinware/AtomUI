using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Button;

[LanguageProvider(LanguageCode.zh_CN, ButtonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string TypeTitle = "按钮类型";
    public const string TypeDescription = "Ant Design 中包含主按钮、默认按钮、虚线按钮、文本按钮和链接按钮。";
    public const string ButtonShapeTitle = "按钮形状";
    public const string ButtonShapeDescription = "展示支持的按钮形状，例如主按钮、默认按钮、虚线按钮和文本按钮等。";
    public const string SizeTitle = "按钮尺寸";
    public const string SizeDescription = "AtomUI 支持小号、默认和大号三种按钮尺寸。需要大号或小号按钮时可设置 size 属性；省略 size 属性时使用默认尺寸。";
    public const string IconTitle = "图标";
    public const string IconDescription = "可以通过 icon 属性添加图标，并使用 iconPosition 调整图标位置。";
    public const string LoadingTitle = "加载状态";
    public const string LoadingDescription = "通过设置 Button 的 loading 属性，可以为按钮添加加载指示。";
    public const string BlockButtonTitle = "通栏按钮";
    public const string BlockButtonDescription = "block 属性会让按钮宽度适配父容器。";
    public const string DangerButtonsTitle = "危险按钮";
    public const string DangerButtonsDescription = "danger 是 Ant Design 4.0 之后提供的按钮属性。";
    public const string GhostButtonTitle = "幽灵按钮";
    public const string GhostButtonDescription = "ghost 属性会让按钮背景透明，通常用于有色背景上。";
    public const string DisabledTitle = "禁用状态";
    public const string DisabledDescription = "为按钮添加 disabled 属性可以将按钮标记为禁用状态。";
    public const string P2ContentPrimaryButton = "主要按钮";
    public const string P2ContentDefaultButton = "默认按钮";
    public const string P2ContentDashed = "虚线";
    public const string P2ContentTextButton = "文本按钮";
    public const string P2ContentLinkButton = "链接按钮";
    public const string P2ContentPrimary = "主要";
    public const string P2ContentDefault = "默认";
    public const string P2ContentText = "文本";
    public const string P2ContentLink = "链接";
    public const string P2ContentAa = "AA";
    public const string P2TextExpandDirection = "按钮尺寸：";
    public const string P2ContentLarge = "大号";
    public const string P2ContentSmall = "小号";
    public const string P2ContentDownload = "下载";
    public const string P2ContentSearch = "搜索";
    public const string P2ContentLoading = "加载中";
    public const string P2ContentClickMe = "点我！";
    public const string P2ContentDanger = "危险";
    public const string P2ContentPrimaryDisabled = "主要（禁用）";
    public const string P2ContentDefaultDisabled = "默认（禁用）";
    public const string P2ContentDashedDisabled = "虚线（禁用）";
    public const string P2ContentTextDisabled = "文本（禁用）";
    public const string P2ContentLinkDisabled = "链接（禁用）";
    public const string P2ContentDangerPrimary = "危险主要";
    public const string P2ContentDangerPrimaryDisabled = "危险主要（禁用）";
    public const string P2ContentDangerDefault = "危险默认";
    public const string P2ContentDangerDefaultDisabled = "危险默认（禁用）";
    public const string P2ContentDangerText = "危险文本";
    public const string P2ContentDangerTextDisabled = "危险文本（禁用）";
    public const string P2ContentDangerLink = "危险链接";
    public const string P2ContentDangerLinkDisabled = "危险链接（禁用）";
    public const string P2ContentGhost = "幽灵";
    public const string P2ContentGhostDisabled = "幽灵（禁用）";

    protected override Type GetResourceKindType() => typeof(ButtonShowCaseLangResourceKind);
}
