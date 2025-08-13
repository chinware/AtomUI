using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

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
    /// 段落骨架屏单行高度
    /// Line height of paragraph skeleton
    /// </summary>
    public double ParagraphLineHeight { get; set; }
    
    /// <summary>
    /// 默认加载的时长
    /// </summary>
    public TimeSpan LoadingMotionDuration { get; set; }
    
    /// <summary>
    /// 默认加载的动画背景笔刷
    /// </summary>
    public IBrush? LoadingBackground { get; set; }

    public SkeletonToken()
        : base(ID)
    {
    }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        GradientFromColor     = SharedToken.ColorFillContent;
        GradientToColor       = SharedToken.ColorFill;
        TitleHeight           = SharedToken.ControlHeight / 2;
        BlockRadius           = SharedToken.BorderRadiusSM;
        ParagraphMarginTop    = new Thickness(0, SharedToken.UniformlyMarginLG + SharedToken.UniformlyMarginXXS, 0, 0);
        ParagraphLineHeight   = SharedToken.ControlHeight / 2;
        LoadingMotionDuration = TimeSpan.FromSeconds(1.4);
        LoadingBackground     = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop { Offset = 0.25, Color = GradientFromColor },
                new GradientStop { Offset = 0.37, Color = GradientToColor },
                new GradientStop { Offset = 0.63, Color = GradientFromColor }
            }
        };
    }
}