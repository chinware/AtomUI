using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_TW, CustomizeThemeShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "通過共享令牌、算法、組件令牌和局部嵌套作用域定制 AtomUI 主題。";
    public const string PageDescription = "Customize Theme 展示 ThemeConfigProvider 如何覆蓋 Seed Token、派生 Token、預設算法、組件級 Token 和嵌套主題上下文，而不需要修改應用全局主題。";
    public const string ApiColumnMember = "成員";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiMemberThemeConfigProviderAlgorithms = "當前 Provider 應用的主題算法，例如 Default、Dark 或 Compact。";
    public const string ApiMemberThemeConfigProviderSharedTokenSetters = "共享令牌覆蓋，會影響 Provider 下的所有後代控件。";
    public const string ApiMemberThemeConfigProviderControlTokenInfoSetters = "按控件 TokenId 分組的組件令牌覆蓋配置。";
    public const string ApiMemberTokenSetterKey = "需要覆蓋的 Design Token 鍵。";
    public const string ApiMemberTokenSetterValue = "用於覆蓋 Token 的字符串值。";
    public const string ApiMemberTokenSetterCatalog = "可選的 Token 目錄，用於指定某個目錄下的 Token。";
    public const string ApiMemberControlTokenInfoSetter = "控件令牌 ID，例如 Button 或 AddOnDecoratedBox。";
    public const string ApiMemberControlTokenInfoSetterEnableAlgorithm = "組件令牌覆蓋是否參與主題算法計算。";
    public const string ApiMemberControlTokenInfoSetterSetters = "應用到當前組件令牌的 TokenSetter 集合。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameColorPrimary = "交互控件使用的主品牌色。";
    public const string TokenNameBorderRadius = "用於派生組件圓角的共享 Seed Token。";
    public const string TokenNameColorBgContainer = "控件和內容容器使用的背景色。";
    public const string TokenNameButtonColorPrimary = "Button 組件的主色覆蓋。";
    public const string TokenNameAddOnDecoratedBoxColorPrimary = "AddOnDecoratedBox 組件主色覆蓋，用於輸入裝飾區域。";
    public const string TokenScopeShared = "共享";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
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
