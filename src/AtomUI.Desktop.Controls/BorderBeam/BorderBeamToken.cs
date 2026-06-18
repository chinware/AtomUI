using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class BorderBeamToken : AbstractControlDesignToken
{
    public const string ID = "BorderBeam";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public double BeamSize { get; set; }

    public double BeamOpacity { get; set; }

    public TimeSpan MotionDuration { get; set; }

    public double MaxVisibleStopPercent { get; set; }

    public BorderBeamToken()
        : base(ID)
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

    protected override Type GetTokenKindType() => typeof(BorderBeamTokenKind);
}
