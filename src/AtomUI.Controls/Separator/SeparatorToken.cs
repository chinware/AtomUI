using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
public class SeparatorToken : AbstractControlDesignToken
{
    public const string ID = "Separator";

    public SeparatorToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 文本横向内间距 单位 em
    /// </summary>
    public double TextPaddingInline { get; set; }

    /// <summary>
    /// 文本与边缘距离的比例，取值 0 ～ 1
    /// </summary>
    public double OrientationMarginPercent { get; set; }

    /// <summary>
    /// 纵向分割线的横向外间距
    /// </summary>
    public double VerticalMarginInline { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        TextPaddingInline        = 1.0;
        OrientationMarginPercent = 0.05;
        VerticalMarginInline     = _globalToken.MarginXS;
    }
}