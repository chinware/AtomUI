using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// CalendarViewCell 对 Automation 暴露 SelectionItem 语义（spec §11），选中/名称来自同一 Cell Model。
/// 说明：GridItem 语义在当前 Avalonia 版本中不作为跨平台可实现的 AutomationPeer provider 接口暴露
/// （IGridItemProvider 仅存在于 Win32 interop 层），因此 Grid 结构语义由容器级
/// <see cref="CalendarViewAutomationPeer"/>（Table 控件类型）承载。
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
