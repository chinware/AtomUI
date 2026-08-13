using System.ComponentModel;
#if NET9_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace AtomUI.Registration;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AotTrimRegistration
{
    internal const string AppContextSwitchName =
        "AtomUI.AotTrimRegistration.Enabled";
    private static readonly bool s_isEnabled =
        AppContext.TryGetSwitch(AppContextSwitchName, out var enabled) && enabled;

#if NET9_0_OR_GREATER
    [FeatureSwitchDefinition(AppContextSwitchName)]
#endif
    public static bool IsEnabled => s_isEnabled;
}
