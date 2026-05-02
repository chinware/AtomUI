using Avalonia.Automation.Peers;

namespace AtomUI.Desktop.Controls;

public class SliderAutomationPeer : RangeBaseAutomationPeer
{
    public SliderAutomationPeer(Slider owner) : base(owner)
    {
    }

    protected override string GetClassNameCore()
    {
        return "Slider";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Slider;
    }
}