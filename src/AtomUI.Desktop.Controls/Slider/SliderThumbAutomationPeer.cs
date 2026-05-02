using Avalonia.Automation.Peers;

namespace AtomUI.Desktop.Controls;

public class SliderThumbAutomationPeer : ControlAutomationPeer
{
    public SliderThumbAutomationPeer(SliderThumb owner) : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Thumb;
    }

    protected override bool IsContentElementCore()
    {
        return false;
    }
}