using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class ToolTipToken : AbstractControlDesignToken
{
    public const string ID = "ToolTip";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    public ToolTipToken()
        : base(ID)
    {
    }

    /// <summary>
    /// tooltip 的最大宽度，超过了就换行
    /// </summary>
    public double ToolTipMaxWidth { get; set; }

    /// <summary>
    /// ToolTip 默认的前景色
    /// </summary>
    public Color ToolTipColor { get; set; }

    /// <summary>
    /// ToolTip 默认的背景色
    /// </summary>
    public Color ToolTipBackground { get; set; }

    /// <summary>
    /// ToolTip 默认的圆角
    /// </summary>
    public CornerRadius BorderRadiusOuter { get; set; }

    /// <summary>
    /// ToolTip 默认的内间距
    /// </summary>
    public Thickness Padding { get; set; }
    
    /// <summary>
    /// 动画时长
    /// </summary>
    public TimeSpan MotionDuration { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        ToolTipMaxWidth   = 250;
        ToolTipColor      = SharedToken.ColorTextLightSolid;
        ToolTipBackground = SharedToken.ColorBgSpotlight;
        BorderRadiusOuter = new CornerRadius(Math.Max(BorderRadiusOuter.TopLeft, 4),
            Math.Max(BorderRadiusOuter.TopRight, 4),
            Math.Max(BorderRadiusOuter.BottomLeft, 4),
            Math.Max(BorderRadiusOuter.BottomRight, 4));
        Padding        = new Thickness(SharedToken.UniformlyPaddingSM, SharedToken.UniformlyPaddingSM / 2 + 2);
        MotionDuration = SharedToken.MotionDurationMid;
    }
    
    protected override Type GetTokenKindType() => typeof(ToolTipTokenKind);
}