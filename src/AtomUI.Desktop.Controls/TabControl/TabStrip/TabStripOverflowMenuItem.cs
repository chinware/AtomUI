using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal class TabStripOverflowMenuItem : BaseOverflowMenuItem
{
    protected override Type StyleKeyOverride => typeof(BaseOverflowMenuItem);
    public TabStripItem? TabStripItem { get; set; }

    protected override void NotifyCloseRequest()
    {
        if (Parent is MenuBase menu)
        {
            var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabStripItem!);
            RaiseEvent(eventArgs);
            if (menu is MenuFlyoutPresenter menuFlyoutPresenter)
            {
                var menuFlyout = menuFlyoutPresenter.MenuFlyout;
                menuFlyout?.Items.Remove(this);
                if (menuFlyout?.Items.Count == 0)
                {
                    Dispatcher.UIThread.Post(() => { menu.Close(); });
                }
            }
        }
    }
}