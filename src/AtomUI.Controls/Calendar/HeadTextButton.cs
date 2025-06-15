using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class HeadTextButton : AvaloniaButton
{

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<HeadTextButton>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty,
                    SharedTokenKey.MotionDurationFast)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }
}