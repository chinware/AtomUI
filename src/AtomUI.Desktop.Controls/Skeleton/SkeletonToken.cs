using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.Tokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class SkeletonToken : AbstractControlDesignToken
{
    public const string ID = "Skeleton";
    
    /// <summary>
    /// 渐变色起点颜色
    /// Start color of gradient
    /// </summary>
    public Color GradientFromColor { get; set; }
    
    /// <summary>
    /// 渐变色终点颜色
    /// End color of gradient
    /// </summary>
    public Color GradientToColor { get; set; }
    
    /// <summary>
    /// 标题骨架屏高度
    /// Height of title skeleton
    /// </summary>
    public double TitleHeight { get; set; }
    
    /// <summary>
    /// 骨架屏圆角
    /// Border radius of skeleton
    /// </summary>
    public CornerRadius BlockRadius { get; set; }
    
    /// <summary>
    /// 段落骨架屏上间距
    /// Margin top of paragraph skeleton
    /// </summary>
    public Thickness ParagraphMarginTop { get; set; }
    
    /// <summary>
    /// 头像骨架屏右间距
    /// </summary>
    public Thickness AvatarMarginRight { get; set; }
    
    /// <summary>
    /// 段落骨架屏单行高度
    /// Line height of paragraph skeleton
    /// </summary>
    public double ParagraphLineHeight { get; set; }
    
    /// <summary>
    /// 段落骨架屏单行胶囊圆角值
    /// </summary>
    public CornerRadius ParagraphLineRoundCornerRadius { get; set; }
    
    /// <summary>
    /// 默认加载的时长
    /// </summary>
    public TimeSpan LoadingMotionDuration { get; set; }
    
    // 流光动画背景定义，等价于 Ant Design 的 400% 背景平移动效。
    public IBrush? LoadingBackgroundStart { get; set; }
    public IBrush? LoadingBackgroundMiddle { get; set; }
    public IBrush? LoadingBackgroundEnd { get; set; }
    
    public double ImageSize { get; set; }
    public double ImageContainerSize {  get; set; }
    public double ImageContainerMaxSize { get; set; }

    public SkeletonToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        GradientFromColor     = SharedToken.ColorFillContent;
        GradientToColor       = SharedToken.ColorFill;
        TitleHeight           = SharedToken.ControlHeight / 2;
        BlockRadius           = SharedToken.BorderRadiusSM;
        ParagraphMarginTop    = new Thickness(0, SharedToken.UniformlyMarginLG + SharedToken.UniformlyMarginXXS, 0, 0);
        AvatarMarginRight = new Thickness(0, 0, SharedToken.UniformlyMargin, 0);
        ParagraphLineHeight   = SharedToken.ControlHeight / 2;
        ParagraphLineRoundCornerRadius = new CornerRadius(ParagraphLineHeight / 2);
        LoadingMotionDuration = TimeSpan.FromSeconds(1.4);
        LoadingBackgroundStart  = CreateLoadingBackground(-3.0, 1.0);
        LoadingBackgroundMiddle = CreateLoadingBackground(-1.5, 2.5);
        LoadingBackgroundEnd    = CreateLoadingBackground(0.0, 4.0);

        var imageSizeBase = SharedToken.ControlHeight * 1.5;
        ImageSize             = imageSizeBase;
        ImageContainerSize    = imageSizeBase * 2;
        ImageContainerMaxSize = imageSizeBase * 4;
    }


    private LinearGradientBrush CreateLoadingBackground(double startX, double endX)
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(startX, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(endX, 0.5, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop { Offset = 0.25, Color = GradientFromColor },
                new GradientStop { Offset = 0.37, Color = GradientToColor },
                new GradientStop { Offset = 0.63, Color = GradientFromColor }
            }
        };
    }
}
