using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls.Internal;

[ControlDesignToken]
internal class InfoPickerInputToken : AbstractControlDesignToken
{
    public const string ID = "InfoPickerInput";
    
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
    
    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        RangePickerArrowMargin        = new Thickness(SharedToken.MarginXS, 0);
        RangePickerIndicatorThickness = SharedToken.LineWidthFocus;
    }
}