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