using AtomUI.Controls.Utils;
using Avalonia.Animation;

namespace AtomUI.Controls;

public class CarouselNavButton : IconButton
{
    protected override Type StyleKeyOverride { get; } = typeof(CarouselNavButton);
    
    protected override void NotifyConfigureTransitions(Transitions transitions)
    {
        transitions.Add(TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty));
    }
}