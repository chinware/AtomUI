using AtomUI.Controls;
using Avalonia.Animation;

namespace AtomUI.Desktop.Controls;

public class CarouselNavButton : IconButton
{
    protected override void NotifyConfigureTransitions(Transitions transitions)
    {
        transitions.Add(TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty));
    }
}