using AtomUI.Theme.DesignTokens;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 新 Calendar 控件的 Design Token，收敛为 Ant Design 6 Calendar 的六个公开视觉语义（spec §13）。
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

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        FullBg            = SharedToken.ColorBgContainer;
        FullPanelBg       = SharedToken.ColorBgContainer;
        ItemActiveBg      = SharedToken.ControlItemBgActive;
        YearControlWidth  = 80;
        MonthControlWidth = 70;
        MiniContentHeight = 256;
    }
}
