using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.zh_CN, BorderBeamShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
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

}
