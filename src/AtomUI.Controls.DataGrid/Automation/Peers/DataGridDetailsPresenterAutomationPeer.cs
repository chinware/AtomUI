using Avalonia.Automation.Peers;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Automation.Peers;

public class DataGridDetailsPresenterAutomationPeer : ControlAutomationPeer
{
    public DataGridDetailsPresenterAutomationPeer(DataGridDetailsPresenter owner)
        : base(owner)
    {
    }

    public new DataGridDetailsPresenter Owner => (DataGridDetailsPresenter)base.Owner;
}
