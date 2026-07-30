using Avalonia.Automation.Peers;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>CalendarView 对 Automation 暴露 Grid（表格）语义（spec §11）。</summary>
internal sealed class CalendarViewAutomationPeer : ControlAutomationPeer
{
    public CalendarViewAutomationPeer(CalendarView owner) : base(owner)
    {
    }

    protected override string GetClassNameCore() => "CalendarView";

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Table;
}
