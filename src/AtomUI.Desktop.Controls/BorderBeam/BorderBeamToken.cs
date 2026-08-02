using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class BorderBeamToken : AbstractControlDesignToken
{

    public double BeamSize { get; set; }

    public double BeamOpacity { get; set; }

    public TimeSpan MotionDuration { get; set; }

    public double MaxVisibleStopPercent { get; set; }

    public BorderBeamToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        BeamSize              = 100d;
        BeamOpacity           = 0.95d;
        MotionDuration        = TimeSpan.FromSeconds(6);
        MaxVisibleStopPercent = 70d;
    }

}
