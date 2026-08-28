using Avalonia.Controls;
namespace AtomUI.Desktop.Controls;

internal class TabStripOverflowMenuItem : BaseOverflowMenuItem
{
    protected override Type StyleKeyOverride => typeof(BaseOverflowMenuItem);
    public TabStripItem? TabStripItem { get; set; }

    protected override void NotifyCloseRequest()
    {
        if (Parent is MenuBase)
        {
            var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabStripItem!);
            RaiseEvent(eventArgs);
        }
    }
}
