using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Button;

[LanguageProvider(LanguageCode.zh_TW, ButtonShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string TypeTitle = "按鈕類型";
    public const string TypeDescription = "Ant Design 中包含主按鈕、默認按鈕、虛線按鈕、文本按鈕和鏈接按鈕。";
    public const string ButtonShapeTitle = "按鈕形狀";
    public const string ButtonShapeDescription = "展示支持的按鈕形狀，例如主按鈕、默認按鈕、虛線按鈕和文本按鈕等。";
    public const string SizeTitle = "按鈕尺寸";
    public const string SizeDescription = "AtomUI 支持小號、默認和大號三種按鈕尺寸。需要大號或小號按鈕時可設置 size 屬性；省略 size 屬性時使用默認尺寸。";
    public const string IconTitle = "圖標";
    public const string IconDescription = "可以通過 icon 屬性添加圖標，並使用 iconPosition 調整圖標位置。";
    public const string LoadingTitle = "加載狀態";
    public const string LoadingDescription = "通過設置 Button 的 loading 屬性，可以為按鈕添加加載指示。";
    public const string BlockButtonTitle = "通欄按鈕";
    public const string BlockButtonDescription = "block 屬性會讓按鈕寬度適配父容器。";
    public const string DangerButtonsTitle = "危險按鈕";
    public const string DangerButtonsDescription = "danger 是 Ant Design 4.0 之後提供的按鈕屬性。";
    public const string GhostButtonTitle = "幽靈按鈕";
    public const string GhostButtonDescription = "ghost 屬性會讓按鈕背景透明，通常用於有色背景上。";
    public const string DisabledTitle = "禁用狀態";
    public const string DisabledDescription = "為按鈕添加 disabled 屬性可以將按鈕標記為禁用狀態。";
    public const string P2ContentPrimaryButton = "主要按鈕";
    public const string P2ContentDefaultButton = "默認按鈕";
    public const string P2ContentDashed = "虛線";
    public const string P2ContentTextButton = "文本按鈕";
    public const string P2ContentLinkButton = "鏈接按鈕";
    public const string P2ContentPrimary = "主要";
    public const string P2ContentDefault = "默認";
    public const string P2ContentText = "文本";
    public const string P2ContentLink = "鏈接";
    public const string P2ContentAa = "AA";
    public const string P2TextExpandDirection = "按鈕尺寸：";
    public const string P2ContentLarge = "大號";
    public const string P2ContentSmall = "小號";
    public const string P2ContentDownload = "下載";
    public const string P2ContentSearch = "搜索";
    public const string P2ContentLoading = "加載中";
    public const string P2ContentClickMe = "點我！";
    public const string P2ContentDanger = "危險";
    public const string P2ContentPrimaryDisabled = "主要（禁用）";
    public const string P2ContentDefaultDisabled = "默認（禁用）";
    public const string P2ContentDashedDisabled = "虛線（禁用）";
    public const string P2ContentTextDisabled = "文本（禁用）";
    public const string P2ContentLinkDisabled = "鏈接（禁用）";
    public const string P2ContentDangerPrimary = "危險主要";
    public const string P2ContentDangerPrimaryDisabled = "危險主要（禁用）";
    public const string P2ContentDangerDefault = "危險默認";
    public const string P2ContentDangerDefaultDisabled = "危險默認（禁用）";
    public const string P2ContentDangerText = "危險文本";
    public const string P2ContentDangerTextDisabled = "危險文本（禁用）";
    public const string P2ContentDangerLink = "危險鏈接";
    public const string P2ContentDangerLinkDisabled = "危險鏈接（禁用）";
    public const string P2ContentGhost = "幽靈";
    public const string P2ContentGhostDisabled = "幽靈（禁用）";

    protected override Type GetResourceKindType() => typeof(ButtonShowCaseLangResourceKind);
}

