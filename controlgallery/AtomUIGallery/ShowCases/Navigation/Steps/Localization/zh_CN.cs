using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.zh_CN, StepsShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的步骤条。";
    public const string MiniVersionTitle = "迷你版本";
    public const string MiniVersionDescription = "将 SizeType 设置为 Small 可获得迷你版本。";
    public const string WithIconTitle = "带图标";
    public const string WithIconDescription = "可以通过为项目设置 icon 属性使用自定义图标。";
    public const string SwitchStepTitle = "切换步骤";
    public const string SwitchStepDescription = "配合内容和按钮展示流程进度。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直方向的简单步骤条。";
    public const string VerticalMiniVersionTitle = "垂直迷你版本";
    public const string VerticalMiniVersionDescription = "垂直方向的简单迷你步骤条。";
    public const string ErrorStatusTitle = "错误状态";
    public const string ErrorStatusDescription = "通过 Steps 的 status 可以指定当前步骤状态。";
    public const string DotStyleTitle = "点状样式";
    public const string DotStyleDescription = "带进度点样式的步骤条。";
    public const string DotStyleVerticalTitle = "垂直点状样式";
    public const string DotStyleVerticalDescription = "垂直方向带进度点样式的步骤条。";
    public const string ClickableTitle = "可点击";
    public const string ClickableDescription = "设置 IsItemClickable=true 可让步骤项可点击。";
    public const string NavigationStepsTitle = "导航步骤";
    public const string NavigationStepsDescription = "导航式步骤。";
    public const string StepsWithProgressTitle = "带进度的步骤";
    public const string StepsWithProgressDescription = "带进度的步骤条。";
    public const string LabelPlacementTitle = "标签位置";
    public const string LabelPlacementDescription = "将 labelPlacement 设置为 vertical。";
    public const string InlineStepsTitle = "内联步骤";
    public const string InlineStepsDescription = "内联类型步骤，适合在列表内容场景中展示对象的流程和当前状态。";

    protected override Type GetResourceKindType() => typeof(StepsShowCaseLangResourceKind);
}
