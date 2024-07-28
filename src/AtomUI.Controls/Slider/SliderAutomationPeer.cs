using Avalonia.Automation.Peers;

namespace AtomUI.Controls;

public class SliderAutomationPeer : RangeBaseAutomationPeer
{
   public SliderAutomationPeer(Slider owner) : base(owner)
   {
   }

   override protected string GetClassNameCore()
   {
      return "Slider";
   }

   override protected AutomationControlType GetAutomationControlTypeCore()
   {
      return AutomationControlType.Slider;
   }

}