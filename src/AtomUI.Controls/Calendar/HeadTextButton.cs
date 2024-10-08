using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class HeadTextButton : AvaloniaButton
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (Transitions is null)
        {
            Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty,
                    GlobalTokenResourceKey.MotionDurationFast)
            };
        }
    }
}