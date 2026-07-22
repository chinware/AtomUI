using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

[LanguageProvider(LanguageCode.zh_TW, CustomizeThemeShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "示例";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "通過共享令牌、算法、組件令牌和局部嵌套作用域定制 AtomUI 主題。";
    public const string PageDescription = "Customize Theme 展示 ThemeConfigProvider 如何覆蓋 Seed Token、派生 Token、預設算法、組件級 Token 和嵌套主題上下文，而不需要修改應用全局主題。";
    public const string ApiMemberThemeConfigProviderConfig = "不可變的局部主題配置；運行期更新需要替換完整配置值。";
    public const string ApiMemberThemeConfigInherit = "是否繼承並合併父級主題配置。";
    public const string ApiMemberThemeConfigAlgorithms = "有序的全局算法標識；null 表示繼承父級算法鏈。";
    public const string ApiMemberThemeConfigTokens = "按 Token 名稱索引的全局 Token 覆蓋。";
    public const string ApiMemberThemeConfigControls = "按 ControlTokenIdentity 索引的控件主題配置。";
    public const string ApiMemberControlThemeConfigAlgorithm = "控件算法模式：Unspecified、Disabled、Global 或 Custom。";
    public const string ApiMemberControlThemeConfigAlgorithms = "僅在 Algorithm 為 Custom 時使用的有序控件算法。";
    public const string ApiMemberControlThemeConfigTokens = "僅應用到目標控件 Token 家族的 Token 覆蓋。";
    public const string CustomizeDesignTokenTitle = "自定義設計令牌";
    public const string CustomizeDesignTokenDescription = "通過修改主題的 token 屬性，可以全局修改 Design Token。部分 token 會影響其他 token，這類 token 稱為 Seed Token。";
    public const string PresetAlgorithmsTitle = "使用預設算法";
    public const string PresetAlgorithmsDescription = "通過修改 algorithm 可以快速生成不同風格的主題。Ant Design 5.0 默認提供三套預設算法：DefaultAlgorithm、DarkAlgorithm、CompactAlgorithm。";
    public const string RuntimeTokenUpdatesTitle = "運行期令牌更新";
    public const string RuntimeTokenUpdatesDescription = "使用新的不可變 ThemeConfig 替換 ThemeConfigProvider.Config，即可發布一個局部 Snapshot，並且不會追加資源 Provider 層。";
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
    public const string P2ContentUseBlue = "藍色";
    public const string P2ContentUseGreen = "綠色";
    public const string P2ContentUseMagenta = "洋紅";
    public const string P2TextEnableAlgorithm = "啓用算法：";
    public const string P2TextDisableAlgorithm = "禁用算法：";
    public const string P2ContentThemeN1 = "主題 1";
    public const string P2ContentThemeN2 = "主題 2";
    public const string P2ContentThemeN3 = "不繼承";

}
