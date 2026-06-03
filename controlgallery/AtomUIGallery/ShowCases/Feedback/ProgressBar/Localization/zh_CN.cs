using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ProgressBar;

[LanguageProvider(LanguageCode.zh_CN, ProgressBarShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string ProgressBarTitle = "进度条";
    public const string ProgressBarDescription = "标准进度条。";
    public const string CircularProgressBarTitle = "环形进度条";
    public const string CircularProgressBarDescription = "环形进度条。";
    public const string MiniSizeTitle = "迷你尺寸进度条";
    public const string MiniSizeDescription = "适合较窄区域。";
    public const string SmallerCircularProgressBarTitle = "更小的环形进度条";
    public const string SmallerCircularProgressBarDescription = "更小的环形进度条。";
    public const string DynamicTitle = "动态进度";
    public const string DynamicDescription = "动态进度条效果更好。";
    public const string CustomTextFormatTitle = "自定义文本格式";
    public const string CustomTextFormatDescription = "可以通过设置 format 属性自定义文本。";
    public const string DashboardTitle = "仪表盘";
    public const string DashboardDescription = "设置 type=dashboard 可以轻松获得仪表盘样式进度条。修改 gapDegree 可设置缺口角度。";
    public const string SuccessSegmentTitle = "带成功分段的进度条";
    public const string SuccessSegmentDescription = "展示多个不同状态的进度片段。";
    public const string StrokeLinecapTitle = "线帽样式";
    public const string StrokeLinecapDescription = "设置 strokeLinecap='butt' 可以将线帽从 round 改为 butt，更多信息请参考 stroke-linecap。";
    public const string CustomLineGradientTitle = "自定义线性渐变";
    public const string CustomLineGradientDescription = "渐变封装；设置渐变时 circle 和 dashboard 会忽略 strokeLinecap。";
    public const string StepsTitle = "步骤进度条";
    public const string StepsDescription = "带步骤的进度条。";
    public const string CircularStepsTitle = "带步骤的环形进度条";
    public const string CircularStepsDescription = "支持步骤和颜色分段的环形进度条，默认间隔为 2px。";
    public const string PercentPositionTitle = "调整进度值位置";
    public const string PercentPositionDescription = "改变进度值的位置，可使用 percentPosition 将进度值调整到进度条内部、外部或底部。";
    public const string StepsPercentPositionTitle = "调整步骤进度值位置";
    public const string StepsPercentPositionDescription = "改变步骤进度值的位置，可使用 percentPosition 将进度值调整到进度条内部、外部或底部。";
    public const string VerticalLinearTitle = "垂直线性进度条";
    public const string VerticalLinearDescription = "普通线性进度条，支持指定附加区域的位置。";
    public const string VerticalStepsTitle = "垂直步骤进度条";
    public const string VerticalStepsDescription = "普通步骤进度条，支持指定附加区域的位置。";
    public const string ToggleDisabledStatusTitle = "切换禁用状态";
    public const string ToggleDisabledStatusDescription = "进度条处于禁用状态并使用禁用样式。";
    public const string P2ContentSub = "Sub";
    public const string P2ContentAdd = "Add";

    protected override Type GetResourceKindType() => typeof(ProgressBarShowCaseLangResourceKind);
}
