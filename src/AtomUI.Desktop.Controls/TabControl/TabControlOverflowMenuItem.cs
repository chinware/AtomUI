using Avalonia.Controls;
namespace AtomUI.Desktop.Controls;

internal class TabControlOverflowMenuItem : BaseOverflowMenuItem
{
    protected override Type StyleKeyOverride => typeof(BaseOverflowMenuItem);
    public TabItem? TabItem { get; set; }

    protected override void NotifyCloseRequest()
    {
        if (Parent is MenuBase)
        {
            var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabItem!);
            RaiseEvent(eventArgs);
        }
    }
}
