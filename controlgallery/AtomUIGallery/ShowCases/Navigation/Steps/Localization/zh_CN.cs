using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.zh_CN, StepsShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioInteractive = "交互";
    public const string ScenarioVertical = "垂直";
    public const string ScenarioDotClickable = "点状与可点击";
    public const string ScenarioNavigation = "导航";
    public const string ScenarioProgress = "进度";
    public const string ScenarioInline = "内联";
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
    public const string P2DescriptionThisIsADescription = "这是一段描述。";
    public const string P2HeaderFinished = "已完成";
    public const string P2HeaderInProgress = "进行中";
    public const string P2HeaderWaiting = "等待中";
    public const string P2HeaderLogin = "登录";
    public const string P2HeaderVerification = "验证";
    public const string P2HeaderPay = "支付";
    public const string P2HeaderDone = "完成";
    public const string P2HeaderFirst = "第一项";
    public const string P2HeaderSecond = "第二项";
    public const string P2HeaderThird = "第三项";
    public const string P2HeaderStepN1 = "步骤 1";
    public const string P2HeaderStepN2 = "步骤 2";
    public const string P2HeaderStepN3 = "步骤 3";
    public const string P2HeaderStepN4 = "步骤 4";
    public const string P2HeaderFinishN1 = "完成 1";
    public const string P2HeaderFinishN2 = "完成 2";
    public const string P2HeaderCurrentProcess = "当前进行中";
    public const string P2HeaderWait = "等待";
    public const string P2SubHeaderLeftTime = "剩余 00:00:08";
    public const string P2SubHeaderWaitingForLongTime = "等待较长时间";
    public const string P2TextAntDesignTitleN1 = "Ant Design 标题 1";
    public const string P2TextAntDesignADesignLanguageForBackgroundApplications = "Ant Design 是由 Ant UED 团队提炼的后台应用设计语言";
    public const string P2TextAntDesignTitleN2 = "Ant Design 标题 2";
    public const string P2TextAntDesignTitleN3 = "Ant Design 标题 3";
    public const string P2TextAntDesignTitleN4 = "Ant Design 标题 4";

    public const string P2ContentFirstContent = "第一步内容";

    public const string P2ContentSecondContent = "第二步内容";

    public const string P2ContentLastContent = "最后一步内容";

    public const string P2ContentNext = "下一步";

    public const string P2ContentPrevious = "上一步";

    public const string P2ContentDone = "完成";

    protected override Type GetResourceKindType() => typeof(StepsShowCaseLangResourceKind);
}
