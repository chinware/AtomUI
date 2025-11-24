using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public interface IWaveSpiritAwareControl : IMotionAwareControl
{
    bool IsWaveSpiritEnabled { get; }
}

public abstract class WaveSpiritAwareControlProperty : MotionAwareControlProperty
{
    public const string IsWaveSpiritEnabledPropertyName = "IsWaveSpiritEnabled";
    public const string WaveSpiritBrushPropertyName = "WaveSpiritBrush";
    public const string WaveSpiritTypePropertyName = "WaveSpiritType";

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        AvaloniaProperty.Register<StyledElement, bool>(IsWaveSpiritEnabledPropertyName);
    
    public static readonly StyledProperty<IBrush?> WaveSpiritBrushProperty =
        AvaloniaProperty.Register<StyledElement, IBrush?>(WaveSpiritBrushPropertyName);
    
    public static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        AvaloniaProperty.Register<StyledElement, WaveSpiritType>(WaveSpiritTypePropertyName);
}