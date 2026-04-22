using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;

namespace AtomUI.Controls;

public static class WaveSpiritAwareControlExtensions
{
    public static void ConfigureWaveSpiritBindingStyle(this IWaveSpiritAwareControl waveSpiritAwareControl)
    {
        if (waveSpiritAwareControl is StyledElement styledElement)
        {
            TokenResourceBinder.CreateTokenBinding(styledElement, MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKind.EnableMotion);
            TokenResourceBinder.CreateTokenBinding(styledElement, WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty, SharedTokenKind.EnableWaveSpirit);
        }
    }
}