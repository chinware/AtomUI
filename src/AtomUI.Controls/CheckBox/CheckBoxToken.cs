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

    public double IndicatorTristateMarkSize { get; set; }
    
    public Thickness TextMargin { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        CheckIndicatorSize        = _globalToken.ControlInteractiveSize;
        IndicatorTristateMarkSize = _globalToken.FontSizeLG / 2;
        TextMargin                = new Thickness(_globalToken.MarginXXS, 0, 0, 0);
    }
}