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
    public const string LastNodeAndReversingDescription = "当时间轴未完成且仍在进行时，可在最后放置一个幽灵节点。将 pending 设置为真值可显示待处理项；可以传入 React Element 自定义 pending 内容，同时使用 pendingDot={a React Element} 自定义待处理项圆点。reverse={true} 用于反转节点。";
    public const string AlternateTitle = "交替展示";
    public const string AlternateDescription = "交替展示的时间轴。";
    public const string LabelTitle = "标签";
    public const string LabelDescription = "使用 label 单独显示时间。";
    public const string RightAlternateTitle = "右侧交替展示";
    public const string RightAlternateDescription = "右侧交替展示的时间轴。";

    protected override Type GetResourceKindType() => typeof(TimelineShowCaseLangResourceKind);
}
