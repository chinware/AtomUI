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
    /// 加载图标尺寸
    /// </summary>
    public double DotSize { get; set; }

    /// <summary>
    /// 加载图标尺寸
    /// </summary>
    public double DotSizeSM { get; set; }

    /// <summary>
    /// 大号加载图标尺寸
    /// </summary>
    public double DotSizeLG { get; set; }

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
    }
}