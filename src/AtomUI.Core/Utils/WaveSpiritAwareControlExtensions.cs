using AtomUI.Controls;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI.Utils;

public static class WaveSpiritAwareControlExtensions
{
    public static void ConfigureWaveSpiritBindingStyle(this IWaveSpiritAwareControl waveSpiritAwareControl)
    {
        if (waveSpiritAwareControl is StyledElement styledElement)
        {
            var bindingStyle = new Style();
            bindingStyle.Add(MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKey.EnableMotion);
            bindingStyle.Add(WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty, SharedTokenKey.EnableWaveSpirit);
            styledElement.Styles.Add(bindingStyle);
        }
    }
}