using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class LoadingIndicatorToken : AbstractControlDesignToken
{
    public const string ID = "LoadingIndicator";

    public LoadingIndicatorToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 加载圆圈尺寸
    /// </summary>
    public double DotSize { get; set; }

    /// <summary>
    /// 小号加载圆圈尺寸
    /// </summary>
    public double DotSizeSM { get; set; }

    /// <summary>
    /// 大号加载圆圈尺寸
    /// </summary>
    public double DotSizeLG { get; set; }
    
    /// <summary>
    /// 大号加载器尺寸
    /// </summary>
    public double IndicatorSizeLG { get; set; }
    
    /// <summary>
    /// 加载器尺寸
    /// </summary>
    public double IndicatorSize { get; set; }
    
    /// <summary>
    /// 小号加载器尺寸
    /// </summary>
    public double IndicatorSizeSM { get; set; }

    /// <summary>
    /// 加载器的周期时间
    /// </summary>
    public TimeSpan IndicatorDuration { get; set; }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var controlHeightLG = SharedToken.ControlHeightLG;
        var controlHeight   = SharedToken.ControlHeight;
        DotSize           = controlHeightLG / 2;
        DotSizeSM         = controlHeightLG * 0.35;
        DotSizeLG         = controlHeight;
        IndicatorDuration = SharedToken.MotionDurationSlow * 4;
        IndicatorSizeLG   = 48;
        IndicatorSize     = 32;
        IndicatorSizeSM   = 16;
    }
}