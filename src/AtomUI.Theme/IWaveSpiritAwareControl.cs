using AtomUI.Controls;
using Avalonia;

namespace AtomUI.Theme;

public interface IWaveSpiritAwareControl : IMotionAwareControl
{
    bool IsWaveSpiritEnabled { get; }
}

public abstract class WaveSpiritAwareControlProperty : MotionAwareControlProperty
{
    public const string IsWaveSpiritEnabledPropertyName = "IsWaveSpiritEnabled";

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty
        = AvaloniaProperty.Register<WaveSpiritAwareControlProperty, bool>(IsWaveSpiritEnabledPropertyName);
}