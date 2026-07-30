using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// CalendarViewCell 对 Automation 暴露 GridItem 与 SelectionItem 语义（spec §11）。
/// 选中/名称来自同一 Cell Model。
/// </summary>
internal sealed class CalendarViewCellAutomationPeer : ControlAutomationPeer, ISelectionItemProvider
{
    public CalendarViewCellAutomationPeer(CalendarViewCell owner) : base(owner)
    {
    }

    private CalendarViewCell Cell => (CalendarViewCell)Owner;

    protected override string GetClassNameCore() => "CalendarViewCell";

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;

    protected override string? GetNameCore() => Cell.DisplayText;

    public bool IsSelected => Cell.Model?.IsSelected ?? false;

    public ISelectionProvider? SelectionContainer => null;

    public void Select() => Cell.Activate();

    public void AddToSelection() => Cell.Activate();

    public void RemoveFromSelection()
    {
        // 单选面板不支持取消选择。
    }
}
