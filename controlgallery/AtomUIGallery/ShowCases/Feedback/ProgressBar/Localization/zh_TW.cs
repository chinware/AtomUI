using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ProgressBar;

[LanguageProvider(LanguageCode.zh_TW, ProgressBarShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAdvanced = "進階";
    public const string ScenarioLayout = "佈局";
    public const string ProgressBarTitle = "進度條";
    public const string ProgressBarDescription = "標準進度條。";
    public const string CircularProgressBarTitle = "環形進度條";
    public const string CircularProgressBarDescription = "環形進度條。";
    public const string MiniSizeTitle = "迷你尺寸進度條";
    public const string MiniSizeDescription = "適合較窄區域。";
    public const string SmallerCircularProgressBarTitle = "更小的環形進度條";
    public const string SmallerCircularProgressBarDescription = "更小的環形進度條。";
    public const string DynamicTitle = "動態進度";
    public const string DynamicDescription = "動態進度條效果更好。";
    public const string CustomTextFormatTitle = "自定義文本格式";
    public const string CustomTextFormatDescription = "可以通過設置 format 屬性自定義文本。";
    public const string DashboardTitle = "儀錶盤";
    public const string DashboardDescription = "設置 type=dashboard 可以輕鬆獲得儀錶盤樣式進度條。修改 gapDegree 可設置缺口角度。";
    public const string SuccessSegmentTitle = "帶成功分段的進度條";
    public const string SuccessSegmentDescription = "展示多個不同狀態的進度片段。";
    public const string StrokeLinecapTitle = "線帽樣式";
    public const string StrokeLinecapDescription = "設置 strokeLinecap='butt' 可以將線帽從 round 改為 butt，更多信息請參考 stroke-linecap。";
    public const string CustomLineGradientTitle = "自定義線性漸變";
    public const string CustomLineGradientDescription = "漸變封裝；設置漸變時 circle 和 dashboard 會忽略 strokeLinecap。";
    public const string StepsTitle = "步驟進度條";
    public const string StepsDescription = "帶步驟的進度條。";
    public const string CircularStepsTitle = "帶步驟的環形進度條";
    public const string CircularStepsDescription = "支持步驟和顏色分段的環形進度條，默認間隔為 2px。";
    public const string PercentPositionTitle = "調整進度值位置";
    public const string PercentPositionDescription = "改變進度值的位置，可使用 percentPosition 將進度值調整到進度條內部、外部或底部。";
    public const string StepsPercentPositionTitle = "調整步驟進度值位置";
    public const string StepsPercentPositionDescription = "改變步驟進度值的位置，可使用 percentPosition 將進度值調整到進度條內部、外部或底部。";
    public const string VerticalLinearTitle = "垂直線性進度條";
    public const string VerticalLinearDescription = "普通線性進度條，支持指定附加區域的位置。";
    public const string VerticalStepsTitle = "垂直步驟進度條";
    public const string VerticalStepsDescription = "普通步驟進度條，支持指定附加區域的位置。";
    public const string ToggleDisabledStatusTitle = "切換禁用狀態";
    public const string ToggleDisabledStatusDescription = "進度條處於禁用狀態並使用禁用樣式。";
    public const string P2ContentSub = "Sub";
    public const string P2ContentAdd = "Add";

    protected override Type GetResourceKindType() => typeof(ProgressBarShowCaseLangResourceKind);
}

