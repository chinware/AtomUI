using AtomUI.Theme.DesignTokens;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 新 Calendar 控件的 Design Token，收敛为八个公开视觉语义。
/// 与 DatePicker CalendarView 使用的旧 <see cref="CalendarToken"/> 完全独立。
/// </summary>
[ControlDesignToken]
internal class CalendarControlToken : AbstractControlDesignToken
{
    public const string ID = "CalendarControl";

    public CalendarControlToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 完整 Calendar 背景，派生自容器背景。
    /// </summary>
    public Color FullBg { get; set; }

    /// <summary>
    /// 完整 Calendar Panel 背景，派生自容器背景。
    /// </summary>
    public Color FullPanelBg { get; set; }

    /// <summary>
    /// 完整模式选中日期/月单元背景，派生自 active item 背景。
    /// </summary>
    public Color ItemActiveBg { get; set; }

    /// <summary>
    /// Year Select 最小宽度。
    /// </summary>
    public double YearControlWidth { get; set; }

    /// <summary>
    /// Month Select 最小宽度。
    /// </summary>
    public double MonthControlWidth { get; set; }

    /// <summary>
    /// Mini 内容高度。
    /// </summary>
    public double MiniContentHeight { get; set; }

    /// <summary>
    /// Fullscreen 日期/月单元最小高度。
    /// </summary>
    public double FullCellMinHeight { get; set; }

    /// <summary>
    /// Fullscreen 日期范围条默认高度。
    /// </summary>
    public double RangeBarHeight { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        var dateContentHeight = (SharedToken.FontHeightSM + SharedToken.UniformlyMarginXS) * 3 +
                                SharedToken.LineWidth * 2;

        FullBg = SharedToken.ColorBgContainer;
        FullPanelBg = SharedToken.ColorBgContainer;
        ItemActiveBg = SharedToken.ControlItemBgActive;
        YearControlWidth = 80;
        MonthControlWidth = 70;
        MiniContentHeight = 256;
        FullCellMinHeight = SharedToken.ControlHeightSM +
                            dateContentHeight +
                            SharedToken.UniformlyPaddingXS / 2 +
                            SharedToken.LineWidthBold;
        RangeBarHeight = SharedToken.ControlHeightSM - SharedToken.UniformlyMarginXXS;
    }
}
