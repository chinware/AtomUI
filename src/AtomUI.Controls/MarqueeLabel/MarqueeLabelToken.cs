using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class MarqueeLabelToken : AbstractControlDesignToken
{
    public const string ID = "MarqueeLabel";

    public MarqueeLabelToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 周期这件的间隔
    /// </summary>
    public double CycleSpace { get; set; }

    /// <summary>
    /// 默认速度，像素每秒
    /// </summary>
    public double DefaultSpeed { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        CycleSpace   = 200;
        DefaultSpeed = 150;
    }
}