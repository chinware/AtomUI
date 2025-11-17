using AtomUI.Controls.Utils;
using Avalonia.Animation;

namespace AtomUI.Controls;

public class CarouselNavButton : IconButton
{
    protected override void NotifyConfigureTransitions(Transitions transitions)
    {
        transitions.Add(TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty));
    }
}