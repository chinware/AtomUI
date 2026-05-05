using AtomUI.Desktop.Controls.Primitives.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Desktop.Controls.Primitives;

[ControlDesignToken]
internal class InfoPickerInputToken : AbstractControlDesignToken
{
    public const string ID = "InfoPickerInput";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    public InfoPickerInputToken()
        : base(ID)
    {
    }
    
    /// <summary>
    /// 范围选择箭头外间距
    /// </summary>
    public Thickness RangePickerArrowMargin { get; set; }

    /// <summary>
    /// 选择指示器厚度
    /// </summary>
    public double RangePickerIndicatorThickness { get; set; }
    
    /// <summary>
    /// 范围选择与锚点的间距，在 RangePicker 模式下
    /// </summary>
    public double RangeMarginToAnchor { get; set; }
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        RangePickerArrowMargin        = new Thickness(SharedToken.UniformlyMarginXS, 0);
        RangePickerIndicatorThickness = SharedToken.LineWidthFocus;
        RangeMarginToAnchor           = SharedToken.UniformlyMarginXS;
    }
    
    protected override Type GetTokenKindType() => typeof(InfoPickerInputTokenKind);
}