using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class ArrowDecoratedBoxToken : AbstractControlDesignToken
{
    public const string ID = "ArrowDecoratedBox";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// 箭头三角形大小
    /// </summary>
    public double ArrowSize { get; set; }

    /// <summary>
    /// 默认的内边距
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// 箭头描边颜色
    /// </summary>
    public Color ArrowStrokeColor { get; set; }

    /// <summary>
    /// 箭头描边粗细
    /// </summary>
    public double ArrowStrokeThickness { get; set; }

    public ArrowDecoratedBoxToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ArrowSize            = SharedToken.SizePopupArrow / 1.3;
        Padding              = SharedToken.PaddingXS;
        ArrowStrokeColor     = ColorUtils.FromRgbF(0.07, 0, 0, 0);
        ArrowStrokeThickness = 1;
    }

    protected override Type GetTokenKindType() => typeof(ArrowDecoratedBoxTokenKind);
}