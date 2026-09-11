using AtomUI.Theme.DesignTokens;
using Avalonia;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal sealed class ShowCasePanelToken : AbstractControlDesignToken
{

    public Thickness ContentMargin { get; set; }
    public double MinItemWidth { get; set; }
    public int MaxColumns { get; set; }
    public double ColumnGap { get; set; }
    public double RowGap { get; set; }

    public ShowCasePanelToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ContentMargin = new Thickness(EffectiveGlobalToken.SizeUnit * 6);
        MinItemWidth  = EffectiveGlobalToken.SizeUnit * 90;
        MaxColumns    = 2;
        ColumnGap     = EffectiveGlobalToken.SizeUnit * 4;
        RowGap        = EffectiveGlobalToken.SizeUnit * 4;
    }

}
