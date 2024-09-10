using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class TabControlOverflowMenuItem : BaseOverflowMenuItem
{
    protected override Type StyleKeyOverride => typeof(BaseOverflowMenuItem);
    public TabItem? TabItem { get; set; }

    protected override void NotifyCloseRequest()
    {
        if (Parent is MenuBase menu)
        {
            var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabItem!);
            RaiseEvent(eventArgs);
            Dispatcher.UIThread.Post(() => { menu.Close(); });
        }
    }
}