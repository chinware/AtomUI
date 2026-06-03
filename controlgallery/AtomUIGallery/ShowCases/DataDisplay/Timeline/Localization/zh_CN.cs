using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Timeline;

[LanguageProvider(LanguageCode.zh_CN, TimelineShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法示例。";
    public const string ColorTitle = "颜色";
    public const string ColorDescription = "设置圆圈颜色。绿色表示完成或成功，红色表示警告或错误，蓝色表示进行中或其他默认状态，灰色表示未完成或禁用。";
    public const string LastNodeAndReversingTitle = "最后节点和反转";
    public const string LastNodeAndReversingDescription = "当时间轴未完成且仍在进行时，可在最后放置一个待处理节点。将 Pending 设置为有效值可显示待处理项；也可以自定义待处理内容和待处理图标。IsReverse 用于反转节点。";
    public const string AlternateTitle = "交替展示";
    public const string AlternateDescription = "交替展示的时间轴。";
    public const string LabelTitle = "标签";
    public const string LabelDescription = "使用 label 单独显示时间。";
    public const string RightAlternateTitle = "右侧交替展示";
    public const string RightAlternateDescription = "右侧交替展示的时间轴。";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiated = "2024-01-01 AtomUI 正式启动";
    public const string P2ContentN2024N08N12AfterMoreThanN7Months = "2024-08-12 经过 7 个多月的开发，AtomUI 正式开源。欢迎大家关注我们。";
    public const string P2ContentN2024N10N01ReleaseOfTheN0N0 = "2024-10-01 发布 0.0.1 预览版";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN1 = "2024-01-01 AtomUI 正式启动。1";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN2 = "2024-01-01 AtomUI 正式启动。2";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN3 = "2024-01-01 AtomUI 正式启动。3";
    public const string P2ContentToggleReverse = "切换反转";
    public const string P2ContentRecording = "记录中...";
    public const string P2ContentLeft = "左侧";
    public const string P2ContentRight = "右侧";
    public const string P2ContentAlternate = "交替";
    public const string P2ContentAtomuiOfficiallyInitiated = "AtomUI 正式启动";
    public const string P2ContentCreateAServicesSite = "创建服务站点";
    public const string P2ContentQinwareWebsiteOnline = "Qinware 网站上线";
    public const string P2ContentNetworkProblemsBeingSolved = "网络问题正在解决";

    protected override Type GetResourceKindType() => typeof(TimelineShowCaseLangResourceKind);
}
