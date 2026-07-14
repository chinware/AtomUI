using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.zh_CN, BorderBeamShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ComponentCategory = "其他";
    public const string ComponentIntroducedVersion = "v6.0.5";
    public const string PageSubtitle = "沿容器边界绘制动态高光。";
    public const string PageDescription = "BorderBeam 包裹一个内容控件，并在其边界上渲染不参与交互的流光层。它只承担装饰性强调，默认不受全局动效设置影响，并支持单色或渐变停靠点。";
    public const string BasicTitle = "基础";
    public const string BasicDescription = "包裹卡片以强调重要的工作台概览，同时不改变卡片自身交互模型。";
    public const string CustomizedColorTitle = "自定义颜色";
    public const string CustomizedColorDescription = "使用多个颜色停靠点构建不同流光预设，对齐 Ant Design 的自定义颜色示例。";
    public const string CustomizedColorCardDescription = "分段选择器会切换流光使用的颜色停靠点集合。";
    public const string NonUniformRadiusTitle = "非统一圆角";
    public const string NonUniformRadiusDescription = "被装饰容器自行裁剪圆角时，可将 Outset 设置为 0。";
    public const string NonUniformRadiusCardDescription = "顶部圆角保持较大半径，底部角保持直角。";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyColor = "单色流光颜色。ColorStops 非空时优先使用 ColorStops。";
    public const string ApiPropertyColorStops = "渐变停靠点集合。Percent 使用公开的 0-100 区间，并映射到可见流光段。";
    public const string ApiPropertyOutset = "流光相对有效边界的外扩距离。为 null 时使用有效边框厚度。";
    public const string ApiPropertyBorderThickness = "内容未暴露 BorderBeam 几何时使用的兜底边框厚度。";
    public const string ApiPropertyCornerRadius = "内容未暴露 BorderBeam 几何时使用的兜底圆角。";
    public const string ApiPropertyIsMotionEnabled = "控制当前实例的流光动画是否启用。默认值不绑定全局动效设置。";
    public const string ApiPropertyDuration = "流光完成一周运动的时长。";
    public const string ApiPropertyBeamSize = "移动高光段的基准尺寸。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameBeamSize = "移动高光段的默认尺寸。";
    public const string TokenNameBeamOpacity = "流光层默认透明度。";
    public const string TokenNameMotionDuration = "流光完成一周运动的默认时长。";
    public const string TokenNameMaxVisibleStopPercent = "用于为透明尾迹保留空间的最大可见停靠点百分比。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

}
