using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CheckBoxToken : AbstractControlDesignToken
{
    public const string ID = "CheckBox";

    public CheckBoxToken()
        : base(ID)
    {
    }
    
    public double CheckIndicatorSize { get; set; }
    public double CheckedMarkSize { get; set; }

    public double IndicatorTristateMarkSize { get; set; }
    
    public Thickness TextMargin { get; set; }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        CheckIndicatorSize        = SharedToken.ControlInteractiveSize;
        CheckedMarkSize           = CheckIndicatorSize * 0.6;
        IndicatorTristateMarkSize = SharedToken.FontSizeLG / 2;
        TextMargin                = new Thickness(SharedToken.UniformlyMarginXXS, 0, 0, 0);
    }
}