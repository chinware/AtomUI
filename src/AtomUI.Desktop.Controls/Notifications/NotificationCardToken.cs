using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class NotificationCardToken : AbstractControlDesignToken
{

    public NotificationCardToken()

    {
    }

    /// <summary>
    /// 提醒框背景色
    /// </summary>
    public Color NotificationBg { get; set; }

    /// <summary>
    /// 提醒框内边距
    /// </summary>
    public Thickness NotificationPadding { get; set; }

    /// <summary>
    /// 提醒框图标尺寸
    /// </summary>
    public double NotificationIconSize { get; set; }

    /// <summary>
    /// 提醒框图标外边距
    /// </summary>
    public Thickness NotificationIconMargin { get; set; }

    /// <summary>
    /// 提醒框关闭按钮尺寸
    /// </summary>
    public double NotificationCloseButtonSize { get; set; }
    
    /// <summary>
    /// 提醒框关闭按钮内间距
    /// </summary>
    public Thickness NotificationCloseButtonPadding { get; set; }
        
    /// <summary>
    /// 提醒框底部外边距
    /// </summary>
    public Thickness NotificationMarginBottom { get; set; }

    /// <summary>
    /// 提醒框上边缘外边距
    /// </summary>
    public Thickness NotificationTopMargin { get; set; }

    /// <summary>
    /// 提醒框下边缘外边距
    /// </summary>
    public Thickness NotificationBottomMargin { get; set; }

    /// <summary>
    /// 提醒框进度条背景色
    /// </summary>
    public IImmutableBrush? NotificationProgressBg { get; set; }

    /// <summary>
    /// 提醒框进度条高度
    /// </summary>
    public double NotificationProgressHeight { get; set; }

    /// <summary>
    /// 进度条外边距
    /// </summary>
    public Thickness NotificationProgressMargin { get; set; }

    /// <summary>
    /// 提醒框宽度
    /// </summary>
    public double NotificationWidth { get; set; }

    /// <summary>
    /// 内容外边距
    /// </summary>
    public Thickness NotificationContentMargin { get; set; }

    /// <summary>
    /// 标题栏的外边距
    /// </summary>
    public Thickness HeaderMargin { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        var compactPaddingMD = EffectiveGlobalToken.UniformlyPaddingMD * 2d / 3d;
        var compactPaddingLG = EffectiveGlobalToken.UniformlyPaddingLG * 2d / 3d;
        var compactMarginXS  = EffectiveGlobalToken.UniformlyMarginXS * 2d / 3d;
        var compactMarginSM  = EffectiveGlobalToken.UniformlyMarginSM * 2d / 3d;

        NotificationProgressHeight = 2;
        NotificationProgressMargin = new Thickness(0, 0, 0, 1);
        NotificationContentMargin = new Thickness(0, 0, 0, compactPaddingMD);
        NotificationPadding = new Thickness(compactPaddingLG, compactPaddingMD, compactPaddingLG, 0);
        NotificationBg = EffectiveGlobalToken.ColorBgElevated;
        NotificationIconSize = EffectiveGlobalToken.FontSizeLG * EffectiveGlobalToken.RelativeLineHeightLG;
        NotificationCloseButtonSize = EffectiveGlobalToken.ControlHeightLG * 0.55;
        NotificationMarginBottom = new Thickness(0, 0, 0, EffectiveGlobalToken.UniformlyMargin);
        NotificationTopMargin = new Thickness(EffectiveGlobalToken.UniformlyMarginLG, EffectiveGlobalToken.UniformlyMarginLG, EffectiveGlobalToken.UniformlyMarginLG, 0);
        NotificationBottomMargin =
            new Thickness(EffectiveGlobalToken.UniformlyMarginLG, 0, EffectiveGlobalToken.UniformlyMarginLG, EffectiveGlobalToken.UniformlyMarginLG);

        NotificationProgressBg = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new() { Color = EffectiveGlobalToken.ColorPrimaryBorderHover, Offset = 0 },
                new() { Color = EffectiveGlobalToken.ColorPrimary, Offset      = 1 }
            }
        }.ToImmutable();
        NotificationWidth              = 384;
        HeaderMargin                   = new Thickness(0, 0, 0, compactMarginXS);
        NotificationIconMargin         = new Thickness(0, 0, compactMarginSM, 0);
        NotificationCloseButtonPadding = EffectiveGlobalToken.PaddingXXS;
    }
    
}
