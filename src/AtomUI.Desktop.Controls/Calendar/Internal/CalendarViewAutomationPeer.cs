using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>CalendarView 对 Automation 暴露 Table 语义，并提供当前选中 Cell。</summary>
internal sealed class CalendarViewAutomationPeer : ControlAutomationPeer, ISelectionProvider
{
    public CalendarViewAutomationPeer(CalendarView owner) : base(owner)
    {
    }

    protected override string GetClassNameCore() => "CalendarView";

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Table;

    private CalendarView View => (CalendarView)Owner;

    public bool CanSelectMultiple => false;

    public bool IsSelectionRequired => true;

    public IReadOnlyList<AutomationPeer> GetSelection()
    {
        var selected = new List<AutomationPeer>(1);
        foreach (var cell in View.GetRealizedCells())
        {
            if (cell.Model?.IsSelected == true && ControlAutomationPeer.CreatePeerForElement(cell) is { } peer)
            {
                selected.Add(peer);
                break;
            }
        }

        return selected;
    }
}
