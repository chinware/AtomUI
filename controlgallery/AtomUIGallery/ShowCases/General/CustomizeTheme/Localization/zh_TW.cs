using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_TW, CustomizeThemeShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string CustomizeDesignTokenTitle = "自定義設計令牌";
    public const string CustomizeDesignTokenDescription = "通過修改主題的 token 屬性，可以全局修改 Design Token。部分 token 會影響其他 token，這類 token 稱為 Seed Token。";
    public const string PresetAlgorithmsTitle = "使用預設算法";
    public const string PresetAlgorithmsDescription = "通過修改 algorithm 可以快速生成不同風格的主題。Ant Design 5.0 默認提供三套預設算法：DefaultAlgorithm、DarkAlgorithm、CompactAlgorithm。";
    public const string CustomizeComponentTokenTitle = "自定義組件令牌";
    public const string CustomizeComponentTokenDescription = "除 Design Token 外，每個組件也有自己的 Component Token，用於實現組件級樣式定制，不同組件之間互不影響。組件的其他 Design Token 也可以用這種方式覆蓋。";
    public const string NestedThemeTitle = "嵌套主題";
    public const string NestedThemeDescription = "通過嵌套 ConfigProvider，可以為頁面的局部區域應用局部主題。子主題中未修改的 Design Token 會繼承父主題。";
    public const string P2PlaceholderTextPleaseInput = "請輸入";
    public const string P2ContentPrimaryButton = "主要按鈕";
    public const string P2ContentDefaultButton = "默認按鈕";
    public const string P2ContentTextButton = "文本按鈕";
    public const string P2ContentLinkButton = "鏈接按鈕";
    public const string P2ContentApple = "蘋果";
    public const string P2ContentPear = "梨";
    public const string P2ContentOrange = "橙子";
    public const string P2ContentSubmit = "提交";
    public const string P2TextEnableAlgorithm = "啓用算法：";
    public const string P2TextDisableAlgorithm = "禁用算法：";
    public const string P2ContentThemeN1 = "主題 1";
    public const string P2ContentThemeN2 = "主題 2";

    protected override Type GetResourceKindType() => typeof(CustomizeThemeShowCaseLangResourceKind);
}

