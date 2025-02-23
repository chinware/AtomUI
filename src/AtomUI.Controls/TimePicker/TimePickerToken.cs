using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TimePickerToken : AbstractControlDesignToken
{
    public const string ID = "TimePicker";

    public TimePickerToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 时间选择项高度
    /// </summary>
    public double ItemHeight { get; set; }
    
    /// <summary>
    /// 时间选择项宽度
    /// </summary>
    public double ItemWidth { get; set; }

    /// <summary>
    /// 时间选择项内间距
    /// </summary>
    public Thickness ItemPadding { get; set; }

    /// <summary>
    /// 按钮区域对上的外边距
    /// </summary> 
    public Thickness ButtonsMargin { get; set; }

    /// <summary>
    /// 范围选择箭头外间距
    /// </summary>
    public Thickness RangePickerArrowMargin { get; set; }

    /// <summary>
    /// 选择指示器厚度
    /// </summary>
    public double RangePickerIndicatorThickness { get; set; }

    /// <summary>
    /// Header 头内间距
    /// </summary>
    public Thickness HeaderMargin { get; set; }
    
    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ItemWidth                     = 40;
        ItemHeight                    = SharedToken.ControlHeight - 4;
        ItemPadding                   = new Thickness(0, SharedToken.PaddingXXS);
        ButtonsMargin                 = new Thickness(0, SharedToken.MarginXS, 0, 0);
        RangePickerArrowMargin        = new Thickness(SharedToken.MarginXS, 0);
        RangePickerIndicatorThickness = SharedToken.LineWidthFocus;
        HeaderMargin                  = new Thickness(0, 0, 0, 3);
    }
}