using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public static class WaveSpiritAwareControlExtensions
{
    public static IDisposable? ConfigureWaveSpiritBindingStyle(this IWaveSpiritAwareControl waveSpiritAwareControl)
    {
        if (waveSpiritAwareControl is Control control)
        {
            return new CompositeDisposable
            {
                TokenResourceBinder.CreateControlTokenBinding(
                    control,
                    MotionAwareControlProperty.IsMotionEnabledProperty,
                    SharedTokenKind.EnableMotion),
                TokenResourceBinder.CreateControlTokenBinding(
                    control,
                    WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty,
                    SharedTokenKind.EnableWaveSpirit)
            };
        }

        return waveSpiritAwareControl is StyledElement styledElement
            ? new CompositeDisposable
            {
                TokenResourceBinder.CreateGlobalTokenBinding(
                    styledElement,
                    MotionAwareControlProperty.IsMotionEnabledProperty,
                    SharedTokenKind.EnableMotion),
                TokenResourceBinder.CreateGlobalTokenBinding(
                    styledElement,
                    WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty,
                    SharedTokenKind.EnableWaveSpirit)
            }
            : null;
    }
}
